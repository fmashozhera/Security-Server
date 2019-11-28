using System.Collections.Generic;

namespace SecureTokenServer.Models
{
    public class ManageRoleVm
    {
        public string   RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleResourceVm> Resources { get; set; }
    }

    public class RoleResourceVm
    {
        public string RoleName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceId { get; set; }
        public List<RoleResourceScopeStruct> ResourceClaims { get; set; }
    }

    public struct RoleResourceScopeStruct
    {
        public bool Assigned { get; set; }
        public string ScopeName { get; set; }
    }    
}
