using IdentityServer4.Models;
using System.Collections.Generic;

namespace SecureTokenServer.Interfaces
{
    public interface IClientService
    {
        void SetDefaults(Client client);
        void Add(Client client);
        void Update(Client client);
        IEnumerable<Client> List();
        Client Get(string clientId);
        ICollection<string> GetAssignedResource(string clientId);
        void AssignResource(string clientName, string resourceName);
        void UnassignResource(string clientName, string resourceName);
        void Commit();
    }
}
