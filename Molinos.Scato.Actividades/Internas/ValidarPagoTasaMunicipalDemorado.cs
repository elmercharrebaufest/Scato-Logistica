using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Ninject.Extensions.Logging;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarPagoTasaMunicipalDemorado : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> WorkflowInstanceId { get; set; }
        public InOutArgument<bool> VehiculoDemorado { get; set; }
        public OutArgument<bool> DebeQuitarDemoraTasaAdeudada { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            var srvRepositorio = context.GetExtension<IServicioRepositorio>();
            var log = context.GetExtension<ILogger>();
            var workflowInstanceId = WorkflowInstanceId.Get<Guid>(context);

            var resultado = new Resultado();

            try
            {
                log.Debug($"Validando el pago de la tasa municipal para el recorrido {workflowInstanceId}.");
                DebeQuitarDemoraTasaAdeudada.Set(context, false);

                if (!srvRepositorio.EstaDemoradoPorTasaAdeudada(workflowInstanceId))
                    return resultado;

                log.Debug($"El recorrido {workflowInstanceId} está demorado por una tasa adeudada.");

                var recorrido = srvRepositorio.ObtenerRecorridoPorGuid(workflowInstanceId);

                if (!TieneDatosValidos(recorrido, workflowInstanceId, log, resultado))
                    return resultado;

                resultado = EjecutarValidacionPago(recorrido, servicioComandos);

                if (resultado.HayErrores)
                    return resultado;

                var resultadoConsultar = resultado as ResultadoConsultarPagoTasaMunicipal;

                if (resultadoConsultar == null)
                {
                    log.Error($"El comando VerificarPagoTasaMunicipal retornó un tipo inesperado: {resultado?.GetType().Name ?? "null"}");
                    resultado = CrearError("Error al verificar el pago de la tasa municipal");
                    return resultado;
                }

                if (!resultadoConsultar.EsPagoAbonado)
                    return resultado;

                VehiculoDemorado.Set(context, false);
                DebeQuitarDemoraTasaAdeudada.Set(context, true);
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error inesperado al validar el pago de la tasa municipal demorado para recorrido {workflowInstanceId}");
                resultado.Errores.Add("", Textos.Error);
            }

            return resultado;
        }

        private bool TieneDatosValidos(RecorridoDto recorrido, Guid instanceId, ILogger log, Resultado resultado)
        {
            if (recorrido?.Centro == null || recorrido.Material == null || recorrido.Vehiculo == null)
            {
                log.Error($"Recorrido {instanceId} tiene datos incompletos.");
                resultado.Errores.Add("", "El recorrido no tiene la información necesaria para validar el pago");
                return false;
            }

            return true;
        }

        private Resultado EjecutarValidacionPago(RecorridoDto recorrido, IServicioComandos servicioComandos)
        {
            return servicioComandos.Ejecutar(new VerificarPagoTasaMunicipal
            {
                CentroId = recorrido.Centro.Id,
                InstanceId = recorrido.InstanciaWorkflow,
                Ctg = recorrido.NumeroDocumentoIngreso ?? string.Empty,
                Patente = recorrido.Patente,
                PatenteAcoplado = recorrido.Vehiculo.PatenteAcoplado,
                MaterialId = recorrido.Material.Id,
                TipoVehiculo = recorrido.TipoVehiculo,
                TipoOrigenDeValidacion = TipoOrigenDeValidacion.Recorrido,
            });
        }

        private Resultado CrearError(string mensaje)
        {
            var resultado = new Resultado();
            resultado.Errores.Add("", mensaje);
            return resultado;
        }
    }
}
