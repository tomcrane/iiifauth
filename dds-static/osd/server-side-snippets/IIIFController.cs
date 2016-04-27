

namespace Wellcome.Dds.Server.Controllers
{
    public class IIIFController : Controller
    {
        private static readonly bool AllowJSONPToken = StringUtils.GetBoolFromAppSetting("IIIF-Auth-Allow-JSONP", true);




        // loads of stuff omitted...



        
        /// <summary>
        /// New token service - supports postMessage, JSONP and direct (non-browser).
        /// 
        /// Simple case:
        /// User acquires cookie from the login service. The access token service is on the same domain, so can see that cookie.
        /// It returns a token that the client can then present on info.json requests.
        /// 
        /// More complex case:
        /// The token is valid on requests for content retrieved via XHR, and not just info.json requests. Annotations too.
        /// In this case the server must not issue the token unless it trusts the origin of the caller.
        /// 
        /// JSONP will be FORBIDDEN after newer deployments of UV have settled in.
        /// 
        /// </summary>
        /// <param name="code">Optional query string parameter for Client Identity Service.</param>
        /// <param name="browser">Must be true to trigger postMessage response.</param>
        /// <param name="id">Optional identifier for the token request, will be returned as a member of the token or error object.</param>
        /// <param name="close">Only valid if browser=true. Emits script to immediately close the opened window.</param>
        /// <returns>An HTML view if browser=true, a JSONP result if callback specified (for now), a JSON response otherwise.</returns>
        public ActionResult Token(string code = null, string browser = "false", string id = null, string close = "false")
        {
            ITokenResponse token = GetResponseToken(code, id); // might be an error object
            Response.AppendStandardNoCacheHeaders();
            if (StringUtils.GetBoolValue(browser, false))
            {
                // return an HTML view with the postMessage script
                var model = new TokenMessageServiceModel
                {
                    OpenerId = id,
                    CloseWindow = StringUtils.GetBoolValue(close, false),
                    PermittedOrigins = new[] {"*"},
                    JsonForPostMessage = JsonContentResultProvider.SerialisePretty(token)
                };
                return View("Token", model); 
            }

            // AllowJSONPToken (from config) is true for a while, to allow transition
            return JsonContentResultProvider.GetJsonContentResult(
                Request, token, true, "application/json", AllowJSONPToken);
        }
        
        
        
        
        // loads of stuff omitted...




    }
}
