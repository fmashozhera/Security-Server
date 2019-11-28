using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using SecureTokenServer.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SecureTokenServer.Services
{
    public class ClientService : IClientService
    {
        private ConfigurationDbContext _context;
        public ClientService(ConfigurationDbContext context)
        {
            _context = context;
        }

        public void Add(IdentityServer4.Models.Client client)
        {
            var clientEntity = client.ToEntity();
            clientEntity.ClientUri = client.ClientUri;
            _context.Clients.Add(clientEntity);            
        }

        public void AssignResource(string clientName, string resourceName)
        {
            var client = _context.Clients.Include("AllowedScopes").FirstOrDefault(t => t.ClientName == clientName);
            client.AllowedScopes.Add(new ClientScope {
                ClientId = client.Id,
                Client = client,
                Scope=resourceName,                
            });          
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public IdentityServer4.Models.Client Get(string clientId)
        {
            var client = _context.Clients
            .Include("RedirectUris")
            .Include("PostLogoutRedirectUris")  
            .Include("AllowedScopes")
            .FirstOrDefault(t => t.ClientId == clientId);

            var clientUid = client.ClientUri;
            var clientModel = client.ToModel();
            return clientModel;
        }

        public ICollection<string> GetAssignedResource(string clientName)
        {
            var client = _context.Clients.Include("AllowedScopes").FirstOrDefault(t => t.ClientName == clientName);
            var assignedResources = client.AllowedScopes?.Select(t=>t.Scope).ToList() ;
            return assignedResources;
        }

        public IEnumerable<IdentityServer4.Models.Client> List()
        {
            return _context.Clients.Include("RedirectUris").Include("PostLogoutRedirectUris").Select(t => t.ToModel());
        }

        public void SetDefaults(IdentityServer4.Models.Client client)
        {
            client.AllowedGrantTypes = GrantTypes.Implicit;
            client.EnableLocalLogin = false;
        }

        public void UnassignResource(string clientName, string resourceName)
        {
            var client = _context.Clients.Include("AllowedScopes").FirstOrDefault(t => t.ClientName == clientName);
            client.AllowedScopes.Remove(client.AllowedScopes.FirstOrDefault(t => t.Scope == resourceName));
        }

        public void Update(IdentityServer4.Models.Client client)
        {
            var savedClient = _context.Clients
                .Include("RedirectUris")
                .Include("PostLogoutRedirectUris")
                .FirstOrDefault(t=>t.ClientId==client.ClientId);

            if (savedClient != null)
            {
                savedClient.ClientName = client.ClientName;
                savedClient.ClientUri = client.ClientUri;
                savedClient.Description = client.Description;
                ConvertUris(savedClient,client);
                savedClient.PostLogoutRedirectUris = ConvertPostLogoutRedirectUris(client.PostLogoutRedirectUris);
            }
        }

        private List<ClientPostLogoutRedirectUri> ConvertPostLogoutRedirectUris(ICollection<string> postLogoutRedirectUris)
        {
            var clientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri>();
            foreach (var postLogoutRedirectUri in postLogoutRedirectUris)
            {
                clientPostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = postLogoutRedirectUri });
            }
            return clientPostLogoutRedirectUris;
        }

        private void ConvertUris(IdentityServer4.EntityFramework.Entities.Client savedclient,IdentityServer4.Models.Client client)
        {
            var removedUris = new List<ClientRedirectUri>();
            var addedUris = new List<ClientRedirectUri>();
            foreach (var redirectUri in savedclient.RedirectUris)
            {                
                if(!client.RedirectUris.Any(t=>t == redirectUri.RedirectUri))
                {
                    removedUris.Add(redirectUri);
                }               
            }

            foreach (var redirectUri in client.RedirectUris)
            {
                if (!savedclient.RedirectUris.Any(t => t.RedirectUri == redirectUri))
                {
                    savedclient.RedirectUris.Add(new ClientRedirectUri {RedirectUri= redirectUri });
                }
            }

            foreach (var uri in removedUris)
            {
                savedclient.RedirectUris.Remove(uri);
            }
        }
    }
}
