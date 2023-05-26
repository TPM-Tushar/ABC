using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net;
using System.Net.Http;
using System.Text;
using ECDataAPI.Models;
using ECDataAPI.Interface;
using ECDataAPI.BAL;
using System.Threading;
using System.Security.Principal;
namespace ECDataAPI.Filters
{
    public class AuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                //No authorize headers are added by client
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            else
            {
                string AuthenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                #region AuthenticationToken Details
                /*
                 AuthenticationToken is a base64 Encoded in format username:password
                 * Client Side Authorization header add Sample code for MVC HttpClient
                 * 
                 *  string credentials = string.Format("{0}:{1}", userName, password);
                     byte[] credentialsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(credentials);
                     string base64 = Convert.ToBase64String(credentialsBytes);
                      AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic", base64);
                 * HttpClient.DefaultRequestHeaders.Authorization =authHeader;
                 */
                #endregion

                string DecodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(AuthenticationToken));
                string[] UserNamePasswordArr = DecodedAuthenticationToken.Split(':');
                if (UserNamePasswordArr.Length == 2)
                {

                    ClientAuthenticationModel model = new ClientAuthenticationModel()
                    {
                        ApiConsumerUserName = UserNamePasswordArr[0],
                        ApiConsumerPassword = UserNamePasswordArr[1]
                    };
                    ICommonInterface reference = new CommonBAL();
                    if (reference.IsAuthorizedClient(model))
                    {
                        Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(model.ApiConsumerUserName), null);
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
            }


        }
    }
}