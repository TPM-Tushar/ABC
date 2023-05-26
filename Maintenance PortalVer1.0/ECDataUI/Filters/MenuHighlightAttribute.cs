using CustomModels.Common;
using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MenuHighlightAttribute : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            MenuHighlightReqModel reqModel = new MenuHighlightReqModel();

            reqModel.ActionName = filterContext.ActionDescriptor.ActionName;
            reqModel.ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            ServiceCaller service = new ServiceCaller("CommonsApiController");

            string errorMessage = string.Empty;
            MenuHighlightResponseModel responseModel = service.PostCall<MenuHighlightReqModel, MenuHighlightResponseModel>("GetMenuDetailsToHighlight", reqModel, out errorMessage);//call to serive



            if (!string.IsNullOrEmpty(errorMessage))
            {


            }
            else
            {
                if (responseModel != null)
                {
                    if (string.IsNullOrEmpty(responseModel.MenuName))
                    {
                        KaveriSession.Current.SubParentMenuName = "";
                    }
                    else
                    {
                        KaveriSession.Current.SubParentMenuName = responseModel.MenuName;
                    }
                }
            }




        }



        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //throw new NotImplementedException();
        }
    }
}