using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.AuthorizeServices;
using MMCApi.Model;
using MMCApi.Repository;
using MMCApi.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace MMCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private SecurityController _security;
        private UserService _userService;
        public EmailUtility objEmail = new EmailUtility();
        private readonly IOptions<EmailSettingModel> _mailSettings;
        List<UserModel> lstClient = new List<UserModel>();
        UseDbClient objClient = new UseDbClient();
        private readonly ILogger<UserController> _logger;
        private IHostingEnvironment _env;

        public UserController(IOptions<MySettingsModel> app, IOptions<EmailSettingModel> mailSettings, ILogger<UserController> logger, IHostingEnvironment env)
        {
            _userService = new UserService(app);
            appSettings = app;
            _security = new SecurityController();
            _mailSettings = mailSettings;
            _logger = logger;
            _env = env;
        }

        #region Invite User

        [HttpPost]
        [Route("InviteUser")]
        public ActionResult<InvitationModel> InviteUser([FromBody]InvitationModel model)
        {
            string encryptedText = _security.Encrypt(model.Password);
            model.EmailInAddress = model.FirstName + '.' + model.LastName + '@' + appSettings.Value.ExchangeServerName;
            model.Password = encryptedText;
            var msg = new Message<InvitationModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var userCount = DbClientFactory<UseDbClient>.Instance.GetUserByEmailForUserCount(appSettings.Value.DbConn, model.EmailId);
                    if (userCount == "0" || userCount == "")
                    {
                        var companyId = DbClientFactory<UseDbClient>.Instance.GetCompanyIdByCompany(appSettings.Value.DbConn, model.Company);
                        model.CompanyId = Convert.ToInt32(companyId);
                        var result = DbClientFactory<UseDbClient>.Instance.InviteUser(model, appSettings.Value.DbConn);

                        if (result == "1")
                        {
                            WebClient client = new WebClient();
                            string fileString = client.DownloadString(new Uri("https://mmcbotstorage.blob.core.windows.net/mmcbotcontainer/SendInvitationTemplate.html"));
                            string body = string.Empty;
                            fileString = fileString.Replace("{UserName}", model.FirstName + ' ' + model.LastName);
                            fileString = fileString.Replace("{link}", "http://mmcbot7.azurewebsites.net/User/ChangePassword/?email=" + model.EmailId);
                            body = fileString;

                            bool isEmailSend = objEmail.SendInvitationOnEmail(model.EmailId, _security.Decrypt(model.Password),
                                                                   _mailSettings.Value.MailFrom, model.EmailId, _mailSettings.Value.Password, body);
                            if (isEmailSend)
                            {
                                msg.success = true;
                                msg.message = "Invite sent successfully";
                                msg.access_token = "";
                                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                            }
                            else
                            {
                                msg.success = false;
                                msg.message = "Unable to send email.";
                                msg.access_token = "";
                                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                            }
                        }
                        else if (result == "-1")
                        {
                            msg.success = false;
                            msg.message = "Unable to send invitation.";
                            msg.access_token = "";
                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                        }
                    }
                    else
                    {
                        msg.success = false;
                        msg.message = "User already exists.";
                        msg.access_token = "";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
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
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            catch (Exception ex)
            {
                msg.success = false;
                msg.message = ex.Message;
            }

            return Ok(msg);
        }
        #endregion

        #region Get Role

        [HttpGet]
        [Route("GetAllRole")]
        public IActionResult GetAllRole()
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetAllRole(appSettings.Value.DbConn);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllRoleMMC")]
        public IActionResult GetAllRoleMMC()
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetAllRoleMMC(appSettings.Value.DbConn);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetUserByRole")]
        public IActionResult GetUserByRole(string createdBy)
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetUserByRole(appSettings.Value.DbConn, createdBy);
            return Ok(data);
        }

        #endregion

        #region Insert and Update User

        [HttpPost]
        [Route("SaveUser")]
        public ActionResult<UserModel> SaveUser([FromBody]UserModel model)
        {
            string encryptedText = _security.Encrypt(model.Password);
            model.EmailInAddress = model.FirstName.ToLower() + '.' + model.LastName.ToLower() + '@' + appSettings.Value.ExchangeServerName;
            model.Password = encryptedText;
            var msg = new Message<UserModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var res = DbClientFactory<CompanyDbClient>.Instance.SaveCompany(model, appSettings.Value.DbConn);
                    if (res != "-1")
                    {
                        model.CompanyId = Convert.ToInt32(res);
                        var result = DbClientFactory<UseDbClient>.Instance.SaveUser(model, appSettings.Value.DbConn);

                        if (result == "1")
                        {
                            var token = _userService.GenerateTokenForSignup(model.EmailId, _security.Decrypt(model.Password));
                            lstClient = objClient.GetUserByEmailPassword(appSettings.Value.DbConn, model.EmailId, encryptedText);
                            List<AuthModel> user = new List<AuthModel>
                            {
                                new AuthModel {
                                    Role= lstClient[0].Role,
                                    Company=lstClient[0].Company,
                                    RegistrationId=lstClient[0].RegistrationId,
                                    Tag=lstClient[0].Tag,
                                    CompanyId=lstClient[0].CompanyId,
                                    Id=lstClient[0].Id,
                                    FirstName=lstClient[0].FirstName,
                                    LastName=lstClient[0].LastName,
                                }
                            };

                            msg.success = true;
                            msg.message = "User saved successfully.";
                            msg.access_token = token;
                            msg.registrationId = user[0].RegistrationId;
                            msg.tag = user[0].Tag;
                            msg.companyId = user[0].CompanyId;
                            msg.company = user[0].Company;
                            msg.user_type = user[0].Role;

                            msg.id = user[0].Id;
                            msg.firstName = user[0].FirstName;
                            msg.lastName = user[0].LastName;
                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                        }
                        else if (result == "-1")
                        {
                            msg.success = false;
                            msg.message = "Unable to save user.";
                            msg.access_token = "";
                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                        }
                        else
                        {
                            msg.success = false;
                            msg.message = "User already exists.";
                            msg.access_token = "";
                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                        }
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
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            catch (Exception ex)
            {
                msg.success = false;
                msg.message = ex.Message;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }

            return Ok(msg);
        }

        [HttpPost]
        [Route("SaveUserDetail")]
        public ActionResult<UserDetailModel> SaveUserDetail([FromBody]UserDetailModel model)
        {
            var msg = new Message<UserDetailModel>();
            try
            {
                var result = DbClientFactory<UseDbClient>.Instance.SaveUserDetail(model, appSettings.Value.DbConn);
                if (result == "1")
                {
                    msg.success = true;
                    msg.message = "User Detail updated successfully.";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                if (result == "-1")
                {
                    msg.success = false;
                    msg.message = "Unable to update user detail.";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            catch (Exception ex)
            {
                msg.success = false;
                msg.message = ex.Message;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            return Ok(msg);
        }

        #endregion

        //#region Forgot Password

        //[HttpPost]
        //[Route("ForgotPassword")]
        //public IActionResult ForgotPassword([FromBody]ForgotPasswordModel model)
        //{
        //    var msg = new AI_Message<ForgotPasswordModel>();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var result = DbClientFactory<UserDbClient>.Instance.ForgotPassword(model.EmailId, _security.Encrypt(model.Password), appSettings.Value.DbConn);
        //            if (result == "1")
        //            {
        //                msg.success = true;
        //                msg.message = "Password updated successfully.";
        //                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
        //            }
        //            else if (result == "2")
        //            {
        //                msg.success = false;
        //                msg.message = "Incorrect EmailId.";
        //                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
        //            }
        //            else if (result == "-1")
        //            {
        //                msg.success = false;
        //                msg.message = "Unable to update Password.";
        //                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
        //            }
        //        }
        //        else
        //        {
        //            var query = from state in ModelState.Values
        //                        from error in state.Errors
        //                        select error.ErrorMessage;

        //            var errorList = query.ToList();

        //            msg.success = false;
        //            msg.message = errorList[0];
        //            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msg.success = false;
        //        msg.message = ex.Message;
        //        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
        //    }
        //    return Ok(msg);
        //}

        //#endregion

        #region User Get By Created By
        [HttpGet]
        [Route("GetUserByCreatedBy")]
        public IActionResult GetUserByCreatedBy(string CreatedBy)
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetUserByCreatedBy(appSettings.Value.DbConn, CreatedBy);
            return Ok(data);
        }

        #endregion

        #region  GetUserByEmailId
        [HttpGet]
        [Route("GetUserByEmailId")]
        public IActionResult GetUserByEmailId(string emailid)
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetUserByEmailId(appSettings.Value.DbConn, emailid);
            return Ok(data);
        }

        #endregion

        #region User Get By Id

        [HttpGet]
        [Route("GetUserByUserId")]
        public IActionResult GetUserByUserId(int Id)
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetUserByUserId(appSettings.Value.DbConn, Id);
            return Ok(data);
        }

        #endregion

        #region Change Password

        [HttpPost]
        [Route("ChangePassword")]
        public IActionResult ChangePassword([FromBody]ChangePassword model)
        {
            var msg = new AI_Message<ChangePassword>();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = DbClientFactory<UseDbClient>.Instance.ChangePassword(model.EmailId, _security.Encrypt(model.Password), appSettings.Value.DbConn);
                    if (result == "1")
                    {
                        msg.success = true;
                        msg.message = "Password changed successfully.";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                    }
                    else if (result == "-1")
                    {
                        msg.success = false;
                        msg.message = "Unable to change Password.";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
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
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            catch (Exception ex)
            {
                msg.success = false;
                msg.message = ex.Message;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            return Ok(msg);
        }

        #endregion

        #region Update User User Profile
        [HttpPost]
        [Route("UpdateUserProfile")]
        public IActionResult UpdateUserProfile(string EmailId, string Address, string Country)
        {
            var msg = new FreshbookMessgae<UserModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var result = DbClientFactory<UseDbClient>.Instance.UpdateUserProfile(appSettings.Value.DbConn, EmailId, Address, Country);
                    if (result == "1")
                    {
                        msg.success = true;
                        msg.message = "User profile updated successfully";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                    }
                    else if (result == "-1")
                    {
                        msg.success = false;
                        msg.message = "Unable to update user profile";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
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
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            catch (Exception ex)
            {
                msg.success = false;
                msg.message = ex.Message;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            return Ok(msg);
        }
        #endregion


        #region Delete Receipt By Id
        [HttpPost]
        [Route("DeleteReceiptById")]
        public ActionResult<AI_ReceiptModel> DeleteReceiptById(int Id)
        {
            var msg = new FreshbookMessgae<AI_ReceiptModel>();
            var result = DbClientFactory<AI_ReceiptDbClient>.Instance.DeleteReceiptById(appSettings.Value.DbConn, Id);
            if (result == "1")
            {
                msg.success = true;
                msg.message = "Receipt deleted successfully";
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            else
            {
                msg.success = false;
                msg.message = "Something went wrong";
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            return Ok(msg);
        }
        #endregion

        #region Send Email When Forgot Password

        [HttpPost]
        [Route("ForgotPassword")]
        public ActionResult<AI_ReceiptModel> ForgotPassword(string Email)
        {
            var msg = new FreshbookMessgae<AI_ReceiptModel>();
            var data = DbClientFactory<UseDbClient>.Instance.GetUserByEmailId(appSettings.Value.DbConn, Email);
            if (data.Count > 0)
            {
                WebClient client = new WebClient();
                string fileString = client.DownloadString(new Uri("https://mmcbotstorage.blob.core.windows.net/mmcbotcontainer/ResetPasswordTemplate.html"));
                string body = string.Empty;
                fileString = fileString.Replace("{UserName}", Email);
                fileString = fileString.Replace("{link}", "http://mmcbot7.azurewebsites.net/User/ChangePassword/?email=" + Email);
                body = fileString;

                bool isEmailSend = objEmail.SendForgotPasswordEmail(_mailSettings.Value.MailFrom, Email, _mailSettings.Value.Password, body);
                if (isEmailSend)
                {
                    msg.success = true;
                    msg.message = "Password reset link has been sent to your email address, please check email";
                }
                else
                {
                    msg.success = false;
                    msg.message = "Unable to send email";
                }
            }
            else
            {
                msg.success = false;
                msg.message = "User does not exists";
            }
            return Ok(msg);
        }

        #endregion

        #region Get All Countries

        [HttpGet]
        [Route("GetAllCountries")]
        public IActionResult GetAllCountries()
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetAllCountries(appSettings.Value.DbConn);
            return Ok(data);
        }

        #endregion

        #region Get All Currency
        [HttpGet]
        [Route("GetAllCurrency")]
        public IActionResult GetAllCurrency()
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetAllCurrency(appSettings.Value.DbConn);
            return Ok(data);
        }

        #endregion
    }
}