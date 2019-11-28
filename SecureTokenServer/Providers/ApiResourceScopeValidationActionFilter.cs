using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Providers
{
    public class ApiResourceScopeValidationActionFilter : ActionFilterAttribute
    {
        //RequestServices.GetService
        private IMapper _mapper;
        private IApiResourceService _apiResourceService;
        private bool _newService;

        public ApiResourceScopeValidationActionFilter(bool newService)
        {
            _newService = newService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _mapper = (IMapper)context.HttpContext.RequestServices.GetService(typeof(IMapper));
            _apiResourceService = (IApiResourceService)context.HttpContext.RequestServices.GetService(typeof(IApiResourceService));
            var model = (ApiResourceScopeViewModel)context.ActionArguments["apiResourceScopeViewModel"];
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.DisplayName))
            {
                var errorMessage = _apiResourceService.ValidateScopeBusinessLogic(_mapper.Map<IdentityServer4.Models.Scope>(model), _newService,model.ApiResourceName);
                if (!string.IsNullOrEmpty(errorMessage))
                    context.ModelState.AddModelError("", errorMessage);
            }
        }
    }
}
