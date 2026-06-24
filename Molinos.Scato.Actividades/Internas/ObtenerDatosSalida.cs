using System;
using System.Activities;
using System.ComponentModel;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Actividades
{
    public class ObtenerDatosSalida : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public InArgument<string> NombreUsuario { get; set; }

        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public OutArgument<string> cupoSalida { get; set; }

        public InArgument<CartaPorteDto> Orden { get; set; }

        public OutArgument<int>KmARecorrerSalida { get; set; }

        public OutArgument<decimal> TarifaDeSalida {  get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get(context);
            var nombreUsuario = NombreUsuario.Get(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var orden = Orden.Get(context);

            var resultado = new Resultado();

            var logActividad = new LogActividadDto
            {
                Actividad = "Obtener datos de salida",
                ActividadXaml = "ObtenerDatosSalida",
                WorkflowInstanceId = instanceId,
                Fecha = DateTime.Now
            };

            if (instanceId == Guid.Empty)
            {
                LogError(context, Textos.Error_IdInstanciaInvalido , nombreUsuario , resultado);
                return resultado;
            }
            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                LogError(context, Textos.Error_UsuarioInvalido, nombreUsuario, resultado);
                return resultado;
            }

            var servicioComandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var persistenceParticipant = context.GetExtension<ScatoPersistenceParticipant>();
            var log = context.GetExtension<ILogger>();

            try
            {
                LogActividad(context, servicioComandos, "ObtenerDatosSalida", resultado);
                cupoSalida.Set(context, orden.CupoSalida);
                KmARecorrerSalida.Set(context, orden.KmRecorrerSalida);
                TarifaDeSalida.Set(context, orden.TarifaToneladaSalida);

                servicioComandos.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "ObtenerDatosSalida",
                        Fecha = DateTime.Now,
                        Comentario = $"Cupo salida : {orden.CupoSalida} Km a recorrer salida: {orden.KmRecorrerSalida} Tarifa salida: {orden.TarifaToneladaSalida}",
                        NombreUsuario = nombreUsuario,
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });

            }
            catch (Exception ex)
            {
                LogControlRecorrido(context, servicioComandos, ex, resultado);
            }

            try
            {
                resultado = servicioComandos.Ejecutar(new FinDeActividad { InstanceId = context.WorkflowInstanceId, Actividad = "ObtenerDatosSalida", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }

        private void LogActividad(CodeActivityContext context, IServicioComandos servicioComandos, string actividad , Resultado resultado)
        {
           resultado =  servicioComandos.Ejecutar(new CrearLogActividad
            {
                Dto = new LogActividadDto
                {
                    Actividad = actividad,
                    ActividadXaml = actividad,
                    WorkflowInstanceId = context.WorkflowInstanceId,
                    Fecha = DateTime.Now
                }
            });
        }

        private void LogError(CodeActivityContext context, string mensaje , string usuario , Resultado resultado)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
            {
                Dto = new ControlRecorridoDto
                {
                    Actividad = "ObtenerDatosSalida",
                    Fecha = DateTime.Now,
                    Comentario = mensaje,
                    NombreUsuario = usuario,
                    WorkflowInstanceId = context.WorkflowInstanceId,
                }
            });
        }

        private void LogControlRecorrido(CodeActivityContext context, IServicioComandos servicioComandos, Exception ex , Resultado resultado)
        {
            resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
            {
                Dto = new ControlRecorridoDto
                {
                    Actividad = "ObtenerDatosSalida",
                    Fecha = DateTime.Now,
                    Comentario = $"{Textos.Error_ActualizarGenerico}: {ex.Message}",
                    NombreUsuario = "",
                    WorkflowInstanceId = context.WorkflowInstanceId,
                }
            });
        }

    }
}
