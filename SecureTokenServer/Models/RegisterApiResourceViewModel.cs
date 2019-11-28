using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class RegisterApiResourceViewModel
    {
        [Required(ErrorMessage ="Resource name is required")]
        public string Name { get; set; }

        [Display(Name ="Display Name")]
        [Required(ErrorMessage ="{0} is required")]
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}
