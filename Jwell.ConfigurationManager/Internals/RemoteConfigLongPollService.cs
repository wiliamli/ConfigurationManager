using Jwell.ConfigurationManager.Core;
using Jwell.ConfigurationManager.Core.Dto;
using Jwell.ConfigurationManager.Core.Schedule;
using Jwell.ConfigurationManager.Core.Utils;
using Jwell.ConfigurationManager.Enums;
using Jwell.ConfigurationManager.Exceptions;
using Jwell.ConfigurationManager.Logging;
using Jwell.ConfigurationManager.Util;
using Jwell.ConfigurationManager.Util.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Jwell.ConfigurationManager.Internals
{
    public class RemoteConfigLongPollService
    {
        private static readonly ILogger Logger = LogManager.CreateLogger(typeof(RemoteConfigLongPollService));
        private static readonly long InitNotificationId = -1;
        private readonly ConfigServiceLocator _serviceLocator;
        private readonly HttpUtil _httpUtil;
        private readonly IJwellOptions _options;
        private readonly ThreadSafe.Boolean _longPollingStarted;
        private readonly ThreadSafe.Boolean _longPollingStopped;
        private readonly ISchedulePolicy _longPollFailSchedulePolicyInSecond;
        private readonly ISchedulePolicy _longPollSuccessSchedulePolicyInMs;
        private readonly ConcurrentDictionary<string, ISet<RemoteConfigRepository>> _longPollNamespaces;
        private readonly ConcurrentDictionary<string, long?> _notifications;
        private readonly ConcurrentDictionary<string, JwelNotificationMessages> _remoteNotificationMessages; //namespaceName -> watchedKey -> notificationId

        public RemoteConfigLongPollService(ConfigServiceLocator serviceLocator, HttpUtil httpUtil, IJwellOptions configUtil)
        {
            _serviceLocator = serviceLocator;
            _httpUtil = httpUtil;
            _options = configUtil;
            _longPollFailSchedulePolicyInSecond = new ExponentialSchedulePolicy(1, 120); //in second
            _longPollSuccessSchedulePolicyInMs = new ExponentialSchedulePolicy(100, 1000); //in millisecond
            _longPollingStarted = new ThreadSafe.Boolean(false);
            _longPollingStopped = new ThreadSafe.Boolean(false);
            _longPollNamespaces = new ConcurrentDictionary<string, ISet<RemoteConfigRepository>>();
            _notifications = new ConcurrentDictionary<string, long?>();
            _remoteNotificationMessages = new ConcurrentDictionary<string, JwelNotificationMessages>();
        }

        public void Submit(string namespaceName, RemoteConfigRepository remoteConfigRepository)
        {
            var remoteConfigRepositories = _longPollNamespaces.GetOrAdd(namespaceName, _ => new HashSet<RemoteConfigRepository>());

            remoteConfigRepositories.Add(remoteConfigRepository);

            _notifications.TryAdd(namespaceName, InitNotificationId);

            if (!_longPollingStarted.ReadFullFence())
                StartLongPolling();
        }


        private void StartLongPolling()
        {
            if (!_longPollingStarted.CompareAndSet(false, true))
            {
                //already started
                return;
            }
            try
            {
                var appId = _options.AppId;
                var cluster = _options.Cluster;
                var dataCenter = _options.DataCenter;

                var unused = DoLongPollingRefresh(appId, cluster, dataCenter);
            }
            catch (Exception ex)
            {
                var exception = new JwellConfigException("Schedule long polling refresh failed", ex);
                Logger.Warn(exception.GetDetailMessage());
            }
        }

        private async Task DoLongPollingRefresh(string appId, string cluster, string dataCenter)
        {
            var random = new Random();
            ServiceDto lastServiceDto = null;

            while (!_longPollingStopped.ReadFullFence())
            {
                var sleepTime = 50; //default 50 ms
                string url = null;
                try
                {
                    if (lastServiceDto == null)
                    {
                        var configServices = await _serviceLocator.GetConfigServices().ConfigureAwait(false);
                        lastServiceDto = configServices[random.Next(configServices.Count)];
                    }

                    url = AssembleLongPollRefreshUrl(lastServiceDto.HomepageUrl, appId, cluster, dataCenter);

                    Logger.Debug($"Long polling from {url}");

                    var response = await _httpUtil.DoGetAsync<IList<JwellConfigNotification>>(url, 600000).ConfigureAwait(false);

                    Logger.Debug(
                        $"Long polling response: {response.StatusCode}, url: {url}");
                    if (response.StatusCode == HttpStatusCode.OK && response.Body != null)
                    {
                        UpdateNotifications(response.Body);
                        UpdateRemoteNotifications(response.Body);
                        Notify(lastServiceDto, response.Body);
                        _longPollSuccessSchedulePolicyInMs.Success();
                    }
                    else
                    {
                        sleepTime = _longPollSuccessSchedulePolicyInMs.Fail();
                    }

                    //try to load balance
                    if (response.StatusCode == HttpStatusCode.NotModified && random.NextDouble() >= 0.5)
                    {
                        lastServiceDto = null;
                    }

                    _longPollFailSchedulePolicyInSecond.Success();
                }
                catch (Exception ex)
                {
                    lastServiceDto = null;

                    var sleepTimeInSecond = _longPollFailSchedulePolicyInSecond.Fail();
                    Logger.Warn(
                        $"Long polling failed, will retry in {sleepTimeInSecond} seconds. appId: {appId}, cluster: {cluster}, namespace: {AssembleNamespaces()}, long polling url: {url}, reason: {ex.GetDetailMessage()}");

                    sleepTime = sleepTimeInSecond * 1000;
                }
                finally
                {
                    await Task.Delay(sleepTime).ConfigureAwait(false);
                }
            }
        }

        private void Notify(ServiceDto lastServiceDto, IList<JwellConfigNotification> notifications)
        {
            if (notifications == null || notifications.Count == 0)
            {
                return;
            }
            foreach (var notification in notifications)
            {
                var namespaceName = notification.NamespaceName;

                //create a new list to avoid ConcurrentModificationException
                var toBeNotified = new List<RemoteConfigRepository>();
                if (_longPollNamespaces.TryGetValue(namespaceName, out var registries) && registries != null)
                    toBeNotified.AddRange(registries);

                //since .properties are filtered out by default, so we need to check if there is any listener for it
                if (_longPollNamespaces.TryGetValue($"{namespaceName}.{ConfigFileFormat.Properties.GetString()}", out registries) && registries != null)
                    toBeNotified.AddRange(registries);

                _remoteNotificationMessages.TryGetValue(namespaceName, out var originalMessages);
                var remoteMessages = originalMessages?.Clone();
                foreach (var remoteConfigRepository in toBeNotified)
                {
                    try
                    {
                        remoteConfigRepository.OnLongPollNotified(lastServiceDto, remoteMessages);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex);
                    }
                }
            }
        }

        private void UpdateNotifications(IList<JwellConfigNotification> deltaNotifications)
        {
            foreach (var notification in deltaNotifications)
            {
                if (string.IsNullOrEmpty(notification.NamespaceName))
                {
                    continue;
                }
                var namespaceName = notification.NamespaceName;
                if (_notifications.ContainsKey(namespaceName))
                {
                    _notifications[namespaceName] = notification.NotificationId;
                }
                //since .properties are filtered out by default, so we need to check if there is notification with .properties suffix
                var namespaceNameWithPropertiesSuffix = $"{namespaceName}.{ConfigFileFormat.Properties.GetString()}";
                if (_notifications.ContainsKey(namespaceNameWithPropertiesSuffix))
                {
                    _notifications[namespaceNameWithPropertiesSuffix] = notification.NotificationId;
                }
            }
        }

        private void UpdateRemoteNotifications(IList<JwellConfigNotification> deltaNotifications)
        {
            foreach (var notification in deltaNotifications)
            {
                if (string.IsNullOrEmpty(notification.NamespaceName))
                {
                    continue;
                }

                if (notification.Messages == null || notification.Messages.IsEmpty())
                {
                    continue;
                }

                var localRemoteMessages = _remoteNotificationMessages.GetOrAdd(notification.NamespaceName, _ => new JwelNotificationMessages());

                localRemoteMessages.MergeFrom(notification.Messages);
            }
        }

        private string AssembleNamespaces()
        {
            return string.Join(ConfigConsts.ClusterNamespaceSeparator, _longPollNamespaces.Keys);
        }

        private string AssembleLongPollRefreshUrl(string uri, string appId, string cluster, string dataCenter)
        {
            if (!uri.EndsWith("/", StringComparison.Ordinal))
            {
                uri += "/";
            }
            var uriBuilder = new UriBuilder(uri + "notifications/v2");
            var query = new Dictionary<string, string>();

            query["appId"] = appId;
            query["cluster"] = cluster;
            query["notifications"] = AssembleNotifications(_notifications);

            if (!string.IsNullOrEmpty(dataCenter))
            {
                query["dataCenter"] = dataCenter;
            }
            var localIp = _options.LocalIp;
            if (!string.IsNullOrEmpty(localIp))
            {
                query["ip"] = localIp;
            }

            uriBuilder.Query = QueryUtils.Build(query);

            return uriBuilder.ToString();
        }

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private string AssembleNotifications(IDictionary<string, long?> notificationsMap)
        {
            return JsonConvert.SerializeObject(notificationsMap
                .Select(kvp => new JwellConfigNotification
                {
                    NamespaceName = kvp.Key,
                    NotificationId = kvp.Value.GetValueOrDefault(InitNotificationId)
                }), JsonSettings);
        }
    }
}
