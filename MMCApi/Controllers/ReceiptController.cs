using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.Configurations;
using MMCApi.Model;
using MMCApi.NotificationHubs;
using MMCApi.Repository;
using MMCApi.Repository.Receipt;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Notification = MMCApi.NotificationHubs.Notification;

namespace MMCApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : Controller
    {
        #region Variables

        private NotificationHubProxy _notificationHubProxy;
        private readonly IOptions<MySettingsModel> _appSettings;
        private readonly IOptions<EmailSettingModel> _mailSettings;
        private readonly ILogger<UserController> _logger;
        public EmailUtility objEmail = new EmailUtility();
        private readonly IOptions<PushNotificationConfig> _notificationConfig;
        #endregion

        #region Constructor
        public ReceiptController(IOptions<NotificationHubConfiguration> standardNotificationHubConfiguration, IOptions<MySettingsModel> appSettings, IOptions<EmailSettingModel> mailSettings, ILogger<UserController> logger, IOptions<PushNotificationConfig> notificationConfig)
        {
            _notificationHubProxy = new NotificationHubProxy(standardNotificationHubConfiguration.Value);
            _appSettings = appSettings;
            _mailSettings = mailSettings;
            _logger = logger;
            _notificationConfig = notificationConfig;
        }

        #endregion

        #region Methods

        #region Archived / Unarchived receipts

        [HttpPost]
        [Route("ArchivedOrUnarchivedReceipt")]
        public IActionResult ArchivedOrUnarchivedReceipt([FromBody]List<AI_GetReceiptModel> model, bool isArchived)
        {
            var msg = new CommonMessgae();
            try
            {
                foreach (var item in model)
                {
                    if (isArchived == true)
                    {
                        item.IsArchived = true;
                    }
                    else
                    {
                        item.IsArchived = false;
                    }

                    var result = DbClientFactory<ReceiptDbClient>.Instance.ArchivedOrUnarchivedReceipt(item, _appSettings.Value.DbConn);

                    if (result == "1")
                    {
                        msg.success = true;
                        if (isArchived)
                        {
                            // Send notification to user for archived receipt
                            var isSend = SendNotificationOnArchiveUnarchive(item.Id, isArchived);
                            msg.success = true;
                            msg.message = "Receipt archived successfully";
                        }
                        else
                        {
                            var isSend = SendNotificationOnArchiveUnarchive(item.Id, isArchived);
                            msg.success = true;
                            msg.message = "Receipt unarchived successfully";
                        }

                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                    }
                    else if (result == "-1")
                    {
                        msg.success = false;
                        if (isArchived)
                        {
                            msg.success = false;
                            msg.message = "Unable to archive receipt";
                        }
                        else
                        {
                            msg.success = false;
                            msg.message = "Unable to unarchive receipt";
                        }
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Ok(msg);
        }

        private async Task<bool> SendNotificationOnArchiveUnarchive(int id, bool IsArchived)
        {
            Notification newNotification = new Notification();
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptById(_appSettings.Value.DbConn, id.ToString());
            string[] str = new string[1];
            str[0] = data[0].CreatedBy;
            newNotification.Tags = str;
            newNotification.Platform = MobilePlatform.fcm;
            if (IsArchived == true)
            {
                newNotification.Content = "Your document " + data[0].VendorName + " is archived by administrator";
            }
            else
            {
                newNotification.Content = "Your document " + data[0].VendorName + " is urchived by administrator";
            }
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationHubProxy.SendNotification(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + "Successfully send notification");
                return true;
            }
            else
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + pushDeliveryResult.FormattedErrorMessages);
                return false;
            }
        }

        #endregion

        #region Delete receipts

        [HttpPost]
        [Route("DeleteSingleOrMultipleReceipt")]
        public IActionResult DeleteSingleOrMultipleReceipt([FromBody]List<AI_GetReceiptModel> model)
        {
            var msg = new CommonMessgae();
            try
            {
                foreach (var item in model)
                {
                    var isDelete = SendNotificationOnDelete(item.Id);
                    var result = DbClientFactory<AI_ReceiptDbClient>.Instance.DeleteReceiptById(_appSettings.Value.DbConn, item.Id);
                    if (result == "1")
                    {
                        msg.success = true;
                        msg.message = "Receipt deleted successfully";
                    }
                    else
                    {
                        msg.success = false;
                        msg.message = "Unable to delete receipt";
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Ok(msg);
        }

        private async Task<bool> SendNotificationOnDelete(int id)
        {
            Notification newNotification = new Notification();
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptById(_appSettings.Value.DbConn, id.ToString());
            string[] str = new string[1];
            str[0] = data[0].CreatedBy;
            newNotification.Tags = str;
            newNotification.Platform = MobilePlatform.fcm;
            newNotification.Content = "Your document " + data[0].VendorName + " is deleted by administrator";
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationHubProxy.SendNotification(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + "Successfully send notification");
                return true;
            }
            else
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + pushDeliveryResult.FormattedErrorMessages);
                return false;
            }
        }
        #endregion

        #region Search Receipt On Mobile
        [HttpGet]
        [Route("SearchReceiptDataMobile")]
        public IActionResult SearchReceiptDataMobile(string status, string searchVal)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.SearchReceiptDataMobile(_appSettings.Value.DbConn, status, searchVal);
            return Ok(data);
        }

        #endregion

        #endregion
    }
}