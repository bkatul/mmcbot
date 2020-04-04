using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Configurations
{
    public class NotificationHubConfiguration
    {
        public string ConnectionString { get; set; }
        public string HubName { get; set; }
    }
}
