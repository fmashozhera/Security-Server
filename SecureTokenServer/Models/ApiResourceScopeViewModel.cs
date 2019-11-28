using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureTokenServer.Models
{
    public class ApiResourceScopeViewModel
    {
        public ApiResourceScopeViewModel()
        {
            UserClaims = new List<string>();
        }

        public int Id { get; set; }

        
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name ="Display Name")]
        public string DisplayName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ApiResourceName { get; set; }
        public ICollection<string> UserClaims { get; set; }
        public ScopeClaimViewModel ActiveClaim { get; set; }
    }
}
