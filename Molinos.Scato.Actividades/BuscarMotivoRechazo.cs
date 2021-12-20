using System;
using System.Activities;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class BuscarMotivoRechazo : CodeActivity<String>
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        public OutArgument<String> Motivo { get; set; }


        protected override String Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var workflowId = WorkflowId.Get<Guid>(context);
            var motivo = repositorio.ObtenerEstadoRecorrido(workflowId);
            
            var motivoRechazo = string.Empty;
            if(motivo != null)
            {
                motivoRechazo = motivo.Descripcion;
            }
            Motivo.Set(context, motivoRechazo);

            return motivoRechazo;

        }
    }
}
