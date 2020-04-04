using MMCApi.Model;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreApiAdoDemo.Model
{
    [DataContract]
    public class Message<T>
    {
        //public Message()
        //{
        //    message = new List<string>();
        //}

        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }
        //  public List<string>  message { get; set; }

        [DataMember(Name = "access_token")]
        public string access_token { get; set; }

        [DataMember(Name = "user_type")]
        public string user_type { get; set; }

        [DataMember(Name = "company")]
        public string company { get; set; }

        [DataMember(Name = "registrationId")]
        public string registrationId { get; set; }

        [DataMember(Name = "tag")]
        public string tag { get; set; }

        [DataMember(Name = "companyId")]
        public int companyId { get; set; }

        [DataMember(Name = "id")]
        public int id { get; set; }

        [DataMember(Name = "firstName")]
        public string firstName { get; set; }

        [DataMember(Name = "lastName")]
        public string lastName { get; set; }

        [DataMember(Name = "emailId")]
        public string emailId { get; set; }

    }

    [DataContract]
    public class AI_Message<T>
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }

        //[DataMember(Name = "url")]
        //public string url { get; set; }

        [DataMember(Name = "isDuplicate")]
        public bool isDuplicate { get; set; }

        [DataMember(Name = "receiptModel")]
        public List<AI_ReceiptModel> receiptModel { get; set; }

        //[DataMember(Name = "receiptGetModel")]
        //public AI_ReceiptModel receiptGetModel { get; set; }
    }

    [DataContract]
    public class FreshbookMessgae<T>
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }

    }


    [DataContract]
    public class AI_Message_Insert<T>
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }

        [DataMember(Name = "receiptModel")]
        public AI_ReceiptModel receiptModel { get; set; }
    }

    [DataContract]
    public class CommonMessgae
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }

    }
}


