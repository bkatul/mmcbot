using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.Model.UserMMC;
using MMCApi.Repository;
using MMCApi.Utility;
using Newtonsoft.Json;
using System.Reflection;

namespace MMCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MMCUserController : Controller
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private SecurityController _security;
        private readonly ILogger<MMCUserController> _logger;

        public MMCUserController(IOptions<MySettingsModel> app, ILogger<MMCUserController> logger)
        {
            appSettings = app;
            _security = new SecurityController();
            _logger = logger;
        }

        [HttpPost]
        [Route("GetMMCUserByEmailAndPassword")]
        public ActionResult<MMCLoginModel> GetMMCUserByEmailAndPassword([FromBody] MMCLoginModel model)
        {
            var msg = new Message<MMCUserModel>();
            var result = DbClientFactory<UserMMCDbClient>.Instance.GetMMCUserByEmailAndPassword(model.EmailId, _security.Encrypt(model.Password), appSettings.Value.DbConn);
            if (result.Count > 0)
            {
                msg.success = true;
                msg.message = "Success.";
                msg.companyId = result[0].companyId;
                msg.id = result[0].id;
                msg.firstName = result[0].firstName;
                msg.lastName = result[0].lastName;
                msg.user_type = result[0].role;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            else
            {
                msg.success = false;
                msg.message = "Failed.";
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            return Ok(msg);
        }

        [HttpGet]
        [Route("GetAllMMCUser")]
        public IActionResult GetAllMMCUser()
        {
            var result = DbClientFactory<UserMMCDbClient>.Instance.GetAllMMCUser(appSettings.Value.DbConn);
            return Ok(result);
        }

        [HttpPost]
        [Route("CreateUser")]
        public ActionResult<MMCUserModel> CreateUser([FromBody]MMCUserModel model)
        {
            model.password = _security.Encrypt(model.password);

            var msg = new Message<MMCUserModel>();
            var result = DbClientFactory<UserMMCDbClient>.Instance.CreateMMCUser(model, appSettings.Value.DbConn);
            if (result == "1")
            {
                msg.success = true;
                msg.message = "User created successfully.";
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            else
            {
                msg.success = false;
                msg.message = "Failed to create user.";
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }
            return Ok(msg);
        }

        [HttpGet]
        [Route("GetAllCompany")]
        public IActionResult GetAllCompany()
        {
            var result = DbClientFactory<UserMMCDbClient>.Instance.GetAllCompany(appSettings.Value.DbConn);
            return Ok(result);
        }

        #region User Get By Id

        [HttpGet]
        [Route("GetMMCUserById")]
        public IActionResult GetUserByUserId(int Id)
        {
            var data = DbClientFactory<UserMMCDbClient>.Instance.GetUserByUserId(appSettings.Value.DbConn, Id);
            return Ok(data);
        }

        #endregion
    }
}