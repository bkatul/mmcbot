using Microsoft.AspNetCore.Mvc;
using MMCBoatWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace MMCBoatWebApp.Controllers
{
    public class TempController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string url)
        {
            url = "https://mmcbotstorage.blob.core.windows.net/mmcbotcontainer/Tim%20Hortons.jpeg";
            string res = ReceiptUpdate(url);
            Thread.Sleep(500);
            TempModel jsonData = JsonConvert.DeserializeObject<TempModel>(res);
            string url1= jsonData.message;
            var ress = GetReceipt(url1);
            
            return View(ress);
        }

        public string GetReceipt(string res)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/Blob/GetRequest_New?url=" + res);
                //HTTP GET
                var responseTask = client.GetAsync(client.BaseAddress);
                responseTask.Wait();

                var result = responseTask.Result;
                var json = result.Content.ReadAsStringAsync();
                return json.Result.ToString();
            }
        }

        public string ReceiptUpdate(string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://mmcbot4.azurewebsites.net/api/blob/UploadImage_New?url=" + url);
                var content = new StringContent(url);
                var responseTask = client.PostAsync(client.BaseAddress, content).Result;
                var res = responseTask.Content.ReadAsStringAsync();
                return res.Result.ToString();
            }
            
        }
    }
}