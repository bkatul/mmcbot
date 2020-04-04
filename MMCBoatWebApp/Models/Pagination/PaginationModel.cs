using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCBoatWebApp.Models.Pagination
{
    public class PaginationModel : PageModel
    {
        public int CurrentPage { get; set; } 

        public int TotalPages { get; set; }

        public List<Models.ReceiptModel> Data { get; set; }

        public List<UserMMCGetModel> userMMCData { get; set; }

     //   public Models.ReceiptSearchModel searchData { get; set; }
    }
}
