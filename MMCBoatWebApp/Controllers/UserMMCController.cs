using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MMCBoatWebApp.Model;
using MMCBoatWebApp.Models;
using MMCBoatWebApp.Models.Pagination;

namespace MMCBoatWebApp.Controllers
{
    [Authorize]
    public class UserMMCController : Controller
    {
        [HttpGet]
      
        public ActionResult List(int pageno = 1)
        {
            string FullName = "";
            //if (TempData["FullName"] != null)
            //{
            //    FullName = TempData["FullName"].ToString();
            //    ViewBag.User = FullName;
            //}

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
            pageno = Convert.ToInt32(HttpContext.Request.Query["currentpage"]);
            IEnumerable<UserMMCGetModel> user = null;
            PaginationModel model = new PaginationModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/MMCUser/GetAllMMCUser");

                var responseTask = client.GetAsync(client.BaseAddress);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<UserMMCGetModel>>();
                    readTask.Wait();

                    user = readTask.Result;
                    model.TotalPages = user.Count();
                    model.CurrentPage = pageno == 0 ? 1 : pageno;
                    model.userMMCData = user.Skip((pageno - 1) * 10).Take(10).ToList();
                }
            }

            return View(model);
        }

        public ActionResult CreateUser()
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
            List<CountriesModel> countries = GetAllCountries();
            countries.Insert(0, new CountriesModel { CountryID = 0, CountryName = "Select Country" });
            if (countries.Count > 0)
            {
                ViewBag.ListOfCountry = countries;
            }

            List<MMCRoleModel> roles = GetAllRoles();
            roles.Insert(0, new MMCRoleModel { Id = 0, Name = "Select Role" });
            if (roles.Count > 0)
            {
                ViewBag.ListOfRoles = roles;
            }

            return View();
        }

        public List<MMCRoleModel> GetAllRoles()
        {
            IEnumerable<MMCRoleModel> roles = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/User/GetAllRoleMMC");
                //HTTP GET
                var responseTask = client.GetAsync(client.BaseAddress);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<MMCRoleModel>>();
                    readTask.Wait();

                    roles = readTask.Result;
                }
            }

            return roles.ToList();
        }

        [HttpPost]
      
        public ActionResult CreateUser(UserMMCModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/MMCUser/CreateUser");

                var responseTask = client.PostAsJsonAsync(client.BaseAddress, model).Result;

                var result = responseTask.StatusCode;

                if (responseTask.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("List", "UserMMC");
                }
            }

            return View();
        }

        public List<CountriesModel> GetAllCountries()
        {
            IEnumerable<CountriesModel> countries = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/User/GetAllCountries");
                //HTTP GET
                var responseTask = client.GetAsync(client.BaseAddress);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<CountriesModel>>();
                    readTask.Wait();

                    countries = readTask.Result;
                }
            }

            return countries.ToList();
        }
    }
}