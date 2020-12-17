using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RegineDesignAdmin.LogicLayer
{
    public class GoogleAuthLL
    {

        public bool IsUserAllowed(AuthenticateResult result)
        {
            try
            {
                if (result.Succeeded == false)
                {
                    return false;
                }
                var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Issuer,
                    claim.OriginalIssuer,
                    claim.Type,
                    claim.Value
                });

                List<string> ValidEmail;
                ValidEmail = new List<string>();
                ValidEmail.Add(Environment.GetEnvironmentVariable("VALID_EMAIL1"));
                ValidEmail.Add(Environment.GetEnvironmentVariable("VALID_EMAIL2"));
                foreach (var claim in claims)
                {
                    if (ValidEmail.Contains(claim.Value.ToUpper()))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
