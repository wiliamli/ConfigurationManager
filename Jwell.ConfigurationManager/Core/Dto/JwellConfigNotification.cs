namespace Jwell.ConfigurationManager.Core.Dto
{
    public class JwellConfigNotification
    {
        private string _namespaceName;
        private long _notificationId;
        private volatile JwelNotificationMessages _messages;

        //for json converter
        public JwellConfigNotification()
        {
        }

        public JwellConfigNotification(string namespaceName, long notificationId)
        {
            _namespaceName = namespaceName;
            _notificationId = notificationId;
        }

        public string NamespaceName
        {
            get => _namespaceName;
            set => _namespaceName = value;
        }

        public long NotificationId
        {
            get => _notificationId;
            set => _notificationId = value;
        }

        public JwelNotificationMessages Messages
        {
            get => _messages;
            set => _messages = value;
        }


        public void AddMessage(string key, long notificationId)
        {
            if (_messages == null)
            {
                lock (this)
                {
                    if (_messages == null)
                    {
                        _messages = new JwelNotificationMessages();
                    }
                }
            }
            _messages.Put(key, notificationId);
        }


        public override string ToString()
        {
            return "JwellConfigNotification{" + "namespaceName='" + _namespaceName + '\'' + ", notificationId=" + _notificationId + '}';
        }

    }
}
