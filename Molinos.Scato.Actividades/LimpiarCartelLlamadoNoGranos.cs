using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class LimpiarCartelLlamadoNoGranos : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new ResultadoMensajeCartelLed();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var servicio = context.GetExtension<IServicioComandos>();

            try
            {

                var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);

                resultado = servicio.Ejecutar(new LimpiarRecorridoHistorialMensajeCartelLed()
                {
                    Codigo = CodigoMensajeCartelLed.LlamadoCamionNoGrano,
                    RecorridoId = recorridoId
                }) as ResultadoMensajeCartelLed;
            }
            catch (System.Exception ex)
            {

                resultado.Error("ErrorException", ex.Message);
            }


            if (!resultado.HayErrores)
            {
                var cartel = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.EstadoDeCallePostCalado, Constantes.ConfiguracionGeneral.PostCalado.CartelLedPostCalado);

                servicio.Ejecutar(new EnviarMensajeCartelLed
                {
                    Mensaje = "-",
                    Codigo = cartel?.Valor,
                    NumeroTrama = resultado.NumeroTrama,
                    NumeroPrograma = resultado.NumeroPrograma,
                    NumeroVariable = resultado.NumeroVariable,
                });
            }
            else
            {
                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "LimpiarCartelLlamadoNoGranos",
                        Fecha = DateTime.Now,
                        Comentario = resultado.Errores.Values.First(),
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
            }

            

        }
    }
}