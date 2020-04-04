using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.Repository;
using MMCApi.Utility;

namespace MMCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MMCReceiptController : ControllerBase
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private SecurityController _security;
        private readonly ILogger<MMCReceiptController> _logger;

        public MMCReceiptController(IOptions<MySettingsModel> app, ILogger<MMCReceiptController> logger)
        {
            appSettings = app;
            _security = new SecurityController();
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllReceipt")]
        public IActionResult GetAllReceipt()
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetAllMMCReceipt(appSettings.Value.DbConn);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllCountries")]
        public IActionResult GetAllCountries()
        {
            var data = DbClientFactory<UseDbClient>.Instance.GetAllCountries(appSettings.Value.DbConn);
            return Ok(data);
        }
    }
}