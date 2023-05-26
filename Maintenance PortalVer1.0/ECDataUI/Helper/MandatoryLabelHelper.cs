using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ECDataUI.Helper
{
    
    public enum Mandatory
    {
        No,
        Yes
    }

    public static class MandatoryLabelHelper
    {
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Mandatory mandatory)
        {
            MvcHtmlString label = html.LabelFor(expression);
            string htmlString = label.ToHtmlString();

            if (mandatory == Mandatory.Yes)
            {
                return MvcHtmlString.Create(htmlString + "<label style='color:red'>*</label>");
            }

            return html.LabelFor(expression);
        }

        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, Mandatory mandatory)
        {
            MvcHtmlString label = html.LabelFor(expression, htmlAttributes);
            string htmlString = label.ToHtmlString();

            if (mandatory == Mandatory.Yes)
            {
                return MvcHtmlString.Create(htmlString + "<label style='color:red'>*</label>");
            }

            return html.LabelFor(expression, htmlAttributes);
        }
    }
}