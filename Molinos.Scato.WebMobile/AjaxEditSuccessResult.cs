using System.Web.Mvc;

namespace Molinos.Scato.WebMobile
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