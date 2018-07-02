using dF.Commons.Models.BL.Contracts;
using dF.Commons.Models.BL.Enums;
using System;
using System.Collections.Generic;

namespace dF.Commons.Models.BL
{
    public class LinkID
    {
        #region Fields
        private IDictionary<string, string> _params = null;
        #endregion

        #region Builder Fields
        private bool _withNoCheks = false;
        private byte _missingNonDefaultRequiredFields = 0;
        #endregion

        #region Properties
        public string ID { get; internal set; }
        public string Template { get; internal set; }
        public IDictionary<string, string> Parameters
        {
            get { return _params != null ? _params : _params = new Dictionary<string, string>(StringComparer.Ordinal); }
        }
        public ResourceLink ResourceSpecs { get; internal set; }
        public bool IsValid
        {
            get
            {
                if (!_withNoCheks && _missingNonDefaultRequiredFields != 0)
                    return false;

                return true;
            }
        }
        #endregion

        #region Constructors
        protected LinkID(string id)
        {
            ID = id;
        }

        //protected LinkID(string id, string template)
        //    : this(id)
        //{
        //    Template = template;
        //}
        #endregion

        #region Builders
        public LinkID withTemplate(string template)
        {
            Template = template;
            return this;
        }

        public LinkID withParameter(string key, string value = "")
        {
            if (!_withNoCheks)
            {
                var parameter = ResourceSpecs.Parameters.Find(p => p.Name == key);

                if (parameter == null)
                    throw new InvalidOperationException("HATEOAS LinkID: Tried to Add Invalid Parameter");

                if (parameter.DefaultValue != null)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        value = parameter.DefaultValue.ToString();
                }
                else if (parameter.IsRequired)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new InvalidOperationException("HAATEOAS LinkID: Required parameter not included");

                    _missingNonDefaultRequiredFields--;
                }
            }

            Parameters[key] = value;

            return this;
        }

        public LinkID withParameter(string key, object value = null)
        {
            return withParameter(key, value.ToString());
        }
        #endregion

        #region Commands
        public void ClearResourceSpecs()
        {
            ResourceSpecs = null;
        }
        #endregion

        #region Static Constructors
        public static LinkID withID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("Please provide a Link ID");

            var linkId = new LinkID(id);
            linkId._withNoCheks = true;

            return linkId;
        }

        public static LinkID fromResourceLink(ResourceLink resourceLink)
        {
            if (resourceLink == null)
                throw new ArgumentNullException("Please provide a ResourceLink");

            var linkId = new LinkID(resourceLink.ResourceID)
            {
                ResourceSpecs = resourceLink
            };

            foreach (var parameter in resourceLink.Parameters)
            {
                if (parameter.IsRequired && parameter.DefaultValue == null)
                    linkId._missingNonDefaultRequiredFields++;
            }

            return linkId;
        }

        public static LinkID fromResourceMap(List<ResourceLink> resourceMap, string resourceId, CRUDverbs verb = CRUDverbs.Read)
        {
            var resourceLink = resourceMap.Find(l => l.ResourceID == resourceId && l.Verb == verb);

            if (resourceLink == null)
                throw new ArgumentNullException("There is no method associated to this Resource Link. Please provide a Resource Link through the ResourceId Attribute.");

            return fromResourceLink(resourceLink);
        }

        public static LinkID fromServiceMap(IServiceResourceMap serviceResourceMap, string resourceId, CRUDverbs verb = CRUDverbs.Read)
        {
            return fromResourceMap(serviceResourceMap.Map, resourceId, verb);
        }
        #endregion
    }
}
