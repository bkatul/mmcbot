using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.AuthorizeServices;
using MMCApi.Model;
using MMCApi.Repository;
using MMCApi.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace MMCApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private IUserService _userService;
        private SecurityController _security;
        List<UserModel> lstClient = new List<UserModel>();
        UseDbClient objClient = new UseDbClient();
        private readonly ILogger<AuthController> _logger;

        private readonly IOptions<MySettingsModel> appSettings;
        public AuthController(IUserService userService, IOptions<MySettingsModel> app, ILogger<AuthController> logger)
        {
            _userService = userService;
            appSettings = app;
            _security = new SecurityController();
            _logger = logger;
        }

        #region User Authenticate and Token Generate

        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public ActionResult<AuthModel> Authenticate([FromBody]AuthModel userParam)
        {
            var msg = new Message<UserModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Get Login Token.");

                    var token = _userService.GenerateTokenForSignup(userParam.Username, userParam.Password);

                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Token: " + token.ToString());

                    var res = DbClientFactory<UseDbClient>.Instance.UpdateUserOnLogin(appSettings.Value.DbConn, userParam.Username, userParam.RegistrationId, userParam.Tag);

                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Update User On Login");

                    if (res == "1")
                    {
                        if (token != "")
                        {
                            lstClient = objClient.GetUserByEmailPassword(appSettings.Value.DbConn, userParam.Username, _security.Encrypt(userParam.Password));
                            List<AuthModel> user = new List<AuthModel>
                            {
                                new AuthModel {
                                    Role= lstClient[0].Role,
                                    Company=lstClient[0].Company,
                                    RegistrationId=lstClient[0].RegistrationId,
                                    Tag=lstClient[0].Tag,
                                    CompanyId=lstClient[0].CompanyId,
                                    Id=lstClient[0].Id,
                                    //FirstName=lstClient[0].FirstName,
                                   FirstName = UppercaseFirst(lstClient[0].FirstName),
                                   LastName = UppercaseFirst(lstClient[0].LastName),
                                   EmailId=lstClient[0].EmailId
                          //  LastName =lstClient[0].LastName,
                                }
                            };

                            msg.success = true;
                            msg.access_token = token;
                            msg.message = "Login successfully.";
                            msg.user_type = user[0].Role;
                            msg.company = user[0].Company;
                            msg.registrationId = user[0].RegistrationId;
                            msg.tag = user[0].Tag;
                            msg.companyId = user[0].CompanyId;
                            msg.emailId = user[0].EmailId;

                            msg.id = user[0].Id;
                            msg.firstName = user[0].FirstName;
                            msg.lastName = user[0].LastName;
                            // Get latest refresh_token

                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Refresh token access start.");
                            string refresh_token = DbClientFactory<UseDbClient>.Instance.AccessFreshbookDetailByCompanyId(appSettings.Value.DbConn, msg.companyId);
                            if (refresh_token != "")
                            {
                                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Refresh token: " + refresh_token);

                                var token_update = UpdateFreshbookDetail(refresh_token, msg.companyId);
                                if (token_update == "-1" || token_update == "Unauthorized freshbook authentication")
                                {
                                    msg.success = true;
                                    msg.access_token = token;
                                    msg.message = "Login successfully.";
                                    msg.user_type = user[0].Role;
                                    msg.company = user[0].Company;
                                    msg.registrationId = user[0].RegistrationId;
                                    msg.tag = user[0].Tag;
                                    msg.companyId = user[0].CompanyId;

                                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Unauthorized freshbook authentication.");
                                }
                                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Login successfully.");
                            }

                        }
                        else
                        {
                            msg.success = false;
                            msg.access_token = "";
                            msg.message = "Incorrect email address or password";
                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " User not found.");
                        }
                    }
                    else
                    {
                        msg.success = false;
                        msg.access_token = "";
                        msg.message = "Problem occured in update.";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Problem occured in update.");
                    }
                }
                else
                {
                    var query = from state in ModelState.Values
                                from error in state.Errors
                                select error.ErrorMessage;

                    var errorList = query.ToList();

                    msg.success = false;
                    msg.message = errorList[0];
                    msg.access_token = "";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + errorList[0]);
                }
            }
            catch (Exception ex)
            {
                msg.success = false;
                msg.message = ex.Message;
            }
            return Ok(msg);
        }

        static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private string UpdateFreshbookDetail(string refresh_token, int companyId)
        {
            string data = GetNewRefreshToken(refresh_token);
            if (data.ToString() != "" && data.ToString() != "UNAUTHORIZED")
            {
                FreshbookTokenModel modelObject = JsonConvert.DeserializeObject<FreshbookTokenModel>(data);
                if (modelObject != null)
                {
                    var res = DbClientFactory<UseDbClient>.Instance.UpdateTokenInDatabase(appSettings.Value.DbConn, modelObject.access_token, modelObject.refresh_token, companyId);
                    return res.ToString();
                }
            }
            return "Unauthorized freshbook authentication";
        }

        public string GetNewRefreshToken(string refresh_token)
        {
            var result = "";
            HttpClient http = new HttpClient();

            var data = new
            {
                grant_type = appSettings.Value.Grant_Type,
                client_secret = appSettings.Value.Client_Secret,
                client_id = appSettings.Value.Client_Id,
                redirect_uri = appSettings.Value.Redirect_Uri,
                refresh_token = refresh_token
            };

            var postdata = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            StringContent content = new StringContent(postdata, Encoding.UTF8, "application/json");
            var response = http.PostAsync("https://api.freshbooks.com/auth/oauth/token", content).Result;
            if (response.IsSuccessStatusCode == true && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = response.Content.ReadAsStringAsync().Result;
                return result.ToString();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                result = response.ReasonPhrase.ToString();
                return result;
            }
            return result;
        }

        #endregion
    }
}