using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Mvc;
using MMCBoatWebApp.Models;
using Newtonsoft.Json;
using System.Net.Http;

namespace MMCBoatWebApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<UserModel> ForgotPassword(UserModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var data = new
                    {
                        EmailId = "nakul.paithankar@beyondkey.com",
                        Password = model.Password,
                    };

                    var response = client.PostAsJsonAsync("http://mmcbot2.azurewebsites.net/api/User/ForgotPassword", data).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    UserMessage msg = JsonConvert.DeserializeObject<UserMessage>(result);
                    TempData["user"] = "" ;
                    if (msg.success == true)
                    {
                        TempData["user"] = msg.message;
                        return RedirectToAction("AlertView", "User");
                    }
                    else
                    {
                        TempData["user"] = msg.message;
                        return RedirectToAction("AlertView", "User");
                    }
                }
            }
            return View();
        }

        public IActionResult AlertView()
        {
            string data = TempData["user"] as string;
            ViewBag.Message = data;
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username,string password)
        {
            return View();
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<UserModel> ChangePassword(UserModel model)
        {
            string email = HttpContext.Request.Query["email"].ToString();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var data = new
                    {
                        EmailId = email,
                        Password = model.Password,
                    };

                    var response = client.PostAsJsonAsync("http://mmcbot4.azurewebsites.net/api/User/ChangePassword", data).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    UserMessage msg = JsonConvert.DeserializeObject<UserMessage>(result);
                   
                    if (msg.success == true)
                    {
                        return RedirectToAction("AlertView", "User");
                    }
                    else
                    {
                        return RedirectToAction("User", "ChangePassword");
                    }
                }
            }
            return View();
        }

    }
}