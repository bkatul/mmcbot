using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MMCBoatWebApp.Models;
using MMCBoatWebApp.Models.Pagination;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace MMCBoatWebApp.Controllers
{
    [Authorize]
    public class ReceiptController : Controller
    {
       
        [HttpGet]
        public ActionResult List(int pageno = 1)
        {

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;

            try
            {
                #region  Company and status list
                List<ReceiptStatusModel> status = new List<ReceiptStatusModel>();

                status.Insert(0, new ReceiptStatusModel { Id = 0, StatusName = "Pending for MMC Review" });
                status.Insert(1, new ReceiptStatusModel { Id = 1, StatusName = "Ready to Publish" });

                if (status.Count > 0)
                {
                    ViewBag.ListOfStatus = status;
                }

                List<CompanyModel> company = GetAllCompanies();
                if (company.Count > 0)
                {
                    ViewBag.ListOfCompany = company;
                }

                #endregion

                string FullName = "";
                //pageno = Convert.ToInt32(HttpContext.Request.Query["currentpage"]);
                if (TempData["FullName"] != null)
                {
                    FullName = TempData["FullName"].ToString();
                    ViewBag.User = FullName;
                    TempData.Keep("FullName");
                }

                if (TempData["user_type"] != null)
                {
                    ViewBag.UserType = TempData["user_type"].ToString();
                    TempData.Keep("user_type");
                }
                //IEnumerable<ReceiptModel> receipt = null;
                //PaginationModel model = new PaginationModel();


                //using (var client = new HttpClient())
                //{
                //    //   client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/GetAllReceiptByCompanyId?CompanyId=" + companyId);

                //    client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/MMCReceipt/GetAllReceipt");
                //    //HTTP GET
                //    var responseTask = client.GetAsync(client.BaseAddress);
                //    responseTask.Wait();

                //    var result = responseTask.Result;
                //    if (result.IsSuccessStatusCode)
                //    {
                //        var readTask = result.Content.ReadAsAsync<IList<ReceiptModel>>();
                //        readTask.Wait();

                //        receipt = readTask.Result;
                //        model.TotalPages = receipt.Count();
                //        model.CurrentPage = pageno == 0 ? 1 : pageno;
                //        model.Data = receipt.Skip((pageno - 1) * 10).Take(10).ToList();

                //        if (TempData["success"] != null)
                //        {
                //            string msg = TempData["success"].ToString();
                //            if (msg == "Receipt submitted successfully")
                //            {
                //                ViewBag.Message = String.Format("Receipt submitted successfully");
                //                TempData["success"] = null;
                //            }
                //        }
                //    }
                //    else //web api sent error response 
                //    {
                //        receipt = Enumerable.Empty<ReceiptModel>();

                //        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                //    }
                //}
                if (TempData["success"] != null)
                {
                    string msg = TempData["success"].ToString();
                    if (msg == "Receipt submitted successfully")
                    {
                        ViewBag.Message = String.Format("Receipt submitted successfully");
                        TempData["success"] = null;
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View(new PaginationModel());
            }
        }

        private List<CompanyModel> GetAllCompanies()
        {
            IEnumerable<CompanyModel> companies = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/MMCUser/GetAllCompany");
                //HTTP GET
                var responseTask = client.GetAsync(client.BaseAddress);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CompanyModel>>();
                    readTask.Wait();

                    companies = readTask.Result;
                }
            }

            return companies.ToList();
        }

        public IEnumerable<ReceiptModel> GetById(string Id)
        {
            try
            {
                IEnumerable<ReceiptModel> receipt = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/GetReceiptById?Id=" + Id);
                    //HTTP GET
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<ReceiptModel>>();
                        readTask.Wait();

                        receipt = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        receipt = Enumerable.Empty<ReceiptModel>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
                return receipt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Edit(string id)
        {
            string FullName = "";
            if (TempData["FullName"] != null)
            {
                FullName = TempData["FullName"].ToString();
                ViewBag.User = FullName;
                TempData.Keep("FullName");
            }

            if (TempData["user_type"] != null)
            {
                ViewBag.UserType = TempData["user_type"].ToString();
                TempData.Keep("user_type");
            }

            IEnumerable<ReceiptModel> receipt = GetById(id);
            TempData["url"] = receipt.FirstOrDefault().url;
            ViewBag.Url = TempData["url"].ToString();
            TempData.Keep("url");
            TempData["CreatedBy"] = receipt.FirstOrDefault().createdBy;
            TempData.Keep("CreatedBy");
            if (TempData["userName"] != null)
            {
                ViewBag.User = TempData["userName"].ToString();
            }

            return View(receipt.FirstOrDefault());
        }

        [HttpPost]
        public IActionResult Edit(ReceiptModel model)
        {
            if (ModelState.IsValid)
            {
                var receiptModel = GetCategoryIdByVendor(model.vendorName);
                if (receiptModel.Count() > 0)
                {
                    UpdateCategory modelUpdate = new UpdateCategory();
                    modelUpdate.Id = model.id;
                    modelUpdate.categoryId = receiptModel.FirstOrDefault().categoryId;
                    modelUpdate.VendorName = receiptModel.FirstOrDefault().VendorName;
                    string resCategoryId = UpdateCategoryIdByVendor(modelUpdate);
                    model.categoryId = modelUpdate.categoryId;
                }
                if (TempData["CreatedBy"] != null)
                {
                    model.createdBy = TempData["CreatedBy"] as string;
                }
                var role = GetUserRoleByEmailId(model.createdBy);
                if (role.FirstOrDefault().Role == "User")
                {
                    var update = UpdateReceiptStatus(model);
                }

                string res = ReceiptUpdate(model);
                if (res == "Success")
                {
                    TempData["success"] = "Receipt submitted successfully";
                    TempData.Keep("success");
                    return RedirectToAction("List", "Receipt");
                }
            }
            else
            {
                if (TempData["url"] != null)
                {
                    ViewBag.Url = TempData["url"].ToString();
                    TempData.Keep("url");
                }

                string FullName = "";
                if (TempData["FullName"] != null)
                {
                    FullName = TempData["FullName"].ToString();
                    ViewBag.User = FullName;
                    TempData.Keep("FullName");
                }

                if (TempData["user_type"] != null)
                {
                    ViewBag.UserType = TempData["user_type"].ToString();
                    TempData.Keep("user_type");
                }
            }
            return View();
        }

        private object UpdateReceiptStatus(ReceiptModel model)
        {

            var res = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Freshbook/UpdateReceiptStatusForNormalUser");

                var responseTask = client.PostAsJsonAsync(client.BaseAddress, model).Result;

                var result = responseTask.StatusCode;

                if (responseTask.StatusCode == HttpStatusCode.OK)
                {
                    var jsonData = JsonConvert.SerializeObject(model);

                    Notification notify = new Notification();
                    notify.Content = jsonData;
                    res = SendPushNotification(notify);
                }
            }
            return res;
        }

        public IEnumerable<UserGetModel> GetUserRoleByEmailId(string createdBy)
        {
            try
            {
                IEnumerable<UserGetModel> user = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/User/GetUserByEmailId?emailid=" + createdBy);
                    //HTTP GET
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<UserGetModel>>();
                        readTask.Wait();

                        user = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        user = Enumerable.Empty<UserGetModel>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
                return user;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<UpdateCategory> GetCategoryIdByVendor(string vendorName)
        {
            try
            {
                IEnumerable<UpdateCategory> receipt = null;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/GetCategoryIdByVendor?vendorName=" + vendorName);
                    //HTTP GET
                    var responseTask = client.GetAsync(client.BaseAddress);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<UpdateCategory>>();
                        readTask.Wait();

                        receipt = readTask.Result;
                    }
                    else //web api sent error response 
                    {
                        receipt = Enumerable.Empty<UpdateCategory>();

                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
                return receipt;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Alert()
        {
            return View();
        }

        public string ReceiptUpdate(ReceiptModel model)
        {
            model.status = "Ready to Publish";
            model.url = TempData["url"].ToString();
            var res = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/UpdateReceiptFromWeb");

                var responseTask = client.PostAsJsonAsync(client.BaseAddress, model).Result;

                var result = responseTask.StatusCode;

                if (responseTask.StatusCode == HttpStatusCode.OK)
                {
                    var jsonData = JsonConvert.SerializeObject(model);

                    Notification notify = new Notification();
                    notify.Content = jsonData;
                    res = SendPushNotification(notify);
                }
            }
            return res;
        }

        public string UpdateCategoryIdByVendor(UpdateCategory model)
        {
            var res = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/UpdateCategoryIdByVendor");

                var responseTask = client.PostAsJsonAsync(client.BaseAddress, model).Result;

                var result = responseTask.StatusCode;

                if (responseTask.StatusCode == HttpStatusCode.OK)
                {

                }
            }
            return res;
        }

        public string SendPushNotification(Notification notify)
        {
            string res = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/PushNotifications/send");

                    var responseTask = client.PostAsJsonAsync(client.BaseAddress, notify);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        res = "Success";
                    }
                    else
                    {
                        res = "Fail";
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //[HttpPost]
        //public IActionResult List(string FromDate, string ToDate, string CompanyName, string StatusName, int pageno = 1)
        //{
        //    IEnumerable<ReceiptModel> receipt_S = null;
        //    PaginationModel model_S = new PaginationModel();

        //    #region  Company and status list
        //    List<ReceiptStatusModel> status = new List<ReceiptStatusModel>();

        //    status.Insert(0, new ReceiptStatusModel { Id = 0, StatusName = "All" });
        //    status.Insert(1, new ReceiptStatusModel { Id = 1, StatusName = "Pending for MMC review" });
        //    status.Insert(2, new ReceiptStatusModel { Id = 2, StatusName = "Ready to Publish" });

        //    if (status.Count > 0)
        //    {
        //        ViewBag.ListOfStatus = status;
        //    }

        //    List<CompanyModel> company = new List<CompanyModel>();

        //    company.Insert(0, new CompanyModel { Id = 0, Name = "All" });
        //    company.Insert(1, new CompanyModel { Id = 1, Name = "Beyondkey" });
        //    company.Insert(2, new CompanyModel { Id = 2, Name = "Beyondkey 02" });

        //    if (company.Count > 0)
        //    {
        //        ViewBag.ListOfCompany = company;
        //    }

        //    #endregion

        //    try
        //    {
        //        string FullName = "";
        //        pageno = Convert.ToInt32(HttpContext.Request.Query["currentpage"]);
        //        if (TempData["FullName"] != null)
        //        {
        //            FullName = TempData["FullName"].ToString();
        //            ViewBag.User = FullName;
        //            TempData.Keep("FullName");
        //        }

        //        if (TempData["user_type"] != null)
        //        {
        //            ViewBag.UserType = TempData["user_type"].ToString();
        //            TempData.Keep("user_type");
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/SearchReceiptData?statusName=" + StatusName + "&companyName=" + CompanyName + "&frmDate=" + FromDate + "&toDate=" + ToDate);
        //            var responseTask = client.GetAsync(client.BaseAddress);
        //            responseTask.Wait();

        //            var result = responseTask.Result;


        //            if (result.IsSuccessStatusCode)
        //            {
        //                var readTask = result.Content.ReadAsAsync<IList<ReceiptModel>>();
        //                readTask.Wait();

        //                receipt_S = readTask.Result;
        //                model_S.TotalPages = receipt_S.Count();
        //                model_S.CurrentPage = pageno == 0 ? 1 : pageno;
        //                model_S.Data = receipt_S.Skip((pageno - 1) * 10).Take(10).ToList();
        //            }
        //            else
        //            {
        //                receipt_S = Enumerable.Empty<ReceiptModel>();

        //                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
        //            }
        //        }
        //        return View(model_S);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}