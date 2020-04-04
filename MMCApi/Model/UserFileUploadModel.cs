using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model
{
    public class UserFileUploadModel
    {
        public string UserId { get; set; }
        public ICollection<IFormFile> File { get; set; }
      //  public IFormFile File { get; set; }
        public string CreatedBy { get; set; }
        public string AssignTo { get; set; }
    }

    public class OutPutResult
    {
        public bool success { get; set; }
        public string error { get; set; }
    }

}
