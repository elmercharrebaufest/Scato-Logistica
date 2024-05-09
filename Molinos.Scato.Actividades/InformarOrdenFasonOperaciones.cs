using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios;
using Molinos.Scato.Servicios.Procesamiento;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class InformarOrdenFasonOperaciones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> IdOrdenFason { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var repo = context.GetExtension<IServicioRepositorio>();
            var servicio = context.GetExtension<IServicioComandos>();

            var recorrido = repo.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);
            var cartaDePorte = repo.ObtenerCartaPorteDerivadoGranarioPorGuid(context.WorkflowInstanceId);
            var ordenFasonId = IdOrdenFason.Get<String>(context);

            try
            {
                servicio.Ejecutar(new CrearLogActividad
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        ActividadXaml = "InformarOrdenFasonOperaciones",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                        Fecha = DateTime.Now
                    }
                });
            }
            catch (Exception ex)
            {
                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        Fecha = DateTime.Now,
                        Comentario = ex.Message,
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
            }

            if (string.IsNullOrEmpty(ordenFasonId))
            {
                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        Fecha = DateTime.Now,
                        Comentario = "No se encontró orden de operaciones",
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
                return;
            }     
            
            try
            {
                var respuesta = servicio.Ejecutar(new InformarViajeOrdenFason
                {
                    Dto = new IngresosEgresosFasonesDto
                    {
                        FasonId = Convert.ToInt32(ordenFasonId),
                        PesadaNeto = recorrido.PesoNeto ?? 0,
                        PesadaTara = recorrido.PesoTara ?? 0,
                        FechaIngreso = Convert.ToString(recorrido.FechaInicio),
                        FechaEgreso = Convert.ToString(recorrido.FechaEgreso),
                        UniMedCant = "Kilogramos",
                        NroRemito = cartaDePorte != null ? cartaDePorte.NroCTG : string.Empty,
                    }
                });

                if (respuesta.HayErrores)
                {
                    servicio.Ejecutar(new CrearControlRecorrido
                    {
                        Dto = new ControlRecorridoDto
                        {
                            Actividad = "InformarOrdenFasonOperaciones",
                            Fecha = DateTime.Now,
                            Comentario = respuesta.Errores.Values.First(),
                            NombreUsuario = "",
                            WorkflowInstanceId = context.WorkflowInstanceId,
                        }
                    });
                    return;
                }

                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        Fecha = DateTime.Now,
                        Comentario = "Llamada exitosa",
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,

                    }
                });

            }
            catch (Exception ex)
            {
                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        Fecha = DateTime.Now,
                        Comentario = ex.Message,
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
            }

        }
    }
}
