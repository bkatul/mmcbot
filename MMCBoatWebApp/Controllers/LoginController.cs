using CoreApiAdoDemo.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MMCBoatWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MMCBoatWebApp.Controllers
{

    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            #region Old Code
            //ViewBag.Message = HttpContext.Request.Query["code"].ToString();
            //var code = HttpContext.Request.Query["code"].ToString();
            //if (!string.IsNullOrEmpty(code))
            //{
            //    HttpClient http = new HttpClient();

            //    var data = new
            //    {
            //        grant_type = "authorization_code",
            //        client_secret = "f4b58666fa3461d056c9b5c5903dd43942bd4db526b0735635c171f3a23e9520",
            //        code = code,
            //        client_id = "12b70c1d206015f98a70ad546559e022ea482cd750540229754e64a765f035e9",
            //        redirect_uri = "https://localhost:44347/Login"
            //    };

            //    var postdata = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            //    http.DefaultRequestHeaders.Add("Api-Version", "alpha");

            //    StringContent content = new StringContent(postdata, Encoding.UTF8, "application/json");
            //    var response = http.PostAsync("https://api.freshbooks.com/auth/oauth/token", content).Result;
            //    var result = response.Content.ReadAsStringAsync().Result;
            //    Authorization authorization = JsonConvert.DeserializeObject<Authorization>(result);
            //    ViewBag.Message = result;
            //    TempData["mydata"] = result;

            //    // return View(authorization);
            //    return RedirectToAction("Index", "Home");
            //}
            #endregion
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<LoginModel>> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var data = new
                    {
                        EmailId = model.EmailId,
                        Password = model.Password,
                    };

                    // var response = client.PostAsJsonAsync("http://mmcbot4.azurewebsites.net/api/Auth/Authenticate", data).Result;
                    var response = client.PostAsJsonAsync("http://mmcbot4.azurewebsites.net/api/MMCUser/GetMMCUserByEmailAndPassword", data).Result;
                    var result = response.Content.ReadAsStringAsync().Result;
                    var res = JsonConvert.DeserializeObject<UserMessage>(result);
                    var loginData = JsonConvert.DeserializeObject<GetLoginModel>(result);

                    // TempData["companyId"] = loginData.companyId;
                    //TempData.Keep("companyId");

                    TempData["FullName"] = loginData.firstName + ' ' + loginData.lastName;
                    TempData["user_type"] = loginData.user_type;
                    //TempData.Keep("FullName");

                    if (res.success == true)
                    {
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.EmailId)
                    };
                        ClaimsIdentity userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(HttpContext, principal);

                        return RedirectToAction("List", "Receipt");
                    }
                    else
                    {
                        if (model.Password != null || model.Password != "")
                        {
                            ModelState.AddModelError("Error", "Your email address or password is incorrect");
                            //ViewBag.Message = String.Format("Invalid credentials");
                        }
                    }
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
           
            //HttpContext context;
            Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(this.HttpContext);
            return RedirectToAction("Login", "Login");
        }
    }


    public class Authorization
    {
        [Newtonsoft.Json.JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [Newtonsoft.Json.JsonProperty("token_type")]
        public string TokenType { get; set; }
        [Newtonsoft.Json.JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [Newtonsoft.Json.JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [Newtonsoft.Json.JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }


}