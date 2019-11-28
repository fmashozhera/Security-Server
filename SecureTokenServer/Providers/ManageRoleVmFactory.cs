using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Models;
using SecureTokenServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Providers
{
    public class ManageRoleVmFactory
    {
        private readonly IApiResourceService _apiResourceService;
        private readonly RolesService _roleService;

        public ManageRoleVmFactory(
           IApiResourceService apiResourceService,
           RolesService rolesService
           )
        {
            _apiResourceService = apiResourceService;
            _roleService = rolesService;
        }

        public async Task<ManageRoleVm> CreateModel(string roleId)
        {
            var role = await _roleService.Get(roleId);
            var model = new ManageRoleVm();
            model.RoleId = roleId;
            model.RoleName = role.Name;

            await InitialiseResources(model,role);

            return model;
        }

        private async Task InitialiseResources(ManageRoleVm model,IdentityRole role)
        {
            model.Resources = new List<RoleResourceVm>();
            var apiResources = _apiResourceService.List();
            foreach (var apiResource in apiResources)
            {
                var resource = new RoleResourceVm { ResourceName = apiResource.Name };
                resource.RoleName = model.RoleName;
                await InitialiseScopes(resource,role);
                model.Resources.Add(resource);                
            }
        }

        private async Task InitialiseScopes(RoleResourceVm resource, IdentityRole role)
        {
            resource.ResourceClaims = new List<RoleResourceScopeStruct>();
            var apiResource = _apiResourceService.GetByName(resource.ResourceName);
            var apiScope = apiResource.Scopes.FirstOrDefault();
            foreach (var claim in apiScope.UserClaims)
            {
                var resourceScope = new RoleResourceScopeStruct { ScopeName = claim };
                resourceScope.Assigned =await IsResourceScopeAssigned(role, resource.ResourceName, claim);
                resource.ResourceClaims.Add(resourceScope);
            }
        }

        private async Task<bool> IsResourceScopeAssigned(IdentityRole role, string resourceName, string claim)
        {
            var roleClaims = await _roleService.GetClaims(role);
            foreach (var roleClaim in roleClaims)
            {
                if (roleClaim.Type == resourceName && roleClaim.Value == claim)
                    return true;
            }

            return false;
        }
    }
}
