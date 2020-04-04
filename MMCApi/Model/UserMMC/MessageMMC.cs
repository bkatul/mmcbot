using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MMCApi.Model.UserMMC
{
    [DataContract]
    public class MessageMMC<T>
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }
    }

    [DataContract]
    public class MessageMMCWithData<T>
    {
        [DataMember(Name = "success")]
        public bool success { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }
        [DataMember(Name = "userModel")]
        public List<MMCUserModel> userModel { get; set; }


    }
}
