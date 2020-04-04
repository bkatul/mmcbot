using System.Runtime.Serialization;

namespace CoreApiAdoDemo.Model
{
    [DataContract]
    public class UserMessage
    {
      
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }

        [DataMember(Name = "user_type")]
        public string user_type { get; set; }
    }
}


