using System;
using System.Activities;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class DigitalizarEtiquetaAuditoriaEnCartaDePorteDetalle : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var workflowId = WorkflowId.Get<Guid>(context);


            var logActividad = new LogActividadDto
            {
                Actividad = "Digitalizar Etiqueta Auditoria En Carta De Porte Detalle",
                ActividadXaml = "DigitalizarEtiquetaAuditoriaEnCartaDePorteDetalle",
                WorkflowInstanceId = workflowId,
                Fecha = DateTime.Now
            };
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }

            try
            {
                var recorrido = repositorio.ObtenerRecorridoPorGuid(workflowId);
                var cp = repositorio.ObtenerCartaPortePorCentroYNumero(recorrido.NumeroDocumentoIngreso, recorrido.Centro.Id);
                var dto = new ImpEtiquetaAuditoriaDetalleDto();
                if (!string.IsNullOrEmpty(cp.FotoRutaDestinoDetalle) && recorrido.TipoVehiculo == TipoVehiculo.Tren)
                {
                   var impEtiquetaAuditoriaDetalle = repositorio.ObtenerPesoNetoTren(cp.Id);
                    if (impEtiquetaAuditoriaDetalle != null)
                    {
                        dto = new ImpEtiquetaAuditoriaDetalleDto
                        {
                            Vagones = impEtiquetaAuditoriaDetalle.Vagones
                        };
                    }
                   
                   
                    servicio.Ejecutar(new EtiquetaAuditoriaEnCartaDePorteDetalle { Dto = dto, RutaFotoCartaDePorte = cp.FotoRutaDestinoDetalle });
                }
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "DigitalizarEtiquetaAuditoriaEnCartaDePorteDetalle", PuestoDeTrabajoId = 0 });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }



            return resultado;
        }


    }
}
