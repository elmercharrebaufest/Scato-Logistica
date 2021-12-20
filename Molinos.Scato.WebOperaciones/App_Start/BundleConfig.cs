using System.Web.Optimization;

namespace Molinos.Scato.WebOperaciones.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery-common").Include(
                        "~/Scripts/modernizr-*",
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/knockout-{version}.js",
                        "~/scripts/scatoWebcommon.validate.js",
                        "~/Scripts/json2.min.js"
                        ));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                       "~/Scripts/moment-with-locales*",
                       "~/Scripts/bootstrap*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/Site.css"));
            
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/bootstrap-datetimepicker.css",
                    "~/Content/fontawesome-all.min.css"
                    ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.css",
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.theme.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css"));
        }
    }
}