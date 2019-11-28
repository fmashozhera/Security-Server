using System.ComponentModel.DataAnnotations;

namespace SecureTokenServer.Models
{
    public class ClientResourceVm
    {
        [Required(ErrorMessage ="{0} is required")]
        [Display(Name ="Resource Name")]
        public string ResourceName { get; set; }

        public string DisplayName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Resource Description")]
        public string ResourceDescription { get; set; }

        public string ClientName { get; set; }

    }
}
