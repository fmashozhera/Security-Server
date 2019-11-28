using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class ScopeClaimViewModel
    {   
        public string Type  { get; set; }
        public string ScopeName { get; set; }
        public string ApiResourceName { get; set; }
    }
}
