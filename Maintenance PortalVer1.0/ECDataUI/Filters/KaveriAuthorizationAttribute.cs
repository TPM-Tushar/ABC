//using ECDataUI.Security;
//using ECDataUI.Session;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Web;
//using System.Web.Mvc;
//using ECDataUI.Common;
//using System.Net;



#region References
using CustomModels.Security;

using ECDataUI.Session;
using System;
using System.Security.Principal;
using System.Web;
using System.Net;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Web.Routing;
using ECDataUI.Common;
using System.Collections.Generic;
using CustomModels.Common;
using System.Configuration;

#endregion


namespace ECDataUI.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class KaveriAuthorizationAttribute : AuthorizeAttribute
    {

        /// <summary>
        /// OnAuthorization()
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
            {
            // add here if any other url you want to ignore 
            List<string> UrlsToIgnoreList = new List<string>() {
                "Accounts/AccountDetails/PaymentGateWayResponse"
            };

            bool bIsURLToIgnore = false;

            if (KaveriSession.Current.IsSessionExpired())
            {
                KaveriSession.Current.EndSession();
                filterContext.Result = new RedirectResult("/Error/SessionExpire");
            }

            else
            {

                foreach (var item in UrlsToIgnoreList)
                {
                    bIsURLToIgnore = filterContext.HttpContext.Request.Url.AbsoluteUri.Contains(item);
                    if (bIsURLToIgnore)
                        break;
                }


                // added by Akash on 29/10/2018 to prevent multiple login(session) of same user
                if (PreventMultipleLogin.IsLoggedSession.ContainsKey(KaveriSession.Current.UserID))
                {
                    string previousSessionID = string.Empty;
                    PreventMultipleLogin.IsLoggedSession.TryGetValue(KaveriSession.Current.UserID, out previousSessionID);
                    if (KaveriSession.Current.SessionID != previousSessionID)
                    {
                        filterContext.Result = new RedirectResult("/Error/SessionExpire");

                        #region commented by Akash 29-10-2018
                        //// filterContext.Result = new RedirectResult("/Login/SessionExpired");
                        //if (filterContext.HttpContext.Request.IsAjaxRequest())
                        //{
                        //    KaveriSession.Current.EndSession();
                        //    //filterContext.HttpContext.Response.StatusCode = 401;
                        //    //filterContext.HttpContext.Response.End();
                        //    KaveriSession.Current.UserID = 0;
                        //    filterContext.Result = new JsonResult
                        //    {
                        //        Data = new
                        //        {
                        //            // put whatever data you want which will be sent
                        //            // to the client
                        //            message = "sorry, but you were logged out due to duplicate session."
                        //            //Data = new { success = false, error = filterContext.Exception.ToString() },
                        //        },
                        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        //    };
                        //}
                        //else
                        //{
                        //    filterContext.Result = new RedirectResult("/Error/SessionExpire");

                        //} 
                        #endregion
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Error/SessionExpire");
                }


                string SiteReferrer=  ConfigurationManager.AppSettings["ApplicationURL"];

                if (filterContext.HttpContext.Request.UrlReferrer == null) //redirect to login page if user directly entres URL in browser
                {

                    if (!bIsURLToIgnore)
                        filterContext.Result = new RedirectResult("/Error/SessionExpire");
                }

                else
                {
                    // Added by akash patil on 9-07-2019
                    // commented by shubham bhagat on 9-07-2019
                    // site is deployed on Shubham machine that is egovdell06
                    // because session is expiring after successfull login and clicking on Parent menu on Shubham 
                    // machine and other's machine when URL used is -'http://egovdell06:portnumber' but it is running on shubham(egovdell06)
                    // machine when URL used is -'http://localhost:portnumber'
                    //if (!filterContext.HttpContext.Request.UrlReferrer.ToString().StartsWith(SiteReferrer))
                    //{
                    //    filterContext.Result = new RedirectResult("/Error/SessionExpire");
                    //}

                    IIdentity identity = new CustomIdentity(KaveriSession.Current.UserName);
                    IPrincipal principal = new CustomPrincipal(identity);
                    filterContext.HttpContext.User = principal;
                    var routeData = filterContext.RequestContext.RouteData;

                    string areaName = Convert.ToString(routeData.DataTokens["area"]);
                    string controllerName = routeData.GetRequiredString("controller");
                    string actionName = routeData.GetRequiredString("action");


                    if (!CommonFunctions.CheckUserPermissionForRole(KaveriSession.Current.RoleID, areaName, controllerName, actionName))
                    {
                        try
                        {
                            if (KaveriSession.Current.RoleID == (Convert.ToInt16(ECDataUI.Common.CommonEnum.RoleDetails.OnlineUser)))
                                filterContext.Result = new RedirectResult("/Error/SessionExpire");
                            else

                                filterContext.Result = new RedirectResult("/Error/SessionExpire");
                        }
                        catch (Exception)
                        {

                            filterContext.Result = new RedirectResult("/Error/SessionExpire");

                        }
                    }


                }
            }

        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateAntiForgeryTokenOnAllPosts : AuthorizeAttribute
    {
        public const string HTTP_HEADER_NAME = "x-RequestVerificationToken";
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;

            //  Only validate POSTs
            if (request.HttpMethod == WebRequestMethods.Http.Post || request.HttpMethod == WebRequestMethods.Http.Put)
            {
                if (request.IsAjaxRequest())
                {

                    var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];

                    var cookieValue = antiForgeryCookie != null
                        ? antiForgeryCookie.Value
                        : null;
                    //added by nilesh on date 10-14-2017
                    var requestVerificationToken = request.Headers["__RequestVerificationToken"];
                    //AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
                    AntiForgery.Validate(cookieValue, requestVerificationToken);
                }
                else
                {
                    new ValidateAntiForgeryTokenAttribute()
                        .OnAuthorization(filterContext);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AjaxValidateAntiForgeryTokenAttribute : AuthorizeAttribute
    {
        public const string HTTP_HEADER_NAME = "x-RequestVerificationToken";
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            try
            {
                //  Only validate POSTs
                if (request.HttpMethod == WebRequestMethods.Http.Post || request.HttpMethod == WebRequestMethods.Http.Put)
                {
                    if (request.IsAjaxRequest())
                    {
                        string cookieToken = string.Empty;
                        string formToken = string.Empty;
                        string tokenValue = request.Headers["__RequestVerificationToken"]; // read the header key and validate the tokens.

                        if (Convert.ToString(HttpContext.Current.Session["RequestVerificationToken"]) == tokenValue)
                        {
                            if (!string.IsNullOrEmpty(tokenValue))
                            {
                                string[] tokens = tokenValue.Split(',');
                                if (tokens.Length == 2)
                                {
                                    cookieToken = tokens[0].Trim();
                                    formToken = tokens[1].Trim();
                                }
                            }
                        }

                        AntiForgery.Validate(cookieToken, formToken); // this validates the request token.
                    }
                    else
                    {
                        new ValidateAntiForgeryTokenAttribute()
                            .OnAuthorization(filterContext);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}