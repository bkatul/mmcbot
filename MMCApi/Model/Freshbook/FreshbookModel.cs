using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MMCApi.Model
{
    public class FreshbookModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int CompanyId { get; set; }
        public string Company { get; set; }
        public string AccountId { get; set; }
        public bool IsSelected { get; set; }
    }
    public class roles
    {
        public string accountid { get; set; }
    }

    public class FreshbookGetCategories
    {
        public int CompanyId { get; set; }
    }

    public class FreshbookTokenModel
    {
        [DataMember(Name = "access_token")]
        public string access_token { get; set; }

        [DataMember(Name = "refresh_token")]
        public string refresh_token { get; set; }
    }

    public class FreshbookBusinessMemberModel
    {
        public business business { get; set; }
    }

    public class business
    {
        public int id { get; set; }
        public string name { get; set; }
        public string account_id { get; set; }
        public bool isSelected { get; set; }
    }

}
