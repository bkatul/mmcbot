using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMCApi.Model
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Role { get; set; }
    }

    public class MMCRoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
