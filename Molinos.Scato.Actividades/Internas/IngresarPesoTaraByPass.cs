using System;
using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Orquestador;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Actividades
{
    public class IngresarPesoTaraByPass : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<Guid> InstanceId { get; set; }

        public InArgument<string> NombreUsuario { get; set; }

        public OutArgument<int> peso { get; set; }

        public OutArgument<DateTime> fechaTara { get; set; }

        public InArgument<int> PuestoDeTrabajoId { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var instanceId = InstanceId.Get(context);
            var nombreUsuario = NombreUsuario.Get(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var resultado = new Resultado();

            var logActividad = new LogActividadDto
            {
                Actividad = "Ingresar Peso Tara ByPass",
                ActividadXaml = "IngresarPesoTaraByPass",
                WorkflowInstanceId = instanceId,
                Fecha = DateTime.Now
            };

            if (instanceId == Guid.Empty)
            {
                LogError(context, Textos.Error_IdInstanciaInvalido, nombreUsuario, resultado);
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

            try
            {
                LogActividad(context, servicioComandos, "IngresarPesoTaraByPass", resultado);

                var recorrido = repositorio.ObtenerRecorridoPorGuid(instanceId);
                if (recorrido == null)
                {
                    LogError(context, Textos.Error_RecorridoNoEncontrado, nombreUsuario, resultado);
                    return resultado;
                }

                var huellaDigital = repositorio.ObtenerHuellaDigitalPorFiltro(
                    recorrido.Patente,
                    recorrido.Vehiculo.PatenteAcoplado,
                    recorrido.Transportista.Id);
                if (huellaDigital == null)
                {
                    LogError(context, Textos.Error_HuellaDigitalNoEncontrada, nombreUsuario, resultado);
                    return resultado;
                }

                var resultadoPeso = ModificarPesoTara(servicioComandos, instanceId, huellaDigital.PesoTara ?? 0, huellaDigital.IdBalanza, nombreUsuario);
                resultado = resultadoPeso;
                peso.Set(context, huellaDigital.PesoTara);
                var recorridoActualizado = repositorio.ObtenerRecorridoPorGuid(instanceId);
                var fechaPesada = recorridoActualizado?.PesoTaraFecha ?? DateTime.Now;
                fechaTara.Set(context, fechaPesada);

                if (resultadoPeso.HayErrores)
                {
                    LogError(context, Textos.Error_ModificarPesoTara, nombreUsuario, resultado);
                    resultado.Errores.Add("1", string.Join(" ", resultado.Errores));
                    return resultado;
                }

                servicioComandos.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "IngresarPesoTaraByPass",
                        Fecha = DateTime.Now,
                        Comentario = "Peso tara por By Pass : " + huellaDigital.PesoTara,
                        NombreUsuario = nombreUsuario,
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });

                persistenceParticipant.DatosProximaActividad = null;
            }
            catch (Exception ex)
            {
                LogControlRecorrido(context, servicioComandos, ex, resultado);
            }

            try
            {
                resultado = servicioComandos.Ejecutar(new FinDeActividad { InstanceId = context.WorkflowInstanceId, Actividad = "IngresarPesoTaraByPass", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }

        private void LogActividad(CodeActivityContext context, IServicioComandos servicioComandos, string actividad, Resultado resultado)
        {
            resultado = servicioComandos.Ejecutar(new CrearLogActividad
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

        private void LogError(CodeActivityContext context, string mensaje, string usuario, Resultado resultado)
        {
            var servicioComandos = context.GetExtension<IServicioComandos>();
            resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
            {
                Dto = new ControlRecorridoDto
                {
                    Actividad = "IngresarPesoTaraByPass",
                    Fecha = DateTime.Now,
                    Comentario = mensaje,
                    NombreUsuario = usuario,
                    WorkflowInstanceId = context.WorkflowInstanceId,
                }
            });
        }

        private void LogControlRecorrido(CodeActivityContext context, IServicioComandos servicioComandos, Exception ex, Resultado resultado)
        {
            resultado = servicioComandos.Ejecutar(new CrearControlRecorrido
            {
                Dto = new ControlRecorridoDto
                {
                    Actividad = "IngresarPesoTaraByPass",
                    Fecha = DateTime.Now,
                    Comentario = $"{Textos.Error_ActualizarGenerico}: {ex.Message}",
                    NombreUsuario = "",
                    WorkflowInstanceId = context.WorkflowInstanceId,
                }
            });
        }

        private Resultado ModificarPesoTara(
            IServicioComandos servicioComandos,
            Guid instanceId,
            int pesoTara,
            int balanzaTaraId,
            string usuario)
        {
            return servicioComandos.Ejecutar(new ModificarRecorridoPeso
            {
                InstanceId = instanceId,
                TipoPesada = TipoPesada.Tara,
                Peso = pesoTara,
                BalanzaId = balanzaTaraId,
                Usuario = usuario
            });
        }
    }
}
