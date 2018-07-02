using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using dF.Commons.Exceptions.Resources;
using dF.Commons.Models.Global.Enums;
using dF.Commons.Models.Globals;
using dF.Commons.Security.Constants;
using dF.Commons.Security.Models;

namespace dF.Commons.Security.Helpers
{
    public class dFAccess
    {
        ClaimsPrincipal _principal;

        Dictionary<string, ClaimNActions> _resourceAccess = new Dictionary<string, ClaimNActions>();

        public dFAccess() { }

        public dFAccess(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        public dFAccess(ClaimsPrincipal principal, string resource, Actions action)
        {
            _principal = principal;
            _resourceAccess.Add(resource, new ClaimNActions(action));
        }

        public dFAccess(ClaimsPrincipal principal, string resource, string claimName)
        {
            _principal = principal;
            _resourceAccess.Add(resource, new ClaimNActions(claimName));
        }

        public dFAccess CanRead(params string[] resources)
        {
            foreach (var resource in resources)
            {
                if (_resourceAccess.TryGetValue(resource, out ClaimNActions access))
                    access.Actions = access.Actions | Actions.Read;
                else
                    _resourceAccess.Add(resource, new ClaimNActions(Actions.Read));
            }

            return this;
        }

        public dFAccess CanReadAll(params string[] resources)
        {
            foreach (var resource in resources)
            {
                if (_resourceAccess.TryGetValue(resource, out ClaimNActions access))
                    access.Actions = access.Actions | Actions.ReadAll;
                else
                    _resourceAccess.Add(resource, new ClaimNActions(Actions.ReadAll));
            }

            return this;
        }

        public dFAccess CanUpdate(params string[] resources)
        {
            foreach (var resource in resources)
            {
                if (_resourceAccess.TryGetValue(resource, out ClaimNActions access))
                    access.Actions = access.Actions | Actions.Update;
                else
                    _resourceAccess.Add(resource, new ClaimNActions(Actions.Update));
            }

            return this;
        }

        public dFAccess CanRemove(params string[] resources)
        {
            foreach (var resource in resources)
            {
                if (_resourceAccess.TryGetValue(resource, out ClaimNActions access))
                    access.Actions = access.Actions | Actions.Remove;
                else
                    _resourceAccess.Add(resource, new ClaimNActions(Actions.Remove));
            }

            return this;
        }

        public dFAccess CanAdd(params string[] resources)
        {
            foreach (var resource in resources)
            {
                if (_resourceAccess.TryGetValue(resource, out ClaimNActions access))
                    access.Actions = access.Actions | Actions.Add;
                else
                    _resourceAccess.Add(resource, new ClaimNActions(Actions.Add));
            }

            return this;
        }

        public dFAccess HasClaim(string claimName, params string[] resources)
        {
            foreach (var resource in resources)
            {
                if (_resourceAccess.TryGetValue(resource, out ClaimNActions access))
                    access.Claims.Add(claimName);
                else
                    _resourceAccess.Add(resource, new ClaimNActions(claimName));
            }

            return this;
        }

        public Result CheckAccess()
        {
            foreach (var resource in _resourceAccess)
            {
                var rsrc = _principal.Claims.FirstOrDefault(c => c.Type == dFClaimTypes.Resource && c.Value == resource.Key);

                if (rsrc != null)
                {
                    if (resource.Value.Actions != Actions.None)
                    {
                        var allowedKeyValue = rsrc.Properties.First(p => p.Key == "AllowedCRUD");

                        byte allowed = 0;

                        if (!(byte.TryParse(allowedKeyValue.Value, out allowed) && ((allowed & (byte)resource.Value.Actions) != 0)))
                            return Result.Fail(ExceptionMessages.UnauthorizedAccess_ActionNotAllowed, ErrorType.UnauthorizedAccess);
                    }

                    foreach (var claimName in resource.Value.Claims)
                    {
                        if (!rsrc.Properties.ContainsKey(claimName))
                            return Result.Fail(ExceptionMessages.UnauthorizedAccess_ActionNotAllowed, ErrorType.UnauthorizedAccess);
                    }
                }
                else
                    return Result.Fail(ExceptionMessages.UnauthorizedAccess_ActionNotAllowed, ErrorType.UnauthorizedAccess);
            }

            return Result.Ok();
        }
    }
}
