using System.Activities.Presentation;
using System.Activities.Presentation.Services;
using System.Activities.Statements;
using System.IO;
using System.Reflection;
using System.ServiceModel.Activities;
using System.Text;

namespace Molinos.Scato.WfEditorWeb.Ejecucion
{
    public static class ExtensionesWorkflowDesigner
    {
        /// <summary>
        /// Carga un workflow desde la representacion en binario de un xamlx
        /// </summary>
        /// <param name="designer"></param>
        /// <param name="xamlx"></param>
        public static void LoadXaml(this WorkflowDesigner designer, byte[] xamlx)
        {
            using (var reader = new StreamReader(new MemoryStream(xamlx)))
            {
                designer.Text = reader.ReadToEnd();
                designer.Load();
                // El designer tiene un bug (https://connect.microsoft.com/VisualStudio/feedback/details/786503/)
                // Cuando se carga el xaml por la propiedad Text que 
                // hace que al guardarlo falle porque no maneja bien un simbolo de debug.
                // Esto de aca abajo lo arregla
                var getAttachedWorkflowSymbol = designer.GetType().GetMethod("GetAttachedWorkflowSymbol", BindingFlags.Instance | BindingFlags.NonPublic);
                getAttachedWorkflowSymbol.Invoke(designer, new object[0]);
            }
        }

        /// <summary>
        /// Devuelve el workflow como la representacion binaria del xamlx correspondiente
        /// </summary>
        /// <param name="designer"></param>
        /// <returns></returns>
        public static byte[] SaveXaml(this WorkflowDesigner designer)
        {
            designer.Flush();
            return new UTF8Encoding().GetBytes(designer.Text);
        }

        public static string ActividadInicial(this WorkflowDesigner designer)
        {
            var modelService = designer.Context.Services.GetService<ModelService>();
            var workflow = (WorkflowService) modelService.Root.GetCurrentValue();
            var flowchart = (Flowchart) workflow.Body;
            var flowStep = flowchart.StartNode as FlowStep;
            return flowStep != null ? flowStep.Action.GetType().Name : null;
        }

    }
}
