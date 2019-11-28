using IdentityServer4;
using SecureTokenServer.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace SecureTokenServer.Models
{
    public class RegisterClientViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Client Id")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Client Url")]
        public string ClientUri { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Redirect Url")]
        public string RedirectUri { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Post Logoout Redirect Id")]       

        public string PostLogoutRedirectUri { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }


        public IEnumerable<OidcScopes> AllowedScopes { get; set; }      
    }
}
