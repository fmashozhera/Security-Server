using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SecureTokenServer.Interfaces
{
    public interface IRoleService
    {
        Task<IdentityRole> Add(IdentityRole identityRole);
        Task Update(IdentityRole identityRole);
        Task Delete(IdentityRole identityRole);
        ICollection<IdentityRole> List();
        Task<IdentityRole> Get(string id);
        Task<IdentityRole> AddClaim(IdentityRole identityRole, string claimType, string claimValue);
        Task DeleteClaim(IdentityRole identityRole, string claimType, string claimValue);
        Task<ICollection<Claim>> GetClaims(IdentityRole identityRole);        
    }

}
