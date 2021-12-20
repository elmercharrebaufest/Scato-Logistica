using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Routing;

namespace Molinos.Scato.Web.Helpers
{
    public static class ExtensionesCheckBoxListFor
    {
        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty[]>> expression, MultiSelectList multiSelectList, object htmlAttributes = null, string id = "")
        {
            //Derive property name for checkbox name
            MemberExpression body = expression.Body as MemberExpression;
            string propertyName = string.IsNullOrEmpty(id) ? body.Member.Name : id;


            //Get currently select values from the ViewData model
            TProperty[] list = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            //Convert selected value list to a List<string> for easy manipulation
            List<string> selectedValues = new List<string>();

            if (list != null)
            {
                selectedValues = new List<TProperty>(list).ConvertAll<string>(delegate(TProperty i) { return i.ToString(); });
            }

            //Create div
            TagBuilder divTag = new TagBuilder("div");
            divTag.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);
            
            //Add checkboxes
            foreach (SelectListItem item in multiSelectList)
            {
                divTag.InnerHtml += String.Format("<div><label for=\"{0}_{1}\" class=\"checkbox validarcheck\"><input type=\"checkbox\" class=\"validarcheck\" name=\"{0}\" id=\"{0}_{1}\" " +
                                                    "value=\"{1}\" {2} />{3}</label></div>",
                                                    propertyName,
                                                    item.Value,
                                                    selectedValues.Contains(item.Value) ? "checked=\"checked\"" : "",
                                                    item.Text);
            }
            return MvcHtmlString.Create(divTag.ToString());
        }
    }
}
