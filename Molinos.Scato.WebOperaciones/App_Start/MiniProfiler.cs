using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Molinos.Scato.WebOperaciones.App_Start;
using StackExchange.Profiling;
using StackExchange.Profiling.Mvc;
using StackExchange.Profiling.SqlFormatters;
using WebActivator;

[assembly: WebActivator.PreApplicationStartMethod(
    typeof (MiniProfilerPackage), "PreStart")]
[assembly: PostApplicationStartMethod(
    typeof (MiniProfilerPackage), "PostStart")]

namespace Molinos.Scato.WebOperaciones.App_Start
{
    public static class MiniProfilerPackage
    {
        public static void PreStart()
        {

            MiniProfiler.Settings.SqlFormatter = new SqlServerFormatter();
            MiniProfilerEF.Initialize();
            DynamicModuleUtility.RegisterModule(typeof (MiniProfilerStartupModule));
            GlobalFilters.Filters.Add(new ProfilingActionFilter());
        }

        public static void PostStart()
        {
            // Intercept ViewEngines to profile all partial views and regular views.
            // If you prefer to insert your profiling blocks manually you can comment this out
            List<IViewEngine> copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (IViewEngine item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }
    }

    public class MiniProfilerStartupModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) =>
                {
                    HttpRequest request = ((HttpApplication) sender).Request;
                    if (request.IsLocal || request.Cookies["profiling"] != null)
                    {
                        MiniProfiler.Start();
                    }
                };

            context.EndRequest += (sender, e) => MiniProfiler.Stop();
        }

        public void Dispose()
        {
        }
    }
}