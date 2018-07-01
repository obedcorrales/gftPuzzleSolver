using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using dF.Commons.Models.Global.Constants;
using dF.Commons.Models.Service;
using dF.Commons.Models.Service.Enums;

namespace dF.Commons.HATEOAS.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string IsHATEOASRequestPropertyName = "IsHATEOASRequest";

        #region IsHATEOASRequest
        public static bool SetIsHATEOASRequest(this HttpRequestMessage httpRequest)
        {
            try
            {
                var IsHATEOASRequest = httpRequest.Headers.Accept.Count(m => m.MediaType == MediaTypes.JSON.HATEOAS) > 0 ? true : false;

                httpRequest.Properties.Add(IsHATEOASRequestPropertyName, IsHATEOASRequest);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsHATEOASRequest(this HttpRequestMessage httpRequest)
        {
            try
            {
                object isHATEOASRequest;

                if (httpRequest.Properties.TryGetValue(IsHATEOASRequestPropertyName, out isHATEOASRequest))
                    return (bool)isHATEOASRequest;
                else
                    return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        #endregion

        #region RequestFlags
        public static bool SetRequestFlags(this HttpRequestMessage httpRequest)
        {
            try
            {
                IEnumerable<string> requestFlags = new List<string>();

                if (httpRequest.Headers.TryGetValues(Headers.HATEOASFlags, out requestFlags))
                {
                    httpRequest.Properties.Add(Headers.HATEOASFlags, RequestFlags.FromEnumerable(requestFlags));
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static RequestFlags GetRequestFlags(this HttpRequestMessage httpRequest)
        {
            try
            {
                object requestFlags;

                if (httpRequest.Properties.TryGetValue("ELTFlags", out requestFlags))
                    return requestFlags as RequestFlags;
                else
                    return new RequestFlags().withLinksReturnFlag(LinksReturnFlag.NoPreference);
            }
            catch (KeyNotFoundException)
            {
                return new RequestFlags().withLinksReturnFlag(LinksReturnFlag.NoPreference);
            }
        }
        #endregion
    }
}
