using IdentityServer4.Models;
using System.Collections.Generic;

namespace SecureTokenServer.Interfaces
{
    public interface IApiResourceService
    {
        void SetDefaults(ApiResource resource);
        void Add(ApiResource resource);
        void Update(ApiResource resource);
        IEnumerable<ApiResource> List();
        ApiResource GetByName(string resourceName);
        void ManageScopes(ApiResource resource);
        void Commit();
        void AddScope(Scope scope,string resourceName);
        ApiResource GetById(int id);
        void AddClaim(string resourceName, string scopeName, string claimType);
        void DeleteClaim(string resourceName, string scopeName, string claimType);
        void DeleteScope(string resourceName, string scopeName);
        string ValidateBusinessLogic(ApiResource resource,bool newService);
        string ValidateScopeBusinessLogic(Scope resourceScope, bool newScope,string resourceName);
        void Delete(string resourceName);
    }
}
