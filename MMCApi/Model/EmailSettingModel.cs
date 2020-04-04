using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model
{
    public class EmailSettingModel
    {
        public string MailFrom { get; set; }
        public string Password { get; set; }
        public string MailTo { get; set; }
    }
}
