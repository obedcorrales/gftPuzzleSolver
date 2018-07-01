using System.Collections.Generic;
using System.Linq;

using dF.Commons.Models.Service.Enums;

namespace dF.Commons.Models.Service
{
    public class RequestFlags
    {
        public LinksReturnFlag LinksReturnFlag { get; set; }

        public RequestFlags withLinksReturnFlag(LinksReturnFlag flag)
        {
            LinksReturnFlag = flag;
            return this;
        }

        public IEnumerable<string> AsEnumerable()
        {
            var flags = new List<string>();

            flags.Add(LinksReturnFlag);

            return flags;
        }

        public static RequestFlags FromEnumerable(IEnumerable<string> flags)
        {
            RequestFlags requestFlags = new RequestFlags();

            var linksReturnFlagStr = flags.ElementAtOrDefault(0);
            int LinksReturnFlagInt;

            requestFlags.LinksReturnFlag = (int.TryParse(linksReturnFlagStr, out LinksReturnFlagInt)) ? LinksReturnFlagInt : (int)LinksReturnFlag.NoPreference;

            return requestFlags;
        }
    }
}
