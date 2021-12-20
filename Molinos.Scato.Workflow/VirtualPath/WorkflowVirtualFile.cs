using System;
using System.IO;
using System.Web.Hosting;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Workflow.VirtualPath
{
    public class WorkflowVirtualFile : VirtualFile
    {
        private readonly IServicioWorkflows servicioWorkflows;
        private byte[] definicion;

        public WorkflowVirtualFile(string virtualPath, IServicioWorkflows servicioWorkflows) : base(virtualPath)
        {
            this.servicioWorkflows = servicioWorkflows;
            definicion = null;
            LoadWorkflow();
        }

        public bool Exists
        {
            get { return definicion != null; }
        }

        private void LoadWorkflow()
        {
            var id = Path.GetFileNameWithoutExtension(VirtualPath);

            var workflowDefinicionIdInt = 0;

            if (string.IsNullOrWhiteSpace(id) || !int.TryParse(id,out workflowDefinicionIdInt))
            {
                throw new InvalidOperationException(string.Format("Cannot find workflow definition for {0}", id));
            }
            definicion = (byte[]) HostingEnvironment.Cache[id];
            if (definicion == null)
            {
                var workflowDefinicion = servicioWorkflows.ObtenerArchivoDefinicionWorkflow(workflowDefinicionIdInt);
                if (workflowDefinicion == null)
                {
                    throw new InvalidOperationException(string.Format("Cannot find workflow definition for {0}", id));
                }
                definicion = workflowDefinicion;
                HostingEnvironment.Cache[id] = workflowDefinicion;
            }
        }

        /// <summary>
        ///   When overridden in a derived class, returns a read-only stream to the virtual resource.
        /// </summary>
        /// <returns>
        ///   A read-only stream to the virtual file.
        /// </returns>
        public override Stream Open()
        {
            if (definicion == null || definicion.Length == 0)
            {
                throw new InvalidOperationException("Workflow definition is null");
            }

            return new MemoryStream(definicion);

            //var stream = new MemoryStream(definicion.Length);
            //var writer = new StreamWriter(stream);
            //writer.Write(definicion);
            //writer.Flush();
            //stream.Seek(0, SeekOrigin.Begin);
            //return stream;
        }

    }
}
