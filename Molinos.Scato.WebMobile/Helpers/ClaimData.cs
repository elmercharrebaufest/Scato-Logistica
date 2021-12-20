using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.WebMobile.Helpers
{
    public static class ClaimData
    {
        public static void AddUpdateUserClaim(this IPrincipal currentPrincipal, string key, string value)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return;
            }
            var existingClaim = identity.Claims.SingleOrDefault(c => c.Type == key);
            if (existingClaim != null)
            {
                identity.RemoveClaim(existingClaim);
            }
            identity.AddClaim(new Claim(key, value));
        }

        public static Claim GetUserClaim(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return null;
            }
            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim;
        }
    }
}
