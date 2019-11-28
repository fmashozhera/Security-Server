using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SampleClient.Controllers
{

    public class AccountController : ControllerBase
    {
        [Authorize]
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}