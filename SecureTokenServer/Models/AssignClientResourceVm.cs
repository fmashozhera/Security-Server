using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class AssignClientResourceVm
    {
        public string ClientName { get; set; }
        public ICollection<string> Resources { get; set; }
    }
}
