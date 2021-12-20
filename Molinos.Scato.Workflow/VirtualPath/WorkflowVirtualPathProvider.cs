using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Workflow.VirtualPath

{
    public class WorkflowVirtualPathProvider : VirtualPathProvider
    {
        private readonly IServicioWorkflows servicioWorkflows;

        public WorkflowVirtualPathProvider(IServicioWorkflows servicioWorkflows)
        {
            this.servicioWorkflows = servicioWorkflows;
        }

        public override bool FileExists(string virtualPath)
        {
            return IsPathVirtual(virtualPath)
                        ? GetWorkflowFile(virtualPath).Exists
                        : base.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return IsPathVirtual(virtualPath)
                        ? GetWorkflowFile(virtualPath)
                        : base.GetFile(virtualPath);
        }

        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            return IsPathVirtual(virtualPath)
                ? Path.GetFileName(virtualPath)
                : base.GetFileHash(virtualPath, virtualPathDependencies);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            return IsPathVirtual(virtualPath) ? null : base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        private WorkflowVirtualFile GetWorkflowFile(string path)
        {
            return new WorkflowVirtualFile(path, servicioWorkflows);
        }

        private bool IsPathVirtual(string virtualPath)
        {
            var checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            return checkPath.StartsWith("~/xaml", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
