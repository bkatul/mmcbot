using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model.UserMMC
{
    public class MMCUserModel
    {
        public int id { get; set; }
        public int roleId { get; set; }
        public int companyId { get; set; }
        public string company { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string password { get; set; }
        public int cityId { get; set; }
        public int stateId { get; set; }
        public int countryId { get; set; }
        public string country { get; set; }
        public bool isActive { get; set; }
        public string role { get; set; }
    }

    public class MMCCompanyModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
}
