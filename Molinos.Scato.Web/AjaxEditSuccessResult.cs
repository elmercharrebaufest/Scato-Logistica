using System.Web.Mvc;

namespace Molinos.Scato.Web
{
    public class AjaxEditSuccessResult : ContentResult
    {
        public const string SuccessValue = "ajax-edit-success";

        public AjaxEditSuccessResult()
        {
            Content = SuccessValue;
        }
    }
}