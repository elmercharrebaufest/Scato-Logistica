using System;
using System.Activities;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class DeshabilitarConfirmacionCargaDescarga : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
                servicio.Ejecutar(new ActualizarConfirmacionCargaDescarga
                {
                    DeshabilitarConfirmacion = true,
                    RecorridoId = recorridoId
                });
            }
            catch (Exception)
            {
            }
        }
    }
}
