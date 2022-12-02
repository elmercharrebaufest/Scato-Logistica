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
    public class HabilitarConfirmacionCargaDescarga : CodeActivity
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
                    Confirmado = false,
                    PendienteConfirmacion = true
                };
                servicio.Ejecutar(new CrearConfirmacionCargaDescarga
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
