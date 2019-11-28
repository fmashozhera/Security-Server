using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class RoleIndexVm
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name ="Role Name")]
        public string Name { get; set; }
    }
}
