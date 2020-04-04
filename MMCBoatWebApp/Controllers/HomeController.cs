using Microsoft.AspNetCore.Mvc;
using MMCBoatWebApp.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MMCBoatWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string data = TempData["mydata"] as string;
            TempData.Keep("mydata");
            Authorization authorization = JsonConvert.DeserializeObject<Authorization>(data);
            return View(authorization);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
