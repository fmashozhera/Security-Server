using AutoMapper;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using SecureTokenServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecureTokenServer.Providers
{
    public static class AutomaperConfig
    {
        public static IMapper BuildMappings()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterClientViewModel, Client>()
                //this implementation works with the assumption that there is only one redirect uri
                .ForMember(t => t.RedirectUris, opt => { opt.MapFrom(src => new List<string> { src.RedirectUri }); })
                //this implementation works with the assumption that there is only one redirect uri
                .ForMember(t => t.PostLogoutRedirectUris, opt => { opt.MapFrom(src => new List<string> { src.PostLogoutRedirectUri }); })
                .ForMember(t => t.AllowedScopes, opt =>
                {
                    opt.MapFrom(src => src.AllowedScopes.Select(t => t.ToString()));
                });


                cfg.CreateMap<Client, RegisterClientViewModel>()
                //this implementation works with the assumption that there is only one redirect uri
                .ForMember(t => t.RedirectUri, opt => { opt.MapFrom(src => src.RedirectUris.FirstOrDefault()); })
                //this implementation works with the assumption that there is only one redirect uri
                .ForMember(t => t.PostLogoutRedirectUri, opt => { opt.MapFrom(src => src.PostLogoutRedirectUris.FirstOrDefault()); });


                cfg.CreateMap<RegisterApiResourceViewModel, ApiResource>();
                cfg.CreateMap<IdentityRole, RoleIndexVm>();
                //.ForMember(t=>t.Id,opt=> { opt.MapFrom(src => src.Id); })
                //.ForMember(t => t.Name, opt => { opt.MapFrom(src => src.Name); });

                cfg.CreateMap<ApiResource, RegisterApiResourceViewModel>();
                cfg.CreateMap<IdentityServer4.Models.ApiResource, ManageApiResourceViewModel>().ConstructUsing((apiResource) => ApiResourceToManageApiResourceViewModelConverter(apiResource));
                cfg.CreateMap<IdentityServer4.Models.Client, ManageClientVm>().ConstructUsing((client) => ClientToManageClientVm(client));
                cfg.CreateMap<ManageApiResourceViewModel, IdentityServer4.Models.ApiResource>().ConstructUsing((apiResourceViewModel) => ManageApiResourceViewModelToApiResourceConverter(apiResourceViewModel));
                cfg.CreateMap<IdentityServer4.EntityFramework.Entities.ApiScope, Scope>();

                cfg.CreateMap<ScopeClaimViewModel, Scope>();
                cfg.CreateMap<Scope, ApiResourceScopeViewModel>();




                cfg.ValidateInlineMaps = false;
            });

            return config.CreateMapper();
        }

        private static ManageClientVm ClientToManageClientVm(Client client)
        {
            var manageClientVm = new ManageClientVm();
            manageClientVm.ClientId = client.ClientId;
            manageClientVm.ClientName = client.ClientName;
            manageClientVm.ClientUri = client.ClientUri;
            manageClientVm.Description = client.Description;
            manageClientVm.PostLogoutRedirectUri = client.PostLogoutRedirectUris.FirstOrDefault();          
            manageClientVm.ClientResources = GetClientScopes(manageClientVm,client.AllowedScopes); 

            return manageClientVm;
        }     

        private static ICollection<ClientResourceVm> GetClientScopes(ManageClientVm manageClientVm,ICollection<string> allowedScopes)
        {
            var clientScopes = new List<ClientResourceVm>();
            foreach (var scope in allowedScopes)
            {              
                clientScopes.Add(new ClientResourceVm
                {
                    ResourceName = scope,                   
                }); 
            }
            return clientScopes;
        }

        private static IdentityServer4.Models.ApiResource ManageApiResourceViewModelToApiResourceConverter(ManageApiResourceViewModel apiResourceViewModel)
        {
            var apiResource = new IdentityServer4.EntityFramework.Entities.ApiResource();
            apiResource.Description = apiResourceViewModel.Description;
            apiResource.Name = apiResourceViewModel.Name;
            apiResource.DisplayName = apiResourceViewModel.DisplayName;
            
            apiResource.Scopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope>();
            foreach (var scopeModel in apiResourceViewModel.Scopes)
            {
                var scope = new IdentityServer4.EntityFramework.Entities.ApiScope
                {
                    Name = scopeModel.Name,
                    Description = scopeModel.Description,
                    DisplayName = scopeModel.DisplayName,
                    UserClaims = new List<IdentityServer4.EntityFramework.Entities.ApiScopeClaim>()
                };

                foreach (var claim in scopeModel.UserClaims)
                {
                    scope.UserClaims.Add(new IdentityServer4.EntityFramework.Entities.ApiScopeClaim {
                        Type = claim
                    });
                }

                apiResource.Scopes.Add(scope);
            }

            return apiResource.ToModel();
        }

        private static ManageApiResourceViewModel ApiResourceToManageApiResourceViewModelConverter(IdentityServer4.Models.ApiResource apiResource)
        {
            var manageApiResourceViewModel = new ManageApiResourceViewModel();
            manageApiResourceViewModel.Name = apiResource.Name;
            manageApiResourceViewModel.Description = apiResource.Description;
            manageApiResourceViewModel.DisplayName = apiResource.DisplayName;
            

            var scopes = new List<ApiResourceScopeViewModel>();
            foreach (var scope in apiResource.Scopes)
            {
                var scopeModel = new ApiResourceScopeViewModel();

                scopeModel.Name = scope.Name;
                scopeModel.Description = scope.Description;
                scopeModel.DisplayName = scope.DisplayName;
                scopeModel.UserClaims = new List<string>();
                
                foreach (var claim in scope.UserClaims)
                {
                    var claimModel = new ScopeClaimViewModel();
                    claimModel.Type = claim;                    
                }
            }
            manageApiResourceViewModel.Scopes = scopes;

            return manageApiResourceViewModel;
        }
    }

   
}
