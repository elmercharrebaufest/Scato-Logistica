using System.Web.Optimization;

namespace Molinos.Scato.Web.App_Start
{
    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = false;
            bundles.Add(new ScriptBundle("~/bundles/jquery-common").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery-ui-i18n.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.globalize/globalize.js",
                        "~/Scripts/jquery.globalize/globalize.extend.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                        "~/Scripts/json2.min.js",
                //     "~/Scripts/jquery-1.10.1.min.js",
                        "~/Scripts/jquery.signalR-2.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap*"));

            // Se mueve directamente al cshtml
            //bundles.Add(new ScriptBundle("~/bundles/scato-common").Include(
            //            "~/Scripts/scatocommon.globalize.js",
            //            "~/Scripts/scatocommon.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.unobtrusive.js", 
                        "~/Scripts/scatocommon.validate.js",
                        "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/blockUI").Include(
                        "~/Scripts/jquery.blockUI.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-*"));
            // El Css ~/Content/site.css se agrega directamente en el cshtml
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/colorPicker.css"));
            
            bundles.Add(new StyleBundle("~/Content/bootstrap")
                    .Include("~/Content/bootstrap.css")
                    .Include("~/Content/bootstrap-responsive.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

           
        }
    }
}
