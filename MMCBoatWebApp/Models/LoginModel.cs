using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMCBoatWebApp.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessage = "Email Id is Required")]
        //[DataType(DataType.EmailAddress)]
        //[MaxLength(50)]
        //[RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect Email Format")]

        public string EmailId { get; set; }

        public string Username { get; set; }
        //[Required(ErrorMessage = "Password is required.")]
        //[RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect Password Format")]
        ////^(?!.* )(?=.*\d)(?=.*[A-Z]).{8,15}$
        //[StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        public string Role { get; set; }
        public int CompanyId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class GetLoginModel
    {
        public int id { get; set; }
        public int companyId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string user_type { get; set; }
    }
}
