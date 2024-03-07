using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;

namespace Molinos.Scato.Actividades.Internas
{
    public class VerificarAutomatismoGranos : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<bool> TieneAutomatismo { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var asignacion = repositorio.ObtenerAsignacionAutomatismoGranoEnRecorrido(context.WorkflowInstanceId);

            if(asignacion != null)
            {
                TieneAutomatismo.Set(context, true);
                return;
            }


            TieneAutomatismo.Set(context, false);

        }
    }
}
