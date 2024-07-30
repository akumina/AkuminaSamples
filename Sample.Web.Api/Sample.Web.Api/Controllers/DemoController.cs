using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Akumina.Common;
using Akumina.Common.Entities;
using Akumina.Common.Enums;
using Akumina.Common.Interfaces.Repository;
using Akumina.DataHub.Web.Api.Controllers;
using Akumina.Interchange.Core;
using Akumina.Interchange.Web.Utility;
using Akumina.Interchange.Web.Utility.Filters;
using Akumina.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sample.Web.Api.Controllers
{
    [RoutePrefix("Api/Demo")]
    public class DemoController : BaseApiOAuth2Controller
    {
        public DemoController(IRepository<UserToken> userTokenRepository) : base(userTokenRepository)
        {

        }

        [HttpGet]
        [Route("{ListName}/{Id}")]
        [AkAuthorizedAccess]
        [AkExecutionTime]
        public async Task<HttpResponseMessage> Get(string listName, string id, string siteUrl)
        {
            try
            {
                var key = InterChangeKeys.Get($"YoucCompany.Web.Api.Get:", listName, id);
                var queryResponse = AppManager.Caching.Get<string>(key);
                var cacheMiss = false;
                if (string.IsNullOrEmpty(queryResponse))
                {
                    cacheMiss = true;
                    //There are multiple way to get the access token, In this case we acquire the token during initial signin process for Graph and SharePoint
                    //If you Connect another data source such as Workday, ServiceNow, Concur and SSO enabled you may perform Redirect User to acquire user context token
                    //OR use ClientID/Secret -- basic authentication  (SSO Not required)
                    //OR use Privileged Access Token generated thru Provider Registration (aka Connector Framework) thru Akumina Connectors
                    var token = await GetAccessToken(ResourceType.SharePoint, siteUrl);

                    // logs current user email from Graph token, it first checks for email claim if it is not found then checks upn
                    await LogCurrentUserEmail(siteUrl);

                    var targetUrl = $"{siteUrl}/_api/web/lists/GetByTitle('{listName}')/items({id})";
                    var headers = new Dictionary<string, string>
                    {
                        { "accept", "application/json;odata=verbose" }
                    };
                    var response = await AuthClient.GetData(targetUrl, headers, token);
                    queryResponse = JsonConvert.SerializeObject(response);
                    AppManager.Caching.Add(key, queryResponse, new TimeSpan(0, 0, AppSettings.Caching.CacheRestApiInterval));
                }
                var httpResponse = Request.CreateResponse(HttpStatusCode.OK);
                httpResponse.Content = new StringContent(queryResponse, Encoding.UTF8, "application/json");
                CustomHeaders.SetAkResponseHeaders(httpResponse, cacheMiss);
                return httpResponse;
            }
            catch (Exception e)
            {
                TraceEvents.Log.Error(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.ToString());
            }
        }

        [HttpPost]
        [Route("Validate")]
        [AkQueryKey]
        [AkExecutionTime]
        public async Task<HttpResponseMessage> Validate()
        {
            try
            {
                return await Task.Run(() =>
                {
                    var httpResponse = Request.CreateResponse(HttpStatusCode.OK);
                    httpResponse.Content = new StringContent("Ok", Encoding.UTF8, "application/json");
                    return httpResponse;
                });
            }
            catch (Exception e)
            {
                TraceEvents.Log.Error(e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.ToString());
            }
        }

        private async Task LogCurrentUserEmail(string siteUrl)
        {
            var email = await GetGraphTokenClaimValue(siteUrl, "email");

            if(string.IsNullOrEmpty(email))
            {
                email = await GetGraphTokenClaimValue(siteUrl, "upn");
            }

            TraceEvents.Log.Debug($"CurrentUser's Email: {email}");
        }

        private async Task<string> GetGraphTokenClaimValue(string siteUrl, string claimName)
        {
            // get graph access token
            var graphAccessToken = await GetAccessToken(ResourceType.Graph, siteUrl);

            // read claim value from token
            var claimValue = GetTokenClaimValue(graphAccessToken, claimName);
            return claimValue;
        }

        private string GetTokenClaimValue(string token, string claimName)
        {
            var claimValue = "";
            try
            {
                JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var tokenCalims = jwtSecurityTokenHandler.ReadJwtToken(token).Claims;

                var claim = tokenCalims.FirstOrDefault(c => c.Type.Equals(claimName, StringComparison.CurrentCultureIgnoreCase));

                if (claim == null)
                    throw new Exception($"{claimName} claim not found.");

                claimValue = claim.Value;
            }
            catch (Exception ex)
            {
                TraceEvents.Log.Error($"Error in reading Jwt token claim.", ex);
            }

            return claimValue;
        }
    }
}
