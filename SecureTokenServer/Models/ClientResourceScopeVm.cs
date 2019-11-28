using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class ClientResourceScopeVm
    {
        [Required(ErrorMessage ="{0} is required")]
        [Display(Name ="Scope Name")]
        public string ScopeName { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Scope Description")]
        public string ScopeDescription { get; set; }
        public string ResourceName { get; internal set; }
    }
}
