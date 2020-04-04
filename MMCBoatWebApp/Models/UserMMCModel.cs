using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMCBoatWebApp.Models
{
    public class UserMMCModel
    {
        public int id { get; set; }

        [Display(Name = "User Type")]
        public int roleId { get; set; }
        public int companyId { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Email Address")]
        public string emailAddress { get; set; }

        [Display(Name = "Password")]
        public string password { get; set; }
        public int cityId { get; set; }
        public int stateId { get; set; }

        [Display(Name = "Country")]
        public int countryId { get; set; }
        public bool isActive { get; set; }
    }

    public class UserMMCGetModel
    {
        public int id { get; set; }

        [Display(Name = "Role")]
        public string role { get; set; }

        [Display(Name = "Company")]
        public string company { get; set; }

        [Display(Name = "CompanyId")]
        public string companyId { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Email Address")]
        public string emailAddress { get; set; }
        public string password { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "Is Active")]
        public bool isActive { get; set; }
    }
}
