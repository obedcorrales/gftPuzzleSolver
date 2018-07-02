using System.Linq;
using System.Security.Claims;

using dF.Commons.Exceptions.Resources;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Security.Constants;

namespace dF.Commons.Security.Helpers
{
    public static class dFAuthorization
    {
        public static Result CheckResourceAccess(this ClaimsPrincipal principal, Actions action, string[] resources, bool requireActionInAllResources = false)
        {
            var retVal = false;

            foreach (var resource in resources)
            {
                var rsrc = principal.Claims
                                    .FirstOrDefault(c => c.Type == dFClaimTypes.Resource && c.Value == resource);
                if (rsrc != null)
                {
                    var allowedKeyValue = rsrc.Properties.FirstOrDefault(p => p.Key == "AllowedCRUD");

                    byte allowed = 0;

                    //if (claim.Properties.Any(p => p.Key == action.ToString() && p.Value == ClaimConstants.Permissions.Allow))
                    if (byte.TryParse(allowedKeyValue.Value, out allowed) && ((allowed & (byte)action) != 0))
                    {
                        if (requireActionInAllResources)
                            retVal = true;
                        else
                            return Result.Ok();
                    }
                    else if (requireActionInAllResources)
                        return Result.Fail(string.Format(ExceptionMessages.UnauthorizedAccess_NoEnoughPermissions, resource), ErrorType.UnauthorizedAccess);
                }
                else if (requireActionInAllResources)
                    return Result.Fail(string.Format(ExceptionMessages.UnauthorizedAccess_NoEnoughPermissions, resource), ErrorType.UnauthorizedAccess);
            }

            if (retVal)
                return Result.Ok();
            else
                return Result.Fail(string.Format(ExceptionMessages.UnauthorizedAccess_NoEnoughPermissions, string.Join(" & ", resources)), ErrorType.UnauthorizedAccess);
        }

        public static Result CheckResourceClaim(this ClaimsPrincipal principal, string claimName, string[] resources, bool requireActionInAllResources = false)
        {
            var retVal = false;

            foreach (var resource in resources)
            {
                var rsrc = principal.Claims
                                    .FirstOrDefault(c => c.Type == dFClaimTypes.Resource && c.Value == resource);
                if (rsrc != null)
                {
                    //if (claim.Properties.Any(p => p.Key == action.ToString() && p.Value == ClaimConstants.Permissions.Allow))
                    if (rsrc.Properties.Count(p => p.Key == claimName && p.Value == "Claim") == 1)
                    {
                        if (requireActionInAllResources)
                            retVal = true;
                        else
                            return Result.Ok();
                    }
                    else if (requireActionInAllResources)
                        return Result.Fail(string.Format(ExceptionMessages.UnauthorizedAccess_NoEnoughPermissions, resource), ErrorType.UnauthorizedAccess);
                }
                else if (requireActionInAllResources)
                    return Result.Fail(string.Format(ExceptionMessages.UnauthorizedAccess_NoEnoughPermissions, resource), ErrorType.UnauthorizedAccess);
            }

            if (retVal)
                return Result.Ok();
            else
                return Result.Fail(string.Format(ExceptionMessages.UnauthorizedAccess_NoEnoughPermissions, string.Join(" & ", resources)), ErrorType.UnauthorizedAccess);
        }

        public static dFAccess CanRead(this ClaimsPrincipal principal, params string[] resources)
        {
            return new dFAccess(principal).CanRead(resources);
        }

        public static dFAccess CanReadAll(this ClaimsPrincipal principal, params string[] resources)
        {
            return new dFAccess(principal).CanReadAll(resources);
        }

        public static dFAccess CanUpdate(this ClaimsPrincipal principal, params string[] resources)
        {
            return new dFAccess(principal).CanUpdate(resources);
        }

        public static dFAccess CanRemove(this ClaimsPrincipal principal, params string[] resources)
        {
            return new dFAccess(principal).CanRemove(resources);
        }

        public static dFAccess CanAdd(this ClaimsPrincipal principal, params string[] resources)
        {
            return new dFAccess(principal).CanAdd(resources);
        }

        public static dFAccess HasClaim(this ClaimsPrincipal principal, string claimName, params string[] resources)
        {
            return new dFAccess(principal).HasClaim(claimName, resources);
        }
    }
}
