using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureTokenServer.Interfaces;
using SecureTokenServer.Models;
using SecureTokenServer.Providers;
using SecureTokenServer.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureTokenServer.Controllers
{
    public class RolesController : Controller
    {
        private readonly RolesService _roleService;
        private readonly IMapper _mapper;
        private readonly ManageRoleVmFactory _manageRoleVmFactory;
        public RolesController(RolesService roleService,
             IMapper mapper,
             ManageRoleVmFactory manageRoleVmFactory)
        {
            _roleService = roleService;
            _mapper = mapper;
            _manageRoleVmFactory = manageRoleVmFactory;
        }
        public IActionResult Index()
        {
            var roles = _roleService.List().Select(t => _mapper.Map<RoleIndexVm>(t));
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleIndexVm model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var role = new IdentityRole(model.Name);
                    await _roleService.Add(role);
                    return RedirectToAction("Index");
                }
                catch (BusinessRuleException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string roleId)
        {
            var roleVm = _mapper.Map<RoleIndexVm>(await _roleService.Get(roleId));
            return View(roleVm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleIndexVm model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var role = _mapper.Map<IdentityRole>(model);
                    await _roleService.Update(role);
                    return RedirectToAction("Index");
                }
                catch (BusinessRuleException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Manage(string roleId)
        {
            var manageRoleVm = await _manageRoleVmFactory.CreateModel(roleId);
            ViewData["RoleName"] = manageRoleVm.RoleName;
            ViewData["RoleId"] = manageRoleVm.RoleId;
            return View(manageRoleVm.Resources);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(string roleId, string resourceName, string scopeName, bool assignClaim)
        {
            var role = await _roleService.Get(roleId);

            if (assignClaim)
                await _roleService.AddClaim(role, resourceName, scopeName);
            else
                await _roleService.DeleteClaim(role, resourceName, scopeName);

            return Ok();
        }
    }
}