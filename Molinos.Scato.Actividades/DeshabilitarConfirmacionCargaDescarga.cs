using System;
using System.Activities;
using System.Configuration;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
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
                var confirmacion = new ConfirmacionCargaDescargaDto()
                {
                    RecorridoId = recorridoId,
                    PendienteConfirmacion = false
                };
                servicio.Ejecutar(new ActualizarConfirmacionCargaDescarga
                {
                    Dto = confirmacion
                });
            }
            catch (Exception)
            {
            }
        }
    }
}
