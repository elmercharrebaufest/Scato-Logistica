using System.Web.Optimization;

namespace Molinos.Scato.WebMobile.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery-common").Include(
                       "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/jquery-ui-1-13.3.js",
                        "~/Scripts/modernizr-{version}.js",
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout-fast-foreach.min.js",
                        "~/scripts/scatoWebcommon.validate.js",
                        "~/Scripts/json2.min.js",
                        "~/Scripts/choices.min.js",
                        "~/Scripts/choices.loader.js"
                        ));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                       "~/Scripts/moment-with-locales*",
                       "~/Scripts/umd/popper.min.js",
                       "~/Scripts/bootstrap-datetimepicker.js",
                       "~/Scripts/bootstrap.bundle.js",
                       "~/Scripts/bootstrap-toggle.min.js"
                       ));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/Site.css"));
            
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/bootstrap-datetimepicker.css",
                    "~/Content/fontawesome-all.min.css",
                    "~/Content/bootstrap-toggle.min.css",
                    "~/Content/choices.min.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.css",
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                     "~/Scripts/jquery.validate.unobtrusive.js",
                     "~/Scripts/scatocommon.validate.js"
                     ));





        }
    }
}