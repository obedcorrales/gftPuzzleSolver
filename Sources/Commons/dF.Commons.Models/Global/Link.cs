using dF.Commons.Models.BL;
using System;

namespace dF.Commons.Models.Globals
{
    public class Link
    {
        public string Rel { get; set; }
        public string StndRel { get; set; }
        public Uri Href { get; set; }
        public LinkID LinkId { get; set; }
        public string Action { get; set; }

        private Link() { }

        public Link(string rel, Uri hRef, string action = "", bool isStandardRel = false)
        {
            if (isStandardRel)
                StndRel = rel;
            else
                Rel = rel;

            Href = hRef;
            Action = action;
        }

        public Link(string rel, LinkID linkId, string action = "", bool isStandardRel = false)
        {
            if (isStandardRel)
                StndRel = rel;
            else
                Rel = rel;

            LinkId = linkId;
            Action = action;
        }
    }
}
