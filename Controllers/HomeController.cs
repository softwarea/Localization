using Localization.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Localization.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new IndexViewModel();

            return View(model);
        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login(IndexViewModel model)
        {
            if (!ModelState.IsValid)
                return View("index", model);

            var claims = new List<Claim>() {
                new Claim("UserName", model.Name),
                new Claim("UserCulture", "de-DE")   // start with German after login
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToRoute("Private");
        }

        [HttpPost("name/change", Name = "ChangeName")]
        public async Task<IActionResult> ChangeName(ChangeNameViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToRoute("Private");

            var principal = this.User as ClaimsPrincipal;
            var identity = principal.Identity as ClaimsIdentity;

            var oldClaim = (from c in principal.Claims
                            where c.Type == "UserName"
                            select c).FirstOrDefault();
            identity.RemoveClaim(oldClaim);

            var newClaim = new Claim("UserName", model.UserName);
            identity.AddClaim(newClaim);

            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToRoute("Private");
        }

        [HttpPost("culture/change", Name = "ChangeCulture")]
        public async Task<IActionResult> ChangeCulture(ChangeCultureViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToRoute("Private");

            var principal = this.User as ClaimsPrincipal;
            var identity = principal.Identity as ClaimsIdentity;

            var oldClaim = (from c in principal.Claims
                            where c.Type == "UserCulture"
                            select c).FirstOrDefault();
            identity.RemoveClaim(oldClaim);

            var newClaim = new Claim("UserCulture", model.CultureName);
            identity.AddClaim(newClaim);

            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToRoute("Private");
        }

        [HttpGet("logout", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("index");
        }

        [Authorize]
        [HttpGet("private", Name = "Private")]
        public IActionResult Private()
        {
            var user = this.User;
            var userNameClaim = user.Claims.First(c => c.Type == "UserName");
            var cultureNameClaim = user.Claims.First(c => c.Type == "UserCulture");

            var model = new PrivateViewModel()
            {
                UserName = userNameClaim.Value,
                CultureName = cultureNameClaim.Value
            };

            return View(model);
        }
    }
}
