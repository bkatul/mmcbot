using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.Configurations;
using MMCApi.NotificationHubs;
using MMCApi.Repository;
using MMCApi.Utility;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading.Tasks;
using Notification = MMCApi.NotificationHubs.Notification;

namespace MMCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushNotificationsController : ControllerBase
    {
        private NotificationHubProxy _notificationHubProxy;
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILogger<PushNotificationsController> _logger;
        private readonly IOptions<PushNotificationConfig> _notificationConfig;

        public PushNotificationsController(IOptions<NotificationHubConfiguration> standardNotificationHubConfiguration, IOptions<MySettingsModel> app, ILogger<PushNotificationsController> logger, IOptions<PushNotificationConfig> notificationConfig)
        {
            _notificationHubProxy = new NotificationHubProxy(standardNotificationHubConfiguration.Value);
            appSettings = app;
            _logger = logger;
            _notificationConfig = notificationConfig;
        }

        /// 
        /// <summary>
        /// Get registration ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("register")]
        public async Task<IActionResult> CreatePushRegistrationId()
        {
            var registrationId = await _notificationHubProxy.CreateRegistrationId();
            return Ok(registrationId);
        }

        /// 
        /// <summary>
        /// Delete registration ID and unregister from receiving push notifications
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpDelete("unregister/{registrationId}")]
        public async Task<IActionResult> UnregisterFromNotifications(string registrationId)
        {
            await _notificationHubProxy.DeleteRegistration(registrationId);
            return Ok();
        }

        /// 
        /// <summary>
        /// Register to receive push notifications
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceUpdate"></param>
        /// <returns></returns>
        [HttpPut("enable/{id}")]
        public async Task<IActionResult> RegisterForPushNotifications(string id, [FromBody] DeviceRegistration deviceUpdate)
        {
            HubResponse registrationResult = await _notificationHubProxy.RegisterForPushNotifications(id, deviceUpdate);

            if (registrationResult.CompletedWithSuccess)
            {
                return Ok();
            }

            return BadRequest("An error occurred while sending push notification: " + registrationResult.FormattedErrorMessages);
        }

        /// 
        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="newNotification"></param>
        /// <returns></returns>
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] Notification newNotification)
        {
            ReceiptModel modlReceipt = JsonConvert.DeserializeObject<ReceiptModel>(newNotification.Content);
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptById(appSettings.Value.DbConn, modlReceipt.id.ToString());

            //DeviceRegistration deviceUpdate = new DeviceRegistration();
            //deviceUpdate.Platform = MobilePlatform.fcm;
            //string[] str1 = new string[1];
            //str1[0] = data[0].CreatedBy;
            //deviceUpdate.Handle = "Test";
            //deviceUpdate.Tags = str1;

            //var registrationId = await _notificationHubProxy.CreateRegistrationId();
            //HubResponse registrationResult = await _notificationHubProxy.RegisterForPushNotifications(registrationId, deviceUpdate);

            string[] str = new string[1];
            str[0] = data[0].CreatedBy;
            //  str[0] = "deepak.gupta@beyondkey.com";
            //  str[1] = "nakul.paithankar@beyondkey.com";
            newNotification.Tags = str;

            newNotification.Platform = MobilePlatform.fcm;
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationHubProxy.SendNotification(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + "Successfully send notification");
                return Ok();
            }
            else
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + pushDeliveryResult.FormattedErrorMessages);
                return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
            }
        }

        //[HttpPost("sendNotification")]
        //public string SendNotification(string _body)
        //{
        //    String sResponseFromServer = "-1";
        //    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
        //    tRequest.Method = "post";
        //    //serverKey - Key from Firebase cloud messaging server  
        //    tRequest.Headers.Add(string.Format("Authorization: key={0}", _notificationConfig.Value.ServerKey));
        //    //Sender Id - From firebase project setting  
        //    tRequest.Headers.Add(string.Format("Sender: id={0}", _notificationConfig.Value.SenderId));
        //    tRequest.ContentType = "application/json";

        //    var payload = new
        //    {
        //        to = "fBIlN94S5jI:APA91bGsBHyam0EiPa3avwB6_SFKD59BYx1tOGItHQHp-ISIXwBGbQqw23DxcIwoIecBvIqAVxu82Ux44URy6-wH0uo6a-DAxoz6dGkU00oIWTjQiWNmA0xue1eNWhZS_ySU21NWNg_H",
        //        priority = "high",
        //        content_available = true,
        //        notification = new
        //        {
        //            body = _body,
        //            title = "MMC Bot",
        //            badge = 1
        //        },
        //        //data = new
        //        //{
        //        //    key1 = "value1",
        //        //    key2 = "value2"
        //        //}

        //    };

        //    string postbody = JsonConvert.SerializeObject(payload).ToString();
        //    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
        //    tRequest.ContentLength = byteArray.Length;
        //    using (Stream dataStream = tRequest.GetRequestStream())
        //    {
        //        dataStream.Write(byteArray, 0, byteArray.Length);
        //        using (WebResponse tResponse = tRequest.GetResponse())
        //        {
        //            using (Stream dataStreamResponse = tResponse.GetResponseStream())
        //            {
        //                if (dataStreamResponse != null)
        //                {
        //                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
        //                    {
        //                        sResponseFromServer = tReader.ReadToEnd();
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return sResponseFromServer;
        //}

        [HttpPost("sendNotification")]
        public async Task<IActionResult> SendNotificationWithReason([FromQuery] int id, string reason)
        {
            Notification newNotification = new Notification();
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptById(appSettings.Value.DbConn, id.ToString());
            string[] str = new string[1];
            str[0] = data[0].CreatedBy;
            newNotification.Tags = str;
            newNotification.Platform = MobilePlatform.fcm;
            newNotification.Content = "Your document " + data[0].VendorName + " is deleted by MMC team due to " + reason;
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationHubProxy.SendNotification(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + "Successfully send notification");
                return Ok();
            }
            else
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + pushDeliveryResult.FormattedErrorMessages);
                return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
            }
        }
    }
}