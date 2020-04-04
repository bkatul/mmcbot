using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiAdoDemo.Model
{
    public class MySettingsModel
    {
        public string DbConn { get; set; }
        public string Secret { get; set; }
        public string Url { get; set; }
        public string SubscriptionKey { get; set; }
        public string Client_Id { get; set; }
        public string Client_Secret { get; set; }
        public string Redirect_Uri { get; set; }
        public string Grant_Type { get; set; }
        public string ExchangeServerName { get; set; }
    }

    public class MyConfig
    {
        public string StorageConnection { get; set; }
        public string Container { get; set; }
    }

    public class PushNotificationConfig
    {
        public string ServerKey { get; set; }
        public string SenderId { get; set; }
    }
}
