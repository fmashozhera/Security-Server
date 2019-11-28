using Microsoft.AspNetCore.Identity;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecureTokenServer.Services
{
    public class RolesService : IRoleService
    {

        private RoleManager<IdentityRole> _roleManager;

        public RolesService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityRole> Add(IdentityRole identityRole)
        {
            var result = await _roleManager.CreateAsync(identityRole);
            if (result.Succeeded)
            {
                return identityRole;
            }
            else
            {
                throw new BusinessRuleException(GetErrorMessage(result.Errors));
            }
        }

        private string GetErrorMessage(IEnumerable<IdentityError> errors)
        {
            var errorBuilder = new StringBuilder();
            foreach (var error in errors)
            {
                errorBuilder.Append(error.Description + "|");
            }
            return errorBuilder.ToString();
        }

        public async Task<IdentityRole> AddClaim(IdentityRole identityRole, string claimType, string claimValue)
        {
            await _roleManager.AddClaimAsync(identityRole, new Claim (claimType, claimValue));
            return identityRole;
        }

        public async Task Delete(IdentityRole identityRole)
        {
            await _roleManager.DeleteAsync(identityRole);
        }

        public async Task DeleteClaim(IdentityRole identityRole,string claimType, string claimValue)
        {
            await _roleManager.AddClaimAsync(identityRole, new Claim(claimType, claimValue));           
        }

        public async Task<IdentityRole> Get(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return role;
        }

        public ICollection<IdentityRole> List()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task Update(IdentityRole identityRole)
        {
            var updatedRole = await _roleManager.FindByIdAsync(identityRole.Id);
            updatedRole.Name = identityRole.Name;
            await _roleManager.UpdateAsync(updatedRole);
        }

        public async Task<ICollection<Claim>> GetClaims(IdentityRole identityRole)
        {
            return await _roleManager.GetClaimsAsync(identityRole);
        }


    }
}
