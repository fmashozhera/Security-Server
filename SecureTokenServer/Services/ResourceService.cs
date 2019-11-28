using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Providers;

namespace SecureTokenServer.Services
{
    public class ResourceService : IApiResourceService
    {
        private readonly ConfigurationDbContext _context;
        public ResourceService(ConfigurationDbContext context)
        {
            _context = context;
        }
        public void Add(IdentityServer4.Models.ApiResource apiResource)
        {
            ValidateBusinessLogic(apiResource,true);
            _context.ApiResources.Add(apiResource.ToEntity());
        }

        public string ValidateBusinessLogic(IdentityServer4.Models.ApiResource apiResource,bool newService)
        {           
            var domainErrors = new StringBuilder();
            if (newService)
            {
                if (!String.IsNullOrEmpty(apiResource.Name) && _context.ApiResources.Any(t => t.Name.ToUpper().Trim() == apiResource.Name.ToUpper().Trim()))
                {
                    domainErrors.Append("There is already a resource with same name. Please use a different name or update the existing resource." + Environment.NewLine);
                }

                if (!String.IsNullOrEmpty(apiResource.DisplayName) && _context.ApiResources.Any(t => t.DisplayName.ToUpper().Trim() == apiResource.DisplayName.ToUpper().Trim() && t.Id != apiResource.ToEntity().Id))
                {
                    domainErrors.Append("There is already a resource with same display name. Please use a different display name or update the existing resource.");
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(apiResource.DisplayName) && _context.ApiResources.Any(t => t.DisplayName.ToUpper().Trim() == apiResource.DisplayName.ToUpper().Trim() && t.Name != apiResource.Name))
                {
                    domainErrors.Append("There is already a resource with same display name. Please use a different display name or update the existing resource.");
                }
            }

            return domainErrors.ToString();           
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IdentityServer4.Models.ApiResource GetByName(string resourceName)
        {
            var resource = _context.ApiResources
                .Include(t => t.Scopes)
                .ThenInclude(scope => scope.UserClaims)
                .FirstOrDefault(t => t.Name.ToUpper().Trim() == resourceName.ToUpper().Trim());

            var resourceModel = resource.ToModel();
            foreach (var scope in resourceModel.Scopes)
            {
                scope.UserClaims = resource.Scopes.FirstOrDefault(t => t.Name == scope.Name).UserClaims.Select(t => t.Type).ToList();
            } 
            return resourceModel;            
        }

        public IEnumerable<IdentityServer4.Models.ApiResource> List()
        {
            return _context.ApiResources.Select(t=>t.ToModel()).ToList();
        }

        public void ManageScopes(IdentityServer4.Models.ApiResource resource)
        {
            var savedResource = _context.ApiResources.FirstOrDefault(t => t.Name == resource.Name);

            var deletedScopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope>();
            var updatedScopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope>();
            foreach (var scope in savedResource.Scopes)
            {
                if (!resource.Scopes.Any(t => t.Name.ToUpper().Trim() == scope.Name.Trim().ToUpper()))
                {
                    deletedScopes.Add(scope);
                }
                else
                {
                    updatedScopes.Add(scope);
                }
            }

            var addedScopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope>();
            foreach (var scopeModel in resource.Scopes)
            {
                if (!savedResource.Scopes.Any(t => t.Name.ToUpper().Trim() == scopeModel.Name.ToUpper().Trim()))
                {
                    addedScopes.Add(new IdentityServer4.EntityFramework.Entities.ApiScope {
                        Name = scopeModel.Name,
                        Description = scopeModel.Description,
                        DisplayName = scopeModel.DisplayName,
                        UserClaims = scopeModel.UserClaims.Select(t=>new IdentityServer4.EntityFramework.Entities.ApiScopeClaim {
                            Type = t
                        }).ToList()
                    });;
                }
            }

            DeleteRemovedScopes(deletedScopes,savedResource);
            AddAddedScopes(addedScopes, savedResource);
            UpdateUpdatedScopes(updatedScopes, savedResource);
        }



        private void UpdateUpdatedScopes(List<ApiScope> updatedScopes, IdentityServer4.EntityFramework.Entities.ApiResource resource)
        {
            foreach (var scopeModel in updatedScopes)
            {
                var addedClaims = new List<ApiScopeClaim>();
                var deletedClaims = new List<ApiScopeClaim>();
                var savedScope = resource.Scopes.FirstOrDefault(t => t.Name.ToUpper().Trim() == scopeModel.Name.Trim().ToUpper());
                if (savedScope!=null){
                    savedScope.DisplayName = scopeModel.DisplayName;
                    savedScope.Description = scopeModel.Description;

                    foreach (var claim in savedScope.UserClaims)
                    {
                        if (!scopeModel.UserClaims.Any(t => t.Type.ToUpper().Trim() == claim.Type.ToUpper().Trim()))
                        {
                            deletedClaims.Add(claim);
                        }
                    }

                    foreach (var claim in scopeModel.UserClaims)
                    {
                        if (!savedScope.UserClaims.Any(t => t.Type.ToUpper().Trim() == claim.Type.ToUpper().Trim()))
                        {
                            addedClaims.Add(claim);
                        }
                    }

                    foreach (var claim in addedClaims)
                    {
                        savedScope.UserClaims.Add(claim);
                    }

                    foreach (var claim in deletedClaims)
                    {
                        savedScope.UserClaims.Remove(claim);
                    }
                }
            }
        }

        private void AddAddedScopes(List<ApiScope> addedScopes, IdentityServer4.EntityFramework.Entities.ApiResource resource)
        {
            foreach (var addedScope in addedScopes)
            {
                resource.Scopes.Add(addedScope);
            }
        }

        private void DeleteRemovedScopes(List<ApiScope> deletedScopes, IdentityServer4.EntityFramework.Entities.ApiResource resource)
        {
            foreach (var deletedScope in deletedScopes)
            {
                resource.Scopes.Remove(deletedScope);
            }            
        }

        public void SetDefaults(IdentityServer4.Models.ApiResource resource)
        {
            throw new System.NotImplementedException();
        }

        public void Update(IdentityServer4.Models.ApiResource resource)
        {
            var savedApiResource = _context.ApiResources.FirstOrDefault(t => t.Name == resource.Name);
            if (_context.ApiResources.Any(t => t.Name.ToUpper().Trim() == resource.Name.ToUpper() && t.Id != savedApiResource.Id))
                throw new BusinessRuleException("The api name you have entered is already in use.");

            savedApiResource.Name = resource.Name;
            savedApiResource.Description = resource.Description;
            savedApiResource.DisplayName = resource.DisplayName;
        }

        public void AddScope(Scope scope, string resourceName)
        {
            var resource = _context.ApiResources.Include("Scopes").FirstOrDefault(t=>t.Name.ToUpper().Trim()==resourceName.ToUpper().Trim());
            if (resource != null)
            {
                var savedScope = resource.Scopes.FirstOrDefault(t => t.Name.ToUpper().Trim() == scope.Name.ToUpper().Trim());
                if (savedScope == null)
                {
                    var apiScope = new ApiScope
                    {
                        Name = scope.Name,
                        Description = scope.Description,
                        DisplayName = scope.DisplayName,
                        ApiResourceId = resource.Id
                    };

                    resource.Scopes.Add(apiScope);
                }
                else
                {
                    savedScope.Description = scope.Description;
                    savedScope.DisplayName = scope.DisplayName;
                }
            }
        }

        public IdentityServer4.Models.ApiResource GetById(int id)
        {
            return _context.ApiResources.Find(id).ToModel();
        }

        public void AddClaim(string resourceName, string scopeName, string claimType)
        {
            var resource = _context.ApiResources
                .Include(t=>t.Scopes)
                .ThenInclude(scope=>scope.UserClaims)
                .FirstOrDefault(t => t.Name.ToUpper().Trim() == resourceName.ToUpper().Trim());

            var resourceScope = resource.Scopes.FirstOrDefault(t => t.Name == scopeName);
            resourceScope.UserClaims.Add(new ApiScopeClaim {Type=claimType });
        }

        public void DeleteClaim(string resourceName, string scopeName, string claimType)
        {
            var resource = _context.ApiResources
                .Include(t => t.Scopes)
                .ThenInclude(scope => scope.UserClaims)
                .FirstOrDefault(t => t.Name.ToUpper().Trim() == resourceName.ToUpper().Trim());

            var deletedClaim = resource.Scopes.FirstOrDefault(t => t.Name == scopeName).UserClaims.FirstOrDefault(t => t.Type == claimType);
            if(deletedClaim!=null)
                resource.Scopes.FirstOrDefault(t => t.Name == scopeName).UserClaims.Remove(deletedClaim);         
        }

        public void DeleteScope(string resourceName, string scopeName)
        {
            var resource = _context.ApiResources
                .Include(t => t.Scopes)                
                .FirstOrDefault(t => t.Name.ToUpper().Trim() == resourceName.ToUpper().Trim());

            var deletedScope = resource.Scopes.FirstOrDefault(t => t.Name == scopeName);
            if (deletedScope != null)
                resource.Scopes.Remove(deletedScope);
        }

        public void Delete(string resourceName)
        {
            var deletedResource = _context.ApiResources.FirstOrDefault(t => t.Name == resourceName);
            if (deletedResource != null)
            {
                _context.ApiResources.Remove(deletedResource);
            }
        }

        public string ValidateScopeBusinessLogic(Scope resourceScope, bool newScope,string resourceName)
        {
            var domainErrors = new StringBuilder();
            var apiResource = _context.ApiResources.Include(t=>t.Scopes).FirstOrDefault(t => t.Name == resourceName);
            if (newScope)
            {
                if (!String.IsNullOrEmpty(resourceScope.Name) && apiResource.Scopes.Any(t => t.Name.ToUpper().Trim() == resourceScope.Name.ToUpper().Trim()))
                {
                    domainErrors.Append("There is already a scope with same name. Please use a different name or update the existing scope." + Environment.NewLine);
                }

                if (!String.IsNullOrEmpty(resourceScope.DisplayName) && apiResource.Scopes.Any(t => t.DisplayName.ToUpper().Trim() == resourceScope.DisplayName.ToUpper().Trim() && t.Name.ToUpper().Trim()!=resourceScope.Name.ToUpper().Trim()))
                {
                    domainErrors.Append("There is already a scope with same display name. Please use a different display name or update the existing scope.");
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(resourceScope.DisplayName) && apiResource.Scopes.Any(t => t.DisplayName.ToUpper().Trim() == resourceScope.DisplayName.ToUpper().Trim() && t.Name != resourceScope.Name))
                {
                    domainErrors.Append("There is already a scope with same display name. Please use a different display name or update the existing scope.");
                }
            }

            return domainErrors.ToString();
        }
    }
}
