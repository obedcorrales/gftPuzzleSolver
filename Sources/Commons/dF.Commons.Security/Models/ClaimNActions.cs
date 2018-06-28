using System.Collections.Generic;

using dF.Commons.Security.Constants;

namespace dF.Commons.Security.Models
{
    class ClaimNActions
    {
        public ClaimNActions() { }

        public ClaimNActions(Actions actions)
        {
            Actions = actions;
            Claims = new HashSet<string>();
        }

        public ClaimNActions(HashSet<string> claims)
        {
            Claims = claims;
        }

        public ClaimNActions(string claim)
        {
            Claims = new HashSet<string> { claim };
        }

        public Actions Actions { get; set; }
        public HashSet<string> Claims { get; set; }
    }
}
