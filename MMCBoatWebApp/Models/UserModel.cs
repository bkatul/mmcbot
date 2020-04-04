using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MMCBoatWebApp.Models
{
    public class UserModel
    {
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect Password Format")]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password Must be Same.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserGetModel
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public string AssignTo { get; set; }
        public string RegistrationId { get; set; }
        public string Tag { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public bool IsRegisteredByFreshbook { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Url { get; set; }
        public string FreshbookAccountType { get; set; }
    }
}
