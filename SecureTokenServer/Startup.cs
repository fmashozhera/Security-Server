// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using SecureTokenServer.Data;
using SecureTokenServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using System.Linq;
using IdentityServer4;
using SecureTokenServer.Providers;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Services;
using AutoMapper;

namespace SecureTokenServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAspNetIdentity(services);
            ConfigureAutomapper(services);
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            //configure identityserver4 in database operational and configuration stores
            ConfigureIdentityServer(services);

            //configure custom services
            RegisterCustomServices(services);
        }

        private void ConfigureAutomapper(IServiceCollection services)
        {
            services.AddTransient<IMapper>((src)=> { return AutomaperConfig.BuildMappings(); });
        }

        private void RegisterCustomServices(IServiceCollection services)
        {
            services.AddTransient<IClientService,ClientService>();
            services.AddTransient<RolesService, RolesService>();
            services.AddTransient<IApiResourceService, ResourceService>();
            services.AddTransient<ManageRoleVmFactory, ManageRoleVmFactory>();            
            var mapper = AutomaperConfig.BuildMappings();
        }

        private void ConfigureMicrosoftAuthentication(IServiceCollection services)
        {
            services.AddAuthentication()
               .AddMicrosoftAccount(options => {
                   options.AuthorizationEndpoint = "https://login.microsoftonline.com/68792612-0f0e-46cb-b16a-fcb82fd80cb1/oauth2/v2.0/authorize";
                   options.ClientId = "aba1dc0b-1892-4395-be4c-69bbcbee9d00";
                   options.ClientSecret = "ZgNxsb?3y]XBv6T1ibeELGoQP:Ln7yy/";
                   options.CallbackPath = "/signin-microsoft2";
                   options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
               });
        }

        private void ConfigureAspNetIdentity(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        private void ConfigureIdentityServer(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer()           
            // this adds the config data from DB (clients, resources)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionString,sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseSqlServer(connectionString,sql => sql.MigrationsAssembly(migrationsAssembly));
                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
            })
            //this adds support for ASPNET core Identity
            .AddAspNetIdentity<ApplicationUser>();

            ConfigureMicrosoftAuthentication(services);

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app);
            AutomaperConfig.BuildMappings();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcWithDefaultRoute();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                var clients = context.Clients.ToList();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}