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
    public class AsignarCalle : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TipoCalle> TipoCalle { get; set; }
        public OutArgument<string> Calle { get; set; }
        public OutArgument<int> Disponibilidad { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var notificar = context.GetExtension<IServicioNotificarUsuario>();

            try
            {
                servicio.Ejecutar(new CrearLogActividad
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = "AsignarCalle",
                        ActividadXaml = "AsignarCalle",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                        Fecha = DateTime.Now
                    }
                });
            }
            catch (Exception)
            {
            }

            var comentario = "ok";
            try
            {
                var tipoCalle = TipoCalle.Get<TipoCalle>(context);

                var resultado = servicio.Ejecutar(new CrearCallePorRecorrido
                {
                    TipoCalle = tipoCalle,
                    InstanciaWorkflow = context.WorkflowInstanceId
                });
                if (resultado.HayErrores)
                {
                    comentario = resultado.Errores.Values.First();
                    Calle.Set(context, comentario ?? "No hay calles disponibles");
                }
                else
                {
                    var resultadoCrear = resultado as ResultadoCrearCalle;
                    if (resultadoCrear != null)
                    {
                        var calle = repositorio.ObtenerCalle(resultadoCrear.CalleId);
                        Calle.Set(context, calle.Nombre);
                        Disponibilidad.Set(context, resultadoCrear.Disponibilidad);
                        if (!resultadoCrear.DisponibilidadCalles )
                        {
                            if (tipoCalle == Dominio.Enums.TipoCalle.PostCalado && resultadoCrear.Calidad == TipoCalidad.Conforme)
                            {
                                notificar.Notificar(new NotificacionDto
                                {
                                    Grupo = resultadoCrear.CentroId + "|PuestoComando",
                                    Mensaje = $"¡Atencion! La { calle.Nombre } esta llena.",
                                    TipoAlerta = TipoAlerta.Advertencia,
                                    Leido = false,
                                });
                            }
                            if(tipoCalle == Dominio.Enums.TipoCalle.PlayaInterna)
                            {
                                notificar.Notificar(new NotificacionDto
                                {
                                    Grupo = resultadoCrear.CentroId + "|IngresoPlayaInterna",
                                    Mensaje = $"¡Atencion! La { calle.Nombre } esta llena.",
                                    TipoAlerta = TipoAlerta.Advertencia,
                                    Leido = false,
                                });
                            }
                        }
                        if (calle.TipoCalle == Dominio.Enums.TipoCalle.PostCalado)
                        {
                            servicio.Ejecutar(new EnviarMensajeCamioneroCircular
                            {
                                CartaPorte = repositorio.ObtenerNumeroCartaPortePorGuid(context.WorkflowInstanceId),
                                Mensaje = $"Por favor ubicarse en la fila: {calle.Nombre}"
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                comentario = e.Message;
            }

            try
            {
                if (ConfigurationManager.AppSettings["LoguearRequestsSap"] == "1")
                {
                    var srv = context.GetExtension<IServicioComandos>();
                    srv.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "AsignarCalle",
                            Fecha = DateTime.Now,
                            Comentario = comentario,
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
