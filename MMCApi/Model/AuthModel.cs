using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model
{
    public class AuthModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect email format")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect password format")]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
        public string Company { get; set; }
        public string RegistrationId { get; set; }
        public string Tag { get; set; }
        public int CompanyId { get; set; }
        public string EmailId { get; set; }
    }
}
