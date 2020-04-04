using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;

namespace MMCApi.NotificationHubs
{
    public class Notification_Test
    {
        public async Task<JObject> SendDiagnosticStatus(string pns, [FromBody]string message, string to_tag, bool Diagnostic)
        {
           // log.Info(MethodBase.GetCurrentMethod().Name + " - Started.");
          //  log.Info("Request for DriverId: " + to_tag);
            JObject json = new JObject();
            var jsonResult = "";
            //var user = HttpContext.Current.User.Identity.Name;
            var user = "Auto";
            string[] userTag = new string[1];
            //userTag[0] = "username:" + to_tag;
            //userTag[1] = "from:" + user;
         //   DriverInfoBL DriverInfoBL = new DriverInfoBL();
            userTag[0] = to_tag;
            //userTag[0] = to_tag;           
            HttpStatusCode ret = HttpStatusCode.InternalServerError;
            if (!(string.IsNullOrEmpty(userTag[0])))
            {
                string DiagnosticStatus = "";
                if (Diagnostic == true)
                {
                    DiagnosticStatus = "Yes";
                }
                else { DiagnosticStatus = "No"; }

                message = "Diagnostic Status : " + DiagnosticStatus;
                Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
                try
                {
                    switch (pns.ToLower())
                    {
                        case "apns":
                            // iOS                    
                            var alert = "{\"aps\":{\"alert\":\"" + message + "\"},\"Diagnostic\":\"" + Diagnostic.ToString() + "\",\"NotificationType\":\"DiagnosticStatus\"}";
                            outcome = await Notifications1.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                            jsonResult = alert;
                            break;
                        case "fcm":
                            // Android
                            var notif = "{ \"data\" : {\"From\":\"" + user.ToString() + "\",\"message\":\"" + message + "\",\"Diagnostic\":\"" + Diagnostic.ToString() + "\",\"NotificationType\":\"DiagnosticStatus\"}}";
                            outcome = await Notifications1.Instance.Hub.SendFcmNativeNotificationAsync(notif, userTag);
                            jsonResult = notif;
                            break;
                    }

                    if (outcome != null)
                    {
                        if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                            (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                        {
                            ret = HttpStatusCode.OK;
                            var jsonData = new
                            {
                                status = "success",
                                message = jsonResult
                            };
                            jsonResult = JsonConvert.SerializeObject(jsonData);
                            json = JObject.Parse(jsonResult);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ret = HttpStatusCode.BadRequest;
                 //   log.Error("Error Occurred in " + MethodBase.GetCurrentMethod().Name, ex);
                    var jsonData = new
                    {
                        status = "fail",
                        message = ex.Message
                    };
                    jsonResult = JsonConvert.SerializeObject(jsonData);
                    json = JObject.Parse(jsonResult);
                }
            }
            else
            {
                ret = HttpStatusCode.BadRequest;
               // log.Error("Error Occurred in " + MethodBase.GetCurrentMethod().Name + "Mobile Device ID Not Found");
                var jsonData = new
                {
                    status = "fail",
                    message = "Mobile Device ID Not Found"
                };
                jsonResult = JsonConvert.SerializeObject(jsonData);
                json = JObject.Parse(jsonResult);
            }
            return json;
            //return Request.CreateResponse(ret);
        }
    }
}
