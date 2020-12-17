using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RegineDesignAdmin.LogicLayer;
using RegineDesignAdmin.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RegineDesignAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GoogleAuthLL _googleAuthLL;

        public HomeController(ILogger<HomeController> logger, GoogleAuthLL googleAuthLL)
        {
            _logger = logger;
            _googleAuthLL = googleAuthLL;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (_googleAuthLL.IsUserAllowed(result))
            {
                return View("~/Views/Home/Panel.cshtml");
            }
            return View();
        }

        [HttpGet("/[action]")]
        public async Task<IActionResult> Panel()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (_googleAuthLL.IsUserAllowed(result))
            {
                return View();
            }
            return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet("/[action]/{category}")]
        public async Task<IActionResult> Category(string category)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (_googleAuthLL.IsUserAllowed(result))
            {
                ViewData["Category"] = category;
                return View();
            }
            return View("~/Views/Shared/Error.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
