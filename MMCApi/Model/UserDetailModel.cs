using System;
using System.ComponentModel.DataAnnotations;

namespace MMCApi.Model
{
    public class UserDetailModel
    {
        public string AssignTo { get; set; }
        public string CreatedBy { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string Position { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }
}
