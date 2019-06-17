using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jwell.ConfigurationManager.Core.Dto
{
    public class JwelNotificationMessages
    {
        private IDictionary<string, long> _details;

        public JwelNotificationMessages()
            :this(new Dictionary<string, long>())
        {
        }

        private JwelNotificationMessages(IDictionary<string, long> details)
        {
            _details = details;
        }

        public void Put(string key, long notificationId)
        {
            _details[key] = notificationId;
        }

        public long Get(string key)
        {
            return _details[key];
        }

        public bool Has(string key)
        {
            return _details.ContainsKey(key);
        }

        public bool IsEmpty()
        {
            return _details.Count == 0;
        }

        public IDictionary<string, long> Details
        {
            get => _details;
            set => _details = value;
        }


        public void MergeFrom(JwelNotificationMessages source)
        {
            if (source == null)
            {
                return;
            }

            foreach (var entry in source.Details)
            {
                //to make sure the notification id always grows bigger
                if (Has(entry.Key) && Get(entry.Key) >= entry.Value)
                {
                    continue;
                }
                Put(entry.Key, entry.Value);
            }
        }

        public JwelNotificationMessages Clone()
        {
            return new JwelNotificationMessages(new Dictionary<string, long>(Details));
        }

    }
}
