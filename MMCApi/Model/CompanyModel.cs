using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model
{
    public class CompanyModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FreshbookAccountType { get; set; }
    }

    public class GetCompanyModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
