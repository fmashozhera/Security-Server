// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServerEntities = IdentityServer4.EntityFramework.Entities;
using SecureTokenServer.Models;
using AutoMapper;
using IdentityServer4.Models;
using SecureTokenServer.Interfaces;

namespace SecurityServer.Controllers
{
    /// <summary>
    /// This sample controller allows a user to revoke grants given to clients
    /// </summary>
    [SecurityHeaders]
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clients;
        private readonly IResourceStore _resources;
        private readonly IEventService _events;
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;

        public ClientsController(
            IIdentityServerInteractionService interaction,
            IClientStore clients,
            IResourceStore resources,
            IEventService events,
            IClientService clientService,
            IMapper mapper)
        {
            _interaction = interaction;
            _clients = clients;
            _resources = resources;
            _events = events;
            _clientService = clientService;
            _mapper = mapper;
        }

        /// <summary>
        /// Show list of grants
        /// </summary>
        [HttpGet]        
        public async Task<IActionResult> Index()
        {
            return View("Index", _clientService.List());
        }

        

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterClientViewModel registerClientViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _mapper.Map<Client>(registerClientViewModel);                
                _clientService.SetDefaults(client);
                _clientService.Add(client);
                _clientService.Commit();
                return RedirectToAction("Index");
            }

            return View(registerClientViewModel);
        }

        [HttpGet]
        public IActionResult Edit(string clientId)
        {
            var client = _clientService.Get(clientId);
            var registerClientViewModel = _mapper.Map<RegisterClientViewModel>(client);
            return View(registerClientViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegisterClientViewModel registerClientViewModel)
        {
            if (ModelState.IsValid)
            {
                var client = _mapper.Map<Client>(registerClientViewModel);
                _clientService.SetDefaults(client);
                _clientService.Update(client);
                _clientService.Commit();
                return RedirectToAction("Index");
            }
            return View(registerClientViewModel);            
        }

        [HttpGet]
        public IActionResult Manage(string clientId)
        {
            var client = _clientService.Get(clientId);
            var manageClientVm = _mapper.Map<ManageClientVm>(client);
            Initialise(manageClientVm);
            return View(manageClientVm);
        }

        private async void Initialise(ManageClientVm manageClientVm)
        {
            var resources = await _resources.GetAllResourcesAsync();
            var apiResources = resources.ApiResources;
            foreach (var resourceVm in manageClientVm.ClientResources)
            {
                var apiResource = apiResources.FirstOrDefault(t => t.Name == resourceVm.ResourceName);
                resourceVm.DisplayName = apiResource.DisplayName;
                resourceVm.ResourceDescription = apiResource.Description;               
            }
        }

        public async Task<IActionResult> GetResources(string clientName)
        {
            var resources = await _resources.GetAllResourcesAsync();
            var apiResources = resources.ApiResources.Select(t => t.Name).ToList();
            var assignableResources = new List<string>();
            var assignedResources = _clientService.GetAssignedResource(clientName);
            var assignClientResourceVm = new AssignClientResourceVm() { ClientName=clientName}; 
            foreach (var apiResource in apiResources)
            {
                if (assignedResources==null || !assignedResources.Any(t => t == apiResource))
                    assignableResources.Add(apiResource);
            }
            assignClientResourceVm.Resources = assignableResources;
            return PartialView("_AddClientResourcePartial", assignClientResourceVm);
        }

        public ActionResult AssignResource(string resourceName,string clientName)
        {
            _clientService.AssignResource(clientName, resourceName);
            _clientService.Commit();
            return Ok(new { success = true });
        }

        public async Task<ActionResult> ConfirmResourceUnassignment(string resourceName, string clientName)
        {
            var assignClientResourceVm = new ClientResourceVm();
            var resources = await _resources.GetAllResourcesAsync();
            var apiResource = resources.ApiResources.FirstOrDefault(t => t.Name == resourceName);
            assignClientResourceVm.ClientName = clientName;
            assignClientResourceVm.DisplayName = apiResource.DisplayName;
            assignClientResourceVm.ResourceDescription = apiResource.Description;
            assignClientResourceVm.ResourceName = apiResource.Name;

            return PartialView("_UnassignResourceScopePartial", assignClientResourceVm);
        }

        [HttpPost]
        public ActionResult UnassignResource(ClientResourceVm model)
        {
            _clientService.UnassignResource(model.ClientName, model.ResourceName);
            _clientService.Commit();
            return Ok(new { success = true });
        }
    }
}