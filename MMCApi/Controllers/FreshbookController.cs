using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MMCApi.Model;
using MMCApi.Model.Freshbook.Expense;
using MMCApi.Repository;
using MMCApi.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace MMCApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreshbookController : ControllerBase
    {
        private readonly IOptions<MySettingsModel> appSettings;
        private readonly ILogger<FreshbookController> _logger;

        public FreshbookController(IOptions<MySettingsModel> app, ILogger<FreshbookController> logger)
        {
            appSettings = app;
            _logger = logger;
        }

        #region Get Categories

        [HttpGet]
        [Route("GetCategories")]
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
            if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized();
            }
            return BadRequest();
        }

        #endregion

        #region Get Default Currency

        [HttpGet]
        [Route("GetDefaultCurrency")]
        public ActionResult<string> GetDefaultCurrency(int CompanyId)
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

                return Ok(currency);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                return NotFound();
            }
            return BadRequest();
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

        #region Get Bussiness Memebers

        [HttpGet]
        [Route("GetBusinessMemebers")]
        public List<business> GetBusinessMemebers(int CompanyId)
        {
            List<business> listBusiness = new List<business>();
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
                            model.business.isSelected = isSelected;
                        }
                        listBusiness.Add(model.business);
                    }
                }
            }
            return listBusiness;
        }
        #endregion

        #region Save Freshbooks Details

        [HttpPost]
        [Route("SaveFreshbookDetails")]
        public ActionResult<FreshbookModel> SaveFreshbookDetails([FromBody] FreshbookModel model)
        {
            var msg = new FreshbookMessgae<FreshbookModel>();

            if (ModelState.IsValid)
            {
                var result = DbClientFactory<FreshbookDbClient>.Instance.SaveFreshbookDetails(model, appSettings.Value.DbConn);
                if (result == "1")
                {
                    msg.success = true;
                    msg.message = "FreshBooks details saved successfully";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                else if (result == "2")
                {
                    msg.success = true;
                    msg.message = "FreshBooks details updated successfully";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                else if (result == "-1")
                {
                    msg.success = false;
                    msg.message = "Problem in save FreshBooks details";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            return Ok(msg);
        }

        #endregion

        #region Create Expense

        [HttpPost]
        [Route("CreateExpense")]
        public ActionResult<FreshbookModel> CreateExpense([FromBody] ExpensePost model, int CompanyId, string url, bool isBillable, int docId)
        {
            string file_name = "";
            ImageResponse freshbookImgRes = null;

            var count = DbClientFactory<FreshbookDbClient>.Instance.GetFreshbookDetailByCompanyId(appSettings.Value.DbConn, CompanyId.ToString());
            _logger.LogInformation(MethodBase.GetCurrentMethod().Name + "Get Freshbook detail by company id.");

            if (count.Count > 0)
            {
                string access_token = count[0].AccessToken;
                //  string userAccountId = GetUserDetail(access_token);
                string userAccountId = count[0].AccountId;

                #region Upload Receipt On Freshbook and get jwt token response

                if (url != "" && url != null)
                {
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(url);
                    string[] strSplit = url.Split('/');
                    for (int i = 0; i < strSplit.Length; i++)
                    {
                        if (strSplit[i].Contains(".jpeg") || strSplit[i].Contains(".jpg") || strSplit[i].Contains(".png"))
                        {
                            file_name = strSplit[i];
                        }
                    }
                    if (userAccountId != "UNAUTHORIZED")
                    {
                        string resultFreshbook = PostReceiptImageOnFreshbook(imageBytes, file_name, userAccountId, access_token);
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + resultFreshbook);
                        if (resultFreshbook != "Payment Required" || resultFreshbook != "Unprocessable Entity" || resultFreshbook != "Bad Request")
                        {
                            freshbookImgRes = JsonConvert.DeserializeObject<ImageResponse>(resultFreshbook);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                #endregion

                #region Demo Model
                //ExpensePost model1 = new ExpensePost();
                //model1.expense = new expense();
                //model1.expense.vendor = "test5";
                //model1.expense.notes = "test desc5";
                //model1.expense.category_name = "Bank Fees";
                //model1.expense.categoryid = "2767245";
                //model1.expense.staffid = 1;
                //model1.expense.date = "2019-12-20";
                //model1.expense.taxName1 = "other tax";
                //model1.expense.taxPercent1 = 13;
                //model1.expense.clientid = 95717;
                //model1.expense.include_receipt = false;
                //model1.expense.amount = new amounts
                //{
                //    amount = "98.73",
                //    code = "CAD"
                //};
                //model.expense.taxAmount1 = new taxAmount1
                //{
                //    amount = "0.46"
                //};
                //model1.expense.attachment = new attachment
                //{
                //    expenseid = null,
                //    jwt = freshbookImgRes.image.jwt,
                //    media_type = "image/*"

                //};
                #endregion
                model.expense.taxPercent1 = 30;
                model.expense.taxName1 = "Total Taxes";


                model.expense.include_receipt = true;
                model.expense.attachment = new attachment
                {
                    expenseid = null,
                    jwt = freshbookImgRes.image.jwt,
                    media_type = "image/*"
                };

                #region Setting Receipt Billable Or Not
                if (isBillable)
                {
                    var settingResponse = SettingForBillableReceipt(model, true, userAccountId, access_token);
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + settingResponse);
                }
                #endregion

                #region Expense Creation

                HttpClient http = new HttpClient();

                #region Dummy Model

                //ExpensePost model1 = new ExpensePost();
                //model1.expense = new expense();
                //model1.expense.vendor = "nakul";
                //model1.expense.notes = "test desc here";
                //model1.expense.category_name = "Bank Fees";
                //model1.expense.categoryid = "3411167";
                //model1.expense.staffid = 1;
                //model1.expense.date = "2019-12-20";
                //model1.expense.taxName1 = "other tax";
                //model1.expense.taxPercent1 = 13;
                //model1.expense.amount = new amounts
                //{
                //    amount = "98.73"
                //};
                //model1.expense.taxAmount1 = new taxAmount1
                //{
                //    amount = "9.71"
                //};
                //model1.expense.attachment = new attachment
                //{
                //    expenseid = null,
                //    jwt = freshbookImgRes.image.jwt,
                //    media_type = "image/*"

                //};
                #endregion

                var json = JsonConvert.SerializeObject(model);

                http.BaseAddress = new Uri("https://api.freshbooks.com/accounting/account/" + userAccountId + "/expenses/expenses");
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", access_token);
                http.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                request.Content = new StringContent(json,
                                                    Encoding.UTF8,
                                                    "application/json");//CONTENT-TYPE header


                var response = http.PostAsync(http.BaseAddress, request.Content).Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var resultUpdate = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateReceiptPushedToFreshbook(appSettings.Value.DbConn, docId);
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + "Pushed to freshbook.");
                    var updateId = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateReceiptCategoryIdClientId(appSettings.Value.DbConn, docId, model.expense.categoryid);
                    return Ok(result);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
                {
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                    return NotFound();
                }
                if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                    return UnprocessableEntity();
                }
                #endregion
            }
            return BadRequest();
        }

        public ActionResult<BillableItems> SettingForBillableReceipt(ExpensePost model, bool isBillable, string userAccountId, string access_token)
        {
            BillableItems billModel = new BillableItems();
            billModel.billable_item = new billable_item
            {
                name = model.expense.vendor,
                description = model.expense.notes,
                billable = isBillable,
                tax1 = null,
                tax2 = null
            };
            billModel.billable_item.unit_cost = new unit_cost
            {
                amount = model.expense.amount.amount,
                code = model.expense.amount.code
            };

            HttpClient http = new HttpClient();

            var json = JsonConvert.SerializeObject(billModel);

            http.BaseAddress = new Uri("https://api.freshbooks.com/accounting/account/" + userAccountId + "/billable_items/billable_items");
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", access_token);
            http.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
            request.Content = new StringContent(json,
                                                Encoding.UTF8,
                                                "application/json");


            var response = http.PostAsync(http.BaseAddress, request.Content).Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return Ok(result);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return NotFound();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return UnprocessableEntity();
            }
            return BadRequest();
        }

        public string PostReceiptImageOnFreshbook(byte[] content, string file_name, string userAccountId, string access_token)
        {
            var result = "";
            HttpClient http = new HttpClient();

            http.BaseAddress = new Uri("https://api.freshbooks.com/uploads/account/" + userAccountId + "/images");
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", access_token);
            http.DefaultRequestHeaders
                  .Accept
                  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            MultipartFormDataContent formContent = new MultipartFormDataContent();

            formContent.Add(new ByteArrayContent(content, 0, content.Length), "content", file_name);

            var response = http.PostAsync(http.BaseAddress, formContent).Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                result = response.Content.ReadAsStringAsync().Result;
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return result;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.PaymentRequired)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return response.ReasonPhrase.ToString();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
            {
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + response.ReasonPhrase.ToString());
                return response.ReasonPhrase.ToString();
            }
            return response.ReasonPhrase.ToString();
        }

        private string GetUserDetail(string access_token)
        {
            string accountid = "";
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
                JObject obj = JObject.Parse(ojJarray[0]["business"].ToString());
                accountid = obj.GetValue("account_id").ToString();
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + accountid);
            }
            else
            {
                accountid = response.ReasonPhrase.ToString();
                _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + accountid);
            }
            return accountid;
        }

        #endregion

        #region Update Receipt Status

        [HttpPost]
        [Route("UpdateReceiptStatus")]
        public ActionResult<AI_ReceiptModel> UpdateReceiptStatus([FromBody]AI_ReceiptModel model, string EmailId)
        {
            var msg = new AI_Message<AI_ReceiptModel>();

            var emailId = DbClientFactory<FreshbookDbClient>.Instance.GetManagerByEmailId(appSettings.Value.DbConn, EmailId);
            if (emailId.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    model.AssignTo = emailId[0].AssignTo.ToString();
                    var result = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateReceiptStatus(model, appSettings.Value.DbConn);

                    if (result == "1")
                    {
                        msg.success = true;
                        msg.message = "Receipt status updated successfully";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                    }
                    else if (result == "-1")
                    {
                        msg.success = false;
                        msg.message = "Unable to update receipt status";
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
            return Ok(msg);
        }

        #endregion

        #region Update Receipt Status For Normal User

        [HttpPost]
        [Route("UpdateReceiptStatusForNormalUser")]
        public ActionResult<AI_ReceiptModel> UpdateReceiptStatusForNormalUser([FromBody]AI_ReceiptModel model)
        {
            var msg = new AI_Message<AI_ReceiptModel>();

            var emailId = DbClientFactory<FreshbookDbClient>.Instance.GetManagerByEmailId(appSettings.Value.DbConn, model.CreatedBy);
            if (emailId.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    model.AssignTo = emailId[0].AssignTo.ToString();
                    var result = DbClientFactory<AI_ReceiptDbClient>.Instance.UpdateReceiptStatus(model, appSettings.Value.DbConn);

                    if (result == "1")
                    {
                        msg.success = true;
                        msg.message = "Receipt status updated successfully";
                        _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                    }
                    else if (result == "-1")
                    {
                        msg.success = false;
                        msg.message = "Unable to update receipt status";
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
            return Ok(msg);
        }

        #endregion

        #region List Of Clients Or Customer Freshbook 

        [HttpGet]
        [Route("GetClients")]
        public ActionResult<FreshbookModel> GetClients(int CompanyId)
        {
            var count = DbClientFactory<FreshbookDbClient>.Instance.GetFreshbookDetailByCompanyId(appSettings.Value.DbConn, CompanyId.ToString());
            string access_token = count[0].AccessToken;
            // string userAccountId = GetUserDetail(access_token);
            string userAccountId = count[0].AccountId;
            //  string userAccountId = account_id;
            HttpClient http = new HttpClient();

            http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", access_token);

            var response = http.GetAsync("https://api.freshbooks.com/accounting/account/" + userAccountId + "/users/clients").Result;
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

        #region Update account_id for Freshbook

        [HttpPost]
        [Route("UpdateFreshbookAccountId")]
        public ActionResult<FreshbookModel> UpdateFreshbookAccountId(string account_id, int companyId, bool IsSelected)
        {
            var msg = new FreshbookMessgae<FreshbookModel>();

            if (ModelState.IsValid)
            {
                var result = DbClientFactory<FreshbookDbClient>.Instance.UpdateFreshbookAccountId(account_id, companyId, IsSelected, appSettings.Value.DbConn);
                if (result == "1")
                {
                    msg.success = true;
                    msg.message = "Business member updated successfully";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
                else if (result == "-1")
                {
                    msg.success = false;
                    msg.message = "Problem in update business member";
                    _logger.LogInformation(MethodBase.GetCurrentMethod().Name + ' ' + msg.message);
                }
            }
            return Ok(msg);
        }

        #endregion
    }
}