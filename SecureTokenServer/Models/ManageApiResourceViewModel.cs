using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Models
{
    public class ManageApiResourceViewModel
    {

        public ManageApiResourceViewModel()
        {
            Scopes = new List<ApiResourceScopeViewModel>();
            ActiveScopeViewModel = new ApiResourceScopeViewModel();
            ActiveScopeClaimViewModel = new ScopeClaimViewModel();
        }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name ="Display Name")]
        public string DisplayName { get; set; }
        [Required]
        public string Description { get; set; }
        public IEnumerable<ApiResourceScopeViewModel> Scopes { get; set; }
        public ApiResourceScopeViewModel ActiveScopeViewModel { get; set; }
        public ScopeClaimViewModel ActiveScopeClaimViewModel { get; set; }
    }
}
