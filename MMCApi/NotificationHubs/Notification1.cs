using Microsoft.Azure.NotificationHubs;
using MMCApi.Configurations;

namespace MMCApi.NotificationHubs
{
    public class Notifications1
    {
        public static Notifications1 Instance = new Notifications1();

        private NotificationHubConfiguration _configuration;
        private NotificationHubClient _hubClient;

        public NotificationHubClient Hub { get; set; }

        private Notifications1()
        { }
        private Notifications1(NotificationHubConfiguration configuration)
        {
            //Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://odtmobilens.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=/C93AAkDV1DZfGGXhTy2cp3Gh/wQBaB96GY3tbE8Yno=",
            //                                                             "ODTMobileNH");
            //Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://odtmobilens.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=JlanuIat2puNn7EmRZlW1RVlHf4c8nvap/F80eKNBfw=",
            //                                                            "odthubstage");
            Hub = NotificationHubClient.CreateClientFromConnectionString(_configuration.ConnectionString, _configuration.HubName);

        }
    }
}

