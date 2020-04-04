using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.Configurations;
using MMCApi.Model;
using MMCApi.NotificationHubs;
using MMCApi.Repository;
using MMCApi.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Notification = MMCApi.NotificationHubs.Notification;

namespace MMCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : Controller
    {
        #region Variable

        BlobStorageService objBlob = new BlobStorageService();
        private readonly IOptions<MyConfig> config;
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILogger<BlobController> _logger;
        private NotificationHubProxy _notificationHubProxy;
        public EmailUtility objEmail = new EmailUtility();
        private readonly IOptions<EmailSettingModel> _mailSettings;
        private IHostingEnvironment _env;
        #endregion

        public BlobController(IOptions<MyConfig> config, IOptions<MySettingsModel> app, ILogger<BlobController> logger, IOptions<NotificationHubConfiguration> standardNotificationHubConfiguration, IOptions<EmailSettingModel> mailSettings, IHostingEnvironment env)
        {
            this.config = config;
            appSettings = app;
            _logger = logger;
            _mailSettings = mailSettings;
            _notificationHubProxy = new NotificationHubProxy(standardNotificationHubConfiguration.Value);
            _env = env;
        }

        #region Upload Image -- Step 1

        [HttpPost, DisableRequestSizeLimit]
        [Route("UploadImage")]
        public async Task<ActionResult> UploadImageAsync(UserFileUploadModel model, int CompanyId, string receiptType)
        {
            CategoryModel categry = new CategoryModel();
            var msg = new AI_Message<AI_ReceiptModel>();
            List<AI_ReceiptModel> AI_model = new List<AI_ReceiptModel>();
            string returnUrl = "";
            try
            {
                AI_model.Clear();

                var categories = GetCategories(CompanyId);
                if (((Microsoft.AspNetCore.Mvc.ObjectResult)categories.Result).StatusCode == 200)
                {
                    var json = categories.Result;
                    var val = ((Microsoft.AspNetCore.Mvc.ObjectResult)json).Value;
                    JObject joResponse = JObject.Parse(val.ToString());
                    JArray ojJarray = (JArray)joResponse["response"]["result"]["categories"];
                    string firstCategory = ojJarray[0].ToString();
                   
                    categry = JsonConvert.DeserializeObject<CategoryModel>(firstCategory);
                }
                else
                {
                    categry.categoryid = 0; // if category not found 
                }

                foreach (var formFile in model.File)
                {
                    if (model.File.Count > 0)
                    {
                        using (var imageStream = formFile.OpenReadStream())
                        {
                            byte[] contents = new byte[formFile.Length];

                            for (int i = 0; i < formFile.Length; i++)
                            {
                                contents[i] = (byte)imageStream.ReadByte();
                            }
                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Url Fetching Started.");

                            returnUrl = objBlob.UploadFileToBlob(formFile.FileName, contents, formFile.ContentType, config.Value.StorageConnection);

                            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Url Fetching End.");

                            if (returnUrl != "" || returnUrl != null)
                            {
                                var result = await MakeRequest_New(returnUrl);
                                bool isDate = IsDate(result.InvoiceDate);

                                DateTime date = DateTime.Today;
                                if (result.ResponseMessage == "OK")
                                {
                                    if (isDate)
                                    {
                                        try
                                        {
                                            CultureInfo culture = new CultureInfo("en-US");
                                            if (result.InvoiceDate != null)
                                            {
                                                date = DateTime.Parse(result.InvoiceDate, culture);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            CultureInfo culture = new CultureInfo("en-GB");
                                            if (result.InvoiceDate != null)
                                            {
                                                date = DateTime.Parse(result.InvoiceDate, culture);
                                            }
                                            //  throw ex;
                                        }
                                    }
                                    else
                                    {
                                        date = DateTime.Today;
                                    }

                                    if (date == null)
                                    {
                                        result.InvoiceDate = Convert.ToString(DateTime.Today);
                                    }
                                    if (result.VendorName == "" || result.VendorName == null)
                                    {
                                        result.VendorName = "Unknown Vendor";
                                    }
                                    AI_model.Add(new AI_ReceiptModel
                                    {
                                        VendorName = result.VendorName,
                                        VendorAddress = result.VendorAddress,
                                        BillDate = Convert.ToDateTime(date),
                                        PhoneNumber = result.PhoneNumber,
                                        TaxAmount = Convert.ToDecimal(result.TaxAmount),
                                        TotalAmount = Convert.ToDecimal(result.TotalAmount),
                                        TransactionTime = result.TransactionTime,
                                        Subtotal = result.Subtotal,
                                        Url = returnUrl,
                                        Description = result.Description,
                                        CompanyId = CompanyId,
                                        CreatedBy = model.CreatedBy,
                                        AssignTo = model.AssignTo,
                                        ImageContent = result.ImageContent,
                                        EmailId = model.UserId,
                                        CategoryId = categry.categoryid.ToString(),
                                        Rate = 30
                                    });

                                    msg.success = true;
                                    msg.receiptModel = AI_model;
                                }
                                else
                                {
                                    msg.success = false;
                                    msg.message = result.ResponseMessage;
                                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                msg.success = false;
                msg.message = "Upload Failed" + ex.Message;
                _logger.LogError("Error Occurred in " + MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
            }

            return Ok(msg);
        }
        #endregion

        public static bool IsDate(string tempDate)
        {
            DateTime fromDateValue;
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(tempDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Make Cognitive Service Call For Analyze Receipt -- Step 2
        public async Task<AI_ReceiptModel> MakeRequest(string file)
        {
            var client = new HttpClient();
            CognitiveServiceRequest model = new CognitiveServiceRequest { url = file };

            string imgUrl = JsonConvert.SerializeObject(model);
            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.Value.SubscriptionKey);

            HttpResponseMessage response;

            byte[] byteData = Encoding.UTF8.GetBytes(imgUrl);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Posting receipt on cognitive service.");

                response = await client.PostAsync(appSettings.Value.Url, content);
                _logger.LogError(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
            }
            if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.Accepted)
            {
                var header = (response.Headers).GetValues("Operation-Location").FirstOrDefault();
                Thread.Sleep(4000);
                var result = await GetRequest(header);
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase);
                return result;
            }
            else
            {
                AI_ReceiptModel modelRes = new AI_ReceiptModel();
                modelRes.ResponseMessage = response.ReasonPhrase.ToString();
                _logger.LogError(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return modelRes;
            }
        }

        #endregion

        #region Make Cognitive Service Call For Get Receipt Result -- Step 3
        public async Task<AI_ReceiptModel> GetRequest(string url)
        {
            var client = new HttpClient();
            AI_ReceiptModel model = new AI_ReceiptModel();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.Value.SubscriptionKey);

            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Get receipt result from cognitive service.");

            HttpResponseMessage response = await client.GetAsync(url);

            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase);

            if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                cognitiveResponse responseAI = JsonConvert.DeserializeObject<cognitiveResponse>(data);

                var json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                JObject joResponse = JObject.Parse(json.ToString());
                JArray ojObject = (JArray)joResponse["understandingResults"];
                if (ojObject != null)
                {
                    foreach (JObject item in ojObject)
                    {
                        string fields = item.GetValue("fields").ToString();
                        var value = item["fields"];
                        if (value["MerchantName"].FirstOrDefault() != null)
                        {
                            if (value["MerchantName"]["text"] != null || value["MerchantName"]["value"] != null)
                            {
                                if (value["MerchantName"]["text"] != null)
                                {
                                    model.VendorName = value["MerchantName"]["text"].ToString();
                                }
                                else if (value["MerchantName"]["value"] != null)
                                {
                                    model.VendorName = value["MerchantName"]["value"].ToString();
                                }
                            }
                        }
                        if (value["MerchantAddress"].FirstOrDefault() != null)
                        {
                            if (value["MerchantAddress"]["text"] != null || value["MerchantAddress"]["value"] != null)
                            {
                                if (value["MerchantAddress"]["text"] != null)
                                {
                                    model.VendorAddress = value["MerchantAddress"]["text"].ToString();
                                }
                                else if (value["MerchantAddress"]["value"] != null)
                                {
                                    model.VendorAddress = value["MerchantAddress"]["value"].ToString();
                                }
                            }
                        }

                        if (value["TransactionDate"].FirstOrDefault() != null)
                        {
                            if (value["TransactionDate"]["text"] != null || value["TransactionDate"]["value"] != null)
                            {
                                if (value["TransactionDate"]["text"] != null)
                                {
                                    model.InvoiceDate = value["TransactionDate"]["text"].ToString();
                                }
                                else if (value["TransactionDate"]["value"] != null)
                                {
                                    model.InvoiceDate = value["TransactionDate"]["value"].ToString();
                                }
                            }
                        }
                        if (value["MerchantPhoneNumber"].FirstOrDefault() != null)
                        {
                            if (value["MerchantPhoneNumber"]["text"] != null || value["MerchantPhoneNumber"]["value"] != null)
                            {
                                if (value["MerchantPhoneNumber"]["text"] != null)
                                {
                                    model.PhoneNumber = value["MerchantPhoneNumber"]["text"].ToString();
                                }
                                else if (value["MerchantPhoneNumber"]["value"] != null)
                                {
                                    model.PhoneNumber = value["MerchantPhoneNumber"]["value"].ToString();
                                }
                            }
                        }
                        if (value["TransactionTime"].FirstOrDefault() != null)
                        {
                            if (value["TransactionTime"]["text"] != null || value["TransactionTime"]["value"] != null)
                            {
                                if (value["TransactionTime"]["text"] != null)
                                {
                                    model.TransactionTime = value["TransactionTime"]["text"].ToString();
                                }
                                else if (value["TransactionTime"]["value"] != null)
                                {
                                    model.TransactionTime = value["TransactionTime"]["value"].ToString();
                                }
                            }
                        }
                        if (value["Subtotal"].FirstOrDefault() != null)
                        {
                            if (value["Subtotal"]["text"] != null || value["Subtotal"]["value"] != null)
                            {
                                if (value["Subtotal"]["value"] != null && value["Subtotal"]["value"].ToString() != "")
                                {
                                    model.Subtotal = Convert.ToDecimal(value["Subtotal"]["value"].ToString());
                                }
                                else
                                {
                                    model.Subtotal = Convert.ToDecimal(value["Subtotal"]["text"].ToString());
                                }
                            }
                        }
                        if (value["Tax"].FirstOrDefault() != null)
                        {
                            if (value["Tax"]["text"] != null || value["Tax"]["value"] != null)
                            {
                                if (value["Tax"]["value"] != null && value["Tax"]["value"].ToString() != "")
                                {
                                    model.TaxAmount = Convert.ToDecimal(value["Tax"]["value"].ToString());
                                }
                                else
                                {
                                    model.TaxAmount = Convert.ToDecimal(value["Tax"]["text"].ToString());
                                }
                            }
                        }
                        if (value["Total"].FirstOrDefault() != null)
                        {
                            if (value["Total"]["text"] != null || value["Total"]["value"] != null)
                            {
                                if (value["Total"]["value"] != null && value["Total"]["value"].ToString() != "")
                                {
                                    model.TotalAmount = Convert.ToDecimal(value["Total"]["value"].ToString());
                                }
                                else
                                {
                                    model.TotalAmount = Convert.ToDecimal(value["Total"]["text"].ToString());
                                }
                            }
                        }
                        model.ImageContent = fields.ToString();
                        //model = new AI_ReceiptModel()
                        //{
                        //    VendorName = value["MerchantName"].ToString() != "" ? value["MerchantName"]["text"].ToString() : null,
                        //    VendorAddress = value["MerchantAddress"].ToString() != "" ? value["MerchantAddress"]["text"].ToString() : null,
                        //    PhoneNumber = value["MerchantPhoneNumber"].ToString() != "" ? value["MerchantPhoneNumber"]["text"].ToString() : null,
                        //    InvoiceDate = value["TransactionDate"].ToString() != "" ? value["TransactionDate"]["text"].ToString() : null,
                        //    TransactionTime = value["TransactionTime"].ToString() != "" ? value["TransactionTime"]["text"].ToString() : null,
                        //    Subtotal = Convert.ToDecimal(value["Subtotal"].ToString() != "" ? Convert.ToDecimal(value["Subtotal"]["text"]).ToString() : null),
                        //    TaxAmount = Convert.ToDecimal(value["Tax"].ToString() != "" ? Convert.ToDecimal(value["Tax"]["text"]).ToString() : null),
                        //    TotalAmount = Convert.ToDecimal(value["Total"].ToString() != "" ? Convert.ToDecimal(value["Total"]["text"]).ToString() : null),
                        //    ImageContent = fields.ToString()
                        //};

                        //string[] validformats = new[] { "MM/dd/yyyy", "yyyy/MM/dd", "dd/MM/yyyy", "dd/MM/yy", "M/dd/yyyy", "M/d/yyyy", "M/d/yy", "MM/d/yyyy" };
                        //if (result.InvoiceDate.ToString() != "")
                        //{
                        //    date = DateTime.ParseExact(result.InvoiceDate, validformats, null);
                        //}
                        //else
                        //{
                        //    date = DateTime.Today;
                        //}
                    }
                }
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                model.ResponseMessage = response.ReasonPhrase.ToString();
                return model;
            }
            else
            {
                AI_ReceiptModel modelRes = new AI_ReceiptModel();
                modelRes.ResponseMessage = response.ReasonPhrase.ToString();
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase);
                return modelRes;
            }
        }

        #endregion


        #region Make Cognitive Service Call For Analyze Receipt -- Step 2
        public async Task<AI_ReceiptModel> MakeRequest_New(string file)
        {
            var client = new HttpClient();
            CognitiveServiceRequest model = new CognitiveServiceRequest { url = file };

            string imgUrl = JsonConvert.SerializeObject(model);
            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.Value.SubscriptionKey);

            HttpResponseMessage response;

            byte[] byteData = Encoding.UTF8.GetBytes(imgUrl);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Posting receipt on cognitive service.");

                response = await client.PostAsync(appSettings.Value.Url, content);
                _logger.LogError(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
            }
            if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.Accepted)
            {
                var header = (response.Headers).GetValues("Operation-Location").FirstOrDefault();
                Thread.Sleep(4000);
                var result = await GetRequest_New(header);
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase);
                return result;
            }
            else
            {
                AI_ReceiptModel modelRes = new AI_ReceiptModel();
                modelRes.ResponseMessage = response.ReasonPhrase.ToString();
                _logger.LogError(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return modelRes;
            }
        }

        #endregion

        #region Make Cognitive Service Call For Get Receipt Result -- Step 3
        public async Task<AI_ReceiptModel> GetRequest_New(string url)
        {
            var client = new HttpClient();
            AI_ReceiptModel model = new AI_ReceiptModel();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.Value.SubscriptionKey);

            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Get receipt result from cognitive service.");

            HttpResponseMessage response = await client.GetAsync(url);

            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase);

            if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsStringAsync();
                cognitiveResponse responseAI = JsonConvert.DeserializeObject<cognitiveResponse>(data);

                var json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                JObject joResponse = JObject.Parse(json.ToString());
                JObject objAnalyzeResult = (JObject)joResponse["analyzeResult"];
                JArray ojObject = (JArray)objAnalyzeResult["documentResults"];
                if (ojObject != null)
                {
                    foreach (JObject item in ojObject)
                    {
                        string fields = item.GetValue("fields").ToString();
                        var value = item["fields"];
                        if (value["MerchantName"] != null)
                        {
                            if (value["MerchantName"].FirstOrDefault() != null)
                            {
                                if (value["MerchantName"]["text"] != null || value["MerchantName"]["valueString"] != null)
                                {
                                    if (value["MerchantName"]["text"] != null)
                                    {
                                        model.VendorName = value["MerchantName"]["text"].ToString();
                                    }
                                    else if (value["MerchantName"]["valueString"] != null)
                                    {
                                        model.VendorName = value["MerchantName"]["valueString"].ToString();
                                    }
                                }
                            }
                        }
                        if (value["MerchantAddress"] != null)
                        {
                            if (value["MerchantAddress"].FirstOrDefault() != null)
                            {
                                if (value["MerchantAddress"]["text"] != null || value["MerchantAddress"]["valueString"] != null)
                                {
                                    if (value["MerchantAddress"]["text"] != null)
                                    {
                                        model.VendorAddress = value["MerchantAddress"]["text"].ToString();
                                    }
                                    else if (value["MerchantAddress"]["valueString"] != null)
                                    {
                                        model.VendorAddress = value["MerchantAddress"]["valueString"].ToString();
                                    }
                                }
                            }
                        }
                        if (value["TransactionDate"] != null)
                        {
                            if (value["TransactionDate"].FirstOrDefault() != null)
                            {
                                if (value["TransactionDate"]["text"] != null || value["TransactionDate"]["valueString"] != null)
                                {
                                    if (value["TransactionDate"]["text"] != null)
                                    {
                                        model.InvoiceDate = value["TransactionDate"]["text"].ToString();
                                    }
                                    else if (value["TransactionDate"]["valueString"] != null)
                                    {
                                        model.InvoiceDate = value["TransactionDate"]["valueString"].ToString();
                                    }
                                }
                            }
                        }
                        if (value["MerchantPhoneNumber"] != null)
                        {
                            if (value["MerchantPhoneNumber"].FirstOrDefault() != null)
                            {
                                if (value["MerchantPhoneNumber"]["text"] != null || value["MerchantPhoneNumber"]["valueString"] != null)
                                {
                                    if (value["MerchantPhoneNumber"]["text"] != null)
                                    {
                                        model.PhoneNumber = value["MerchantPhoneNumber"]["text"].ToString();
                                    }
                                    else if (value["MerchantPhoneNumber"]["valueString"] != null)
                                    {
                                        model.PhoneNumber = value["MerchantPhoneNumber"]["valueString"].ToString();
                                    }
                                }
                            }
                        }
                        if (value["TransactionTime"] != null)
                        {
                            if (value["TransactionTime"].FirstOrDefault() != null)
                            {
                                if (value["TransactionTime"]["text"] != null || value["TransactionTime"]["valueString"] != null)
                                {
                                    if (value["TransactionTime"]["text"] != null)
                                    {
                                        model.TransactionTime = value["TransactionTime"]["text"].ToString();
                                    }
                                    else if (value["TransactionTime"]["valueString"] != null)
                                    {
                                        model.TransactionTime = value["TransactionTime"]["valueString"].ToString();
                                    }
                                }
                            }
                        }
                        if (value["Subtotal"] != null)
                        {
                            if (value["Subtotal"].FirstOrDefault() != null)
                            {
                                if (value["Subtotal"]["text"] != null || value["Subtotal"]["valueNumber"] != null)
                                {
                                    if (value["Subtotal"]["valueNumber"] != null && value["Subtotal"]["valueNumber"].ToString() != "")
                                    {
                                        model.Subtotal = Convert.ToDecimal(value["Subtotal"]["valueNumber"].ToString());
                                    }
                                    else
                                    {
                                        model.Subtotal = Convert.ToDecimal(value["Subtotal"]["text"].ToString());
                                    }
                                }
                            }
                        }
                        if (value["Tax"] != null)
                        {
                            if (value["Tax"].FirstOrDefault() != null)
                            {
                                if (value["Tax"]["text"] != null || value["Tax"]["valueNumber"] != null)
                                {
                                    if (value["Tax"]["valueNumber"] != null && value["Tax"]["valueNumber"].ToString() != "")
                                    {
                                        model.TaxAmount = Convert.ToDecimal(value["Tax"]["valueNumber"].ToString());
                                    }
                                    else
                                    {
                                        model.TaxAmount = Convert.ToDecimal(value["Tax"]["text"].ToString());
                                    }
                                }
                            }
                        }
                        if (value["Total"] != null)
                        {
                            if (value["Total"].FirstOrDefault() != null)
                            {
                                if (value["Total"]["text"] != null || value["Total"]["valueNumber"] != null)
                                {
                                    if (value["Total"]["valueNumber"] != null && value["Total"]["valueNumber"].ToString() != "")
                                    {
                                        model.TotalAmount = Convert.ToDecimal(value["Total"]["valueNumber"].ToString());
                                    }
                                    else
                                    {
                                        model.TotalAmount = Convert.ToDecimal(value["Total"]["text"].ToString());
                                    }
                                }
                            }
                        }
                        StringBuilder sb = new StringBuilder();
                        JObject objItems = (JObject)value["Items"];
                        JArray ojValueItems = (JArray)objItems["valueArray"];
                        foreach (JObject items in ojValueItems)
                        {
                            string strName = "";
                            string strQty = "";
                            string strTotalPrice = "";
                            if (items["valueObject"]["Name"] != null)
                            {
                                strName = items["valueObject"]["Name"]["valueString"].ToString();
                                strName = "Name : " + strName + ",";
                            }
                            if (items["valueObject"]["TotalPrice"] != null)
                            {
                                strTotalPrice = items["valueObject"]["TotalPrice"]["text"].ToString();
                                strTotalPrice = "Total Price : " + strTotalPrice + ",";
                            }
                            if (items["valueObject"]["Quantity"] != null)
                            {
                                strQty = items["valueObject"]["Quantity"]["text"].ToString();
                                strQty = "Qty : " + strQty + "";
                            }
                            sb.AppendLine(strName + ' ' + strTotalPrice + ' ' + strQty);
                        }
                        model.Description = sb.ToString();
                        model.ImageContent = fields.ToString();
                    }
                }
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                model.ResponseMessage = response.ReasonPhrase.ToString();
                return model;
            }
            else
            {
                AI_ReceiptModel modelRes = new AI_ReceiptModel();
                modelRes.ResponseMessage = response.ReasonPhrase.ToString();
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase);
                return modelRes;
            }
        }

        #endregion

        #region Insert Receipt On Database -- Step 4
        public string InsertReceipt(AI_ReceiptModel model)
        {
            var msg = new AI_Message<AI_ReceiptModel>();
            var result = "";
            try
            {
                if (ModelState.IsValid)
                {
                    result = DbClientFactory<AI_ReceiptDbClient>.Instance.InsertReceipt(model, appSettings.Value.DbConn);
                    if (result == "1")
                    {
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Successfully inserted receipt");
                    }
                    else
                    {
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + " Problem in receipt insert.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        #endregion

        #region Get Receipts Methods

        [HttpGet]
        [Route("GetReceiptResult")]
        public IActionResult GetReceiptResult(string emailId)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptResult(appSettings.Value.DbConn, emailId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllReceiptByCompanyId")]
        public IActionResult GetAllReceiptByCompanyId(int CompanyId)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptByCompanyId(appSettings.Value.DbConn, CompanyId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllReceipt")]
        public IActionResult GetAllReceipt(string emailId)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetAllReceipt(appSettings.Value.DbConn, emailId);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetReceiptByAssignTo")]
        public IActionResult GetReceiptByAssignTo(string assignTo)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptByAssignTo(appSettings.Value.DbConn, assignTo);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetReceiptById")]
        public IActionResult GetReceiptById(string id)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetReceiptById(appSettings.Value.DbConn, id);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCategoryIdByVendor")]
        public IActionResult GetCategoryIdByVendor(string vendorName)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetCategoryIdByVendor(appSettings.Value.DbConn, vendorName);
            return Ok(data);
        }

        #endregion

        #region Update Receipt

        [HttpPost]
        [Route("UpdateReceipt")]
        public ActionResult<AI_ReceiptModel> UpdateReceipt([FromBody]AI_ReceiptModel model)
        {
            var msg = new AI_Message<AI_ReceiptModel>();
            if (ModelState.IsValid)
            {
                var result = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateReceipt(model, appSettings.Value.DbConn);

                if (result == "1")
                {
                    msg.success = true;
                    msg.message = "Receipt updated successfully";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                else if (result == "-1")
                {
                    msg.success = false;
                    msg.message = "Unable to update Receipt";
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
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + errorList[0]);
            }
            return Ok(msg);
        }

        #endregion

        #region Update Category Id By Vendor

        [HttpPost]
        [Route("UpdateCategoryIdByVendor")]
        public ActionResult<UpdateCategory> UpdateCategoryIdByVendor([FromBody]UpdateCategory model)
        {
            var msg = new AI_Message<AI_ReceiptModel>();
            if (ModelState.IsValid)
            {
                var result = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateCategoryIdByVendor(model, appSettings.Value.DbConn);

                if (result == "1")
                {
                    msg.success = true;
                    msg.message = "Category updated successfully";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                else if (result == "-1")
                {
                    msg.success = false;
                    msg.message = "Unable to update Category";
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
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + errorList[0]);
            }
            return Ok(msg);
        }

        #endregion

        #region Testing Code
        public async Task<IActionResult> SendNotification([FromBody] Notification newNotification)
        {
            string[] str = new string[1];
            str[0] = "deepak.gupta@beyondkey.com";
            //  str[1] = "nakul.paithankar@beyondkey.com";
            newNotification.Tags = str;
            newNotification.Platform = MobilePlatform.fcm;
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationHubProxy.SendNotification(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                return Ok();
            }

            return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
        }
        //[HttpPost, DisableRequestSizeLimit]
        //[Route("UploadImage_New")]
        //public async Task<IActionResult> UploadImageAsync_New(string url)
        //{
        //    var msg = new AI_Message<AI_ReceiptModel>();
        //    List<AI_ReceiptModel> AI_model = new List<AI_ReceiptModel>();
        //    try
        //    {
        //        var result = await MakeRequest_New(url);
        //       msg.message= result.ToString();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        msg.success = false;
        //        msg.message = "Upload Failed" + ex.Message;
        //        _logger.LogError("Error Occurred in " + MethodBase.GetCurrentMethod().Name + " " + msg.message);
        //    }

        //    return Ok(msg);
        //}


        //#region Make Cognitive Service Call For Analyze Receipt -- Step 2
        //public async Task<string> MakeRequest_New(string file)
        //{
        //    var client = new HttpClient();
        //    CognitiveServiceRequest model = new CognitiveServiceRequest { url = file };

        //    string imgUrl = JsonConvert.SerializeObject(model);
        //    // Request headers
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.Value.SubscriptionKey);

        //    HttpResponseMessage response;

        //    byte[] byteData = Encoding.UTF8.GetBytes(imgUrl);

        //    using (var content = new ByteArrayContent(byteData))
        //    {
        //        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //        _logger.LogInformation(MethodBase.GetCurrentMethod().Name, "Posting receipt on cognitive service.");

        //        response = await client.PostAsync(appSettings.Value.Url, content);

        //        _logger.LogInformation(MethodBase.GetCurrentMethod().Name, "Response by cognitive service.");
        //    }
        //    if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.Accepted)
        //    {
        //        var header = (response.Headers).GetValues("Operation-Location").FirstOrDefault();

        //        return header;
        //    }
        //    else
        //    {
        //        _logger.LogError(MethodBase.GetCurrentMethod().Name, "Bad Request.");
        //        return null;
        //    }
        //}

        //#endregion

        //#region Make Cognitive Service Call For Get Receipt Result -- Step 3

        //[HttpGet]
        //[Route("GetRequest_New")]
        //public async Task<AI_ReceiptModel> GetRequest_New(string url)
        //{
        //    var client = new HttpClient();
        //    AI_ReceiptModel model = new AI_ReceiptModel();

        //    // Request headers
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", appSettings.Value.SubscriptionKey);

        //    HttpResponseMessage response = await client.GetAsync(url);
        //    if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.OK)
        //    {
        //        var data = await response.Content.ReadAsStringAsync();
        //        var json = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
        //        JObject joResponse = JObject.Parse(json.ToString());
        //        JArray ojObject = (JArray)joResponse["understandingResults"];
        //        if (ojObject != null)
        //        {

        //            foreach (JObject item in ojObject)
        //            {
        //                string fields = item.GetValue("fields").ToString();
        //                var value = item["fields"];

        //                model = new AI_ReceiptModel()
        //                {
        //                    VendorName = value["MerchantName"].ToString() != "" ? value["MerchantName"]["text"].ToString() : null,
        //                    TotalAmount = Convert.ToDecimal(value["Total"].ToString() != "" ? Convert.ToDecimal(value["Total"]["value"]).ToString() : null),
        //                    VendorAddress = value["MerchantAddress"].ToString() != "" ? value["MerchantAddress"]["text"].ToString() : null,
        //                    PhoneNumber = value["MerchantPhoneNumber"].ToString() != "" ? value["MerchantPhoneNumber"]["text"].ToString() : null,
        //                    InvoiceDate = value["TransactionDate"].ToString() != "" ? value["TransactionDate"]["text"].ToString() : null,
        //                    TransactionTime = value["TransactionTime"].ToString() != "" ? value["TransactionTime"]["text"].ToString() : null,
        //                    Subtotal = Convert.ToDecimal(value["Subtotal"].ToString() != "" ? Convert.ToDecimal(value["Subtotal"]["value"]).ToString() : null),
        //                    TaxAmount = Convert.ToDecimal(value["Tax"].ToString() != "" ? Convert.ToDecimal(value["Tax"]["value"]).ToString() : null),
        //                    ImageContent = fields.ToString()
        //                };
        //            }


        //        }
        //        _logger.LogInformation(MethodBase.GetCurrentMethod().Name, "Success.");
        //        return model;
        //    }
        //    else
        //    {
        //        _logger.LogError(MethodBase.GetCurrentMethod().Name, "Bad Request.");
        //        return null;
        //    }
        //}

        //#endregion

        //#region Insert Receipt On Database -- Step 4
        //public string InsertReceipt_New(AI_ReceiptModel model)
        //{
        //    var msg = new AI_Message<AI_ReceiptModel>();
        //    var result = "";
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            result = DbClientFactory<AI_ReceiptDbClient>.Instance.InsertReceipt(model, appSettings.Value.DbConn);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = ex.Message;
        //    }

        //    return result;
        //}

        //#endregion
        #endregion

        #region Insert Receipt

        [HttpPost]
        [Route("InsertNewReceipt")]
        public ActionResult<AI_ReceiptInsertModel> InsertNewReceipt([FromBody]List<AI_ReceiptModel> insertModel)
        {
            var msg = new CommonMessgae();

            for (int i = 0; i < insertModel.Count; i++)
            {
                List<AI_ReceiptModel> AI_model = new List<AI_ReceiptModel>();

                string strCurrency = GetDefaultCurrency(insertModel[i].CompanyId);
                if (strCurrency != "")
                {
                    insertModel[i].Currency = strCurrency;
                }
                else
                {
                    insertModel[i].Currency = "INR";
                }

                var res = InsertReceipt(insertModel[i]);
                if (res == "1" || res == "2")
                {
                    msg.success = true;
                    msg.message = "Image uploaded successfully";
                    string fullName = "";
                    var data = DbClientFactory<UseDbClient>.Instance.GetUserByEmailId(appSettings.Value.DbConn, insertModel[i].CreatedBy);
                    if (data.Count > 0)
                    {
                        fullName = data[0].FirstName + ' ' + data[0].LastName;
                    }
                    else
                    {
                        fullName = "Unknown user";
                    }
                    WebClient client = new WebClient();
                    string fileString = client.DownloadString(new Uri("https://mmcbotstorage.blob.core.windows.net/mmcbotcontainer/UploadReceiptTemplate.html"));
                    string body = string.Empty;
                    fileString = fileString.Replace("{UserName}", "Administrator");
                    fileString = fileString.Replace("{VendorName}", insertModel[i].VendorName);
                    body = fileString;

                    bool isEmailSend = objEmail.SendUploadReceiptEmail(_mailSettings.Value.MailFrom, "donotreplymmcbot@gmail.com", _mailSettings.Value.Password, insertModel[i].VendorName, fullName, body);

                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }

            }
            return Ok(msg);
        }

        public string GetDefaultCurrency(int CompanyId)
        {
            int businessId = GetBusinessId(CompanyId);
            // Get Access Token By Company ID
            var count = DbClientFactory<FreshbookDbClient>.Instance.GetFreshbookDetailByCompanyId(appSettings.Value.DbConn, CompanyId.ToString());
            string access_token = count[0].AccessToken;
            // string userAccountId = GetUserDetail(access_token);
            //  string userAccountId = account_id;
            string userAccountId = count[0].AccountId;
            HttpClient http = new HttpClient();

            http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", access_token);

            var response = http.GetAsync("https://api.freshbooks.com/accounting/account/" + userAccountId + "/systems/systems/" + businessId).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                var json = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                JObject joResponse = JObject.Parse(json.ToString());
                string currency = joResponse["response"]["result"]["system"]["currency_code"].ToString();

                return currency;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                return "";
            }
            return "";
        }

        private int GetBusinessId(int CompanyId)
        {
            int business_id = 0;
            FreshbookBusinessMemberModel model = new FreshbookBusinessMemberModel();
            var count = DbClientFactory<FreshbookDbClient>.Instance.GetFreshbookDetailByCompanyId(appSettings.Value.DbConn, CompanyId.ToString());
            bool isSelected = false;
            isSelected = count[0].IsSelected;
            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + "Get Freshbook detail by company id.");
            if (count.Count > 0)
            {
                string access_token = count[0].AccessToken;
                HttpClient http = new HttpClient();

                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", access_token);

                var response = http.GetAsync("https://api.freshbooks.com/auth/api/v1/users/me").Result;
                if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                    JObject joResponse = JObject.Parse(json.ToString());
                    JArray ojJarray = (JArray)joResponse["response"]["business_memberships"];
                    for (int i = 0; i < ojJarray.Count; i++)
                    {
                        JObject obj = JObject.Parse(ojJarray[i]["business"].ToString());
                        string jsonObj = obj.ToString();
                        model.business = JsonConvert.DeserializeObject<business>(jsonObj);
                        if (model.business.account_id == count[0].AccountId)
                        {
                            business_id = Convert.ToInt32(ojJarray[i]["id"]);
                        }
                    }
                }
            }
            return business_id;
        }

        #endregion

        #region Search Receipt

        [HttpGet]
        [Route("SearchReceiptData")]
        public IActionResult SearchReceiptData(string statusName, string companyName, DateTime? frmDate, DateTime? toDate)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.SearchReceiptData(appSettings.Value.DbConn, statusName, companyName, frmDate, toDate);
            return Ok(data);
        }

        #endregion


        #region Update Receipt From Web

        [HttpPost]
        [Route("UpdateReceiptFromWeb")]
        public ActionResult<AI_ReceiptModel> UpdateReceiptFromWeb([FromBody]AI_ReceiptModel model)
        {
            var msg = new AI_Message<AI_ReceiptModel>();
            if (ModelState.IsValid)
            {
                var result = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateReceiptFromWeb(model, appSettings.Value.DbConn);

                if (result == "1")
                {
                    msg.success = true;
                    msg.message = "Receipt updated successfully";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                else if (result == "-1")
                {
                    msg.success = false;
                    msg.message = "Unable to update Receipt";
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
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + errorList[0]);
            }
            return Ok(msg);
        }

        #endregion

        #region Get All Receipts By Status

        [HttpGet]
        [Route("GetAllReceiptByCompanyIdAndStatus")]
        public IActionResult GetAllReceiptByCompanyIdAndStatus(int companyId, string status, int pageNo, bool isArchived)
        {
            var data = DbClientFactory<AI_ReceiptDbClient>.Instance.GetAllReceiptByCompanyIdAndStatus(appSettings.Value.DbConn, companyId, status, pageNo, isArchived);
            for (int i = 0; i < data.Count; i++)
            {
                // Get FreshBooks Category by id
                string category = GetCategoriesById(Convert.ToInt32(data[i].CategoryId), companyId);
                data[i].Category = category;
            }
            return Ok(data);
        }

        private string GetCategoriesById(int categoryId, int compnayId)
        {
            string category = "";
            try
            {
                var count = DbClientFactory<FreshbookDbClient>.Instance.GetFreshbookDetailByCompanyId(appSettings.Value.DbConn, compnayId.ToString());
                bool isSelected = false;
                isSelected = count[0].IsSelected;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + "Get Freshbook detail by company id.");
                if (count.Count > 0)
                {
                    string access_token = count[0].AccessToken;
                    string userAccountId = count[0].AccountId;
                    HttpClient http = new HttpClient();

                    http.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", access_token);

                    var response = http.GetAsync("https://api.freshbooks.com/accounting/account/" + userAccountId + "/expenses/categories/" + categoryId).Result;
                    if (response.IsSuccessStatusCode == true && response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        var json = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
                        JObject joResponse = JObject.Parse(json.ToString());
                        category = joResponse["response"]["result"]["category"]["category"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return category;
        }

        #endregion

        #region Get Categories

        public ActionResult<FreshbookModel> GetCategories(int CompanyId)
        {
            // Get Access Token By Company ID
            var count = DbClientFactory<FreshbookDbClient>.Instance.GetFreshbookDetailByCompanyId(appSettings.Value.DbConn, CompanyId.ToString());
            string access_token = count[0].AccessToken;
            // string userAccountId = GetUserDetail(access_token);
            //  string userAccountId = account_id;
            string userAccountId = count[0].AccountId;
            HttpClient http = new HttpClient();

            http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", access_token);

            var response = http.GetAsync("https://api.freshbooks.com/accounting/account/" + userAccountId + "/expenses/categories").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                return Ok(result);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                return NotFound();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            return BadRequest();
        }

        #endregion

    }
}