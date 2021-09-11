using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Localization.Misc
{
    public class MyCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return Task.FromResult((ProviderCultureResult)null);
            }

            var principal = httpContext.User as ClaimsPrincipal;
            var claim = principal.Claims.FirstOrDefault(c => c.Type == "UserCulture");

            var requestCulture = new ProviderCultureResult(claim.Value);

            return Task.FromResult(requestCulture);
        }
    }
}
