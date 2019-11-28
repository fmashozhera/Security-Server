using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc.Filters;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Models;
using System;
using System.Linq;
using System.Text;

namespace SecureTokenServer.Providers
{
    public class ApiResourceValidationActionFilter : ActionFilterAttribute
    {
        //RequestServices.GetService
        private IMapper _mapper;
        private IApiResourceService _apiResourceService;
        private bool _newService;

        public ApiResourceValidationActionFilter(bool newService)
        {
            _newService = newService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _mapper = (IMapper)context.HttpContext.RequestServices.GetService(typeof(IMapper));
            _apiResourceService = (IApiResourceService)context.HttpContext.RequestServices.GetService(typeof(IApiResourceService));
            var model = (RegisterApiResourceViewModel)context.ActionArguments["registerApiResourceViewModel"];
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.DisplayName))
            {
                var errorMessage = _apiResourceService.ValidateBusinessLogic(_mapper.Map<IdentityServer4.Models.ApiResource>(model),_newService);
                if (!string.IsNullOrEmpty(errorMessage))
                    context.ModelState.AddModelError("", errorMessage);
            }           
        }
    }
}
