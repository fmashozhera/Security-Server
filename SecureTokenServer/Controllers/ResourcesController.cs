using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Models;
using SecureTokenServer.Providers;
using System;
using System.Linq;
using System.Text;

namespace SecureTokenServer.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly IApiResourceService _apiResourceService;
        private readonly IMapper _mapper;

        public ResourcesController(
            IApiResourceService apiResourceService,
            IMapper mapper
            )
        {
            _apiResourceService = apiResourceService;
            _mapper = mapper;
        }
        // GET: Resources
        public ActionResult Index()
        {
            return View(_apiResourceService.List());
        }
        
        // GET: Resources/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: Resources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApiResourceValidationActionFilter(true)]
        public ActionResult Register(RegisterApiResourceViewModel registerApiResourceViewModel)
        {          
            if (ModelState.IsValid)
            {
                var apiResource = _mapper.Map<IdentityServer4.Models.ApiResource>(registerApiResourceViewModel);
                _apiResourceService.Add(apiResource);
                _apiResourceService.Commit();
                return RedirectToAction("Index");
            }

            return View(registerApiResourceViewModel);
        }

        // GET: Resources/Edit/5
        public ActionResult Edit(string resourceName)
        {
            return View(_mapper.Map<RegisterApiResourceViewModel>(_apiResourceService.GetByName(resourceName)));
        }

        // POST: Resources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApiResourceValidationActionFilter(false)]
        public ActionResult Edit(RegisterApiResourceViewModel registerApiResourceViewModel)
        {
            if (ModelState.IsValid)
            {
                _apiResourceService.Update(_mapper.Map<IdentityServer4.Models.ApiResource>(registerApiResourceViewModel));
                _apiResourceService.Commit();
                return RedirectToAction("Index");
            }
            return View(registerApiResourceViewModel);
        }

        public ActionResult Manage(string resourceName)
        {
            var manageApiResourceViewModel = _mapper.Map<ManageApiResourceViewModel>(_apiResourceService.GetByName(resourceName));
            return View("ManageResource", manageApiResourceViewModel);
        }    

        public ActionResult EditResourceScope(string resourceName, string scopeName)
        {
            var manageApiResourceViewModel = _mapper.Map<ManageApiResourceViewModel>(_apiResourceService.GetByName(resourceName));
            manageApiResourceViewModel.ActiveScopeViewModel = manageApiResourceViewModel.Scopes.FirstOrDefault(t => t.Name == scopeName);
            return View("ManageResource", manageApiResourceViewModel);
        }

        // POST: Resources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(ManageApiResourceViewModel manageApiResourceViewModel)
        {
            if (ModelState.IsValid)
            {
                _apiResourceService.ManageScopes(_mapper.Map<IdentityServer4.Models.ApiResource>(manageApiResourceViewModel));
                _apiResourceService.Commit();
            }
            return View("ManageResource", new { resourceName = manageApiResourceViewModel.Name });
        }

        public IActionResult AddScope(string resourceName, string scopeName)
        {
            var apiResourceScopeViewModel = new ApiResourceScopeViewModel();
            if (!string.IsNullOrEmpty(scopeName))
            {
                var resource = _apiResourceService.GetByName(resourceName);
                apiResourceScopeViewModel = _mapper.Map<ApiResourceScopeViewModel>(resource.Scopes.FirstOrDefault(t => t.Name == scopeName));
                apiResourceScopeViewModel.ApiResourceName = resource.Name;
                return PartialView("_AddScopePartial", apiResourceScopeViewModel);
            }
            else
            {
                apiResourceScopeViewModel = new ApiResourceScopeViewModel { ApiResourceName = resourceName };
                return PartialView("_AddScopePartial", apiResourceScopeViewModel);
            }
        }

        public IActionResult ManageScope(string resourceName, string scopeName)
        {
            var apiResourceScopeViewModel = new ApiResourceScopeViewModel();
            var resource = _apiResourceService.GetByName(resourceName);
            apiResourceScopeViewModel = _mapper.Map<ApiResourceScopeViewModel>(resource.Scopes.FirstOrDefault(t => t.Name == scopeName));
            apiResourceScopeViewModel.ApiResourceName = resource.Name;
            return PartialView("_ManageScopePartial", apiResourceScopeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApiResourceScopeValidationActionFilter(true)]
        public ActionResult AddScope(ApiResourceScopeViewModel apiResourceScopeViewModel)
        {
            if (ModelState.IsValid)
            {
                _apiResourceService.AddScope(_mapper.Map<Scope>(apiResourceScopeViewModel), apiResourceScopeViewModel.ApiResourceName);
                _apiResourceService.Commit();
                return Ok(new { success = true});
            }
            return Ok(new { success = false, errorMessage = GetModelErrors(ModelState.Values) });
        }

        private object GetModelErrors(ModelStateDictionary.ValueEnumerable values)
        {
            var errorBuilder = new StringBuilder();
            foreach (var item in values)
            {
                foreach (var error in item.Errors)
                {
                    errorBuilder.Append(error.ErrorMessage+";");
                }                
            }
            return errorBuilder.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddClaim(ApiResourceScopeViewModel apiResourceScopeViewModel)
        {
            _apiResourceService.AddClaim(apiResourceScopeViewModel.ApiResourceName, apiResourceScopeViewModel.Name, apiResourceScopeViewModel.ActiveClaim.Type);
            _apiResourceService.Commit();
            return Ok(apiResourceScopeViewModel.ActiveClaim.Type);
        }
                       
        [HttpPost]
        public ActionResult RemoveClaim(string apiResourceName,string scopeName,string claimType)
        {
            _apiResourceService.DeleteClaim(apiResourceName, scopeName, claimType);
            _apiResourceService.Commit();
            return Ok();
        }

        public ActionResult RemoveScope(string apiResourceName, string name)
        {
            _apiResourceService.DeleteScope(apiResourceName, name);
            _apiResourceService.Commit();
            return Ok();
        }

        public ActionResult ConfirmScopeRemoval(string resourceName, string scopeName)
        {
            var apiResourceScopeViewModel = new ApiResourceScopeViewModel();
            var resource = _apiResourceService.GetByName(resourceName);
            apiResourceScopeViewModel = _mapper.Map<ApiResourceScopeViewModel>(resource.Scopes.FirstOrDefault(t => t.Name == scopeName));
            apiResourceScopeViewModel.ApiResourceName = resource.Name;
            return PartialView("_DeleteScopePartial", apiResourceScopeViewModel);
        }

        // GET: Resources/Delete/5
        public ActionResult Delete(string resourceName)
        {                  
            var manageApiResourceViewModel = _mapper.Map<ManageApiResourceViewModel>(_apiResourceService.GetByName(resourceName));                     
            return PartialView("_DeleteResourcePartial", manageApiResourceViewModel);
        }

        // POST: Resources/Delete/5
        [HttpPost]       
        public ActionResult RemoveResource(ManageApiResourceViewModel model)
        {
            _apiResourceService.Delete(model.Name);
            _apiResourceService.Commit();
            return RedirectToAction("Index");
        }
    }
}