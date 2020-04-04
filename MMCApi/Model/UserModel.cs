using System;
using System.ComponentModel.DataAnnotations;

namespace MMCApi.Model
{
    public class UserModel
    {
        public int Id { get; set; }

        public string CreatedBy { get; set; }
        public string AssignTo { get; set; }
        public string RegistrationId { get; set; }

        public string Tag { get; set; }

        public int CompanyId { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        public string Company { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect email format")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect password format")]
        //^(?!.* )(?=.*\d)(?=.*[A-Z]).{8,15}$
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }

        public bool IsRegisteredByFreshbook { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Url { get; set; }
        public string FreshbookAccountType { get; set; }

        public string EmailInAddress { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect email format")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect password format")]
        //^(?!.* )(?=.*\d)(?=.*[A-Z]).{8,15}$
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
    }

    public class InvitationModel
    {
        public int CompanyId { get; set; }
        public string RegistrationId { get; set; }
        public string Tag { get; set; }
        public string CreatedBy { get; set; }

        public string AssignTo { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        public string Company { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(50)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Incorrect email format")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect password format")]
        //^(?!.* )(?=.*\d)(?=.*[A-Z]).{8,15}$
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string EmailInAddress { get; set; }
        
    }

    public class UserGetModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string CreatedBy { get; set; }
        public string AssignTo { get; set; }
        public string RegistrationId { get; set; }
        public string Tag { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZIP { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public bool IsRegisteredByFreshbook { get; set; }
        public string PushNotificationKey { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class ChangePassword
    {
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?!.* )(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Incorrect password format")]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
    }

    public class CountriesModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string ISO3 { get; set; }

    }

}
