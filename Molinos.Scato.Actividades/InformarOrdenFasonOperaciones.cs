using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
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

            
            
            try
            {

                
                var req = new IngresosEgresosFasonesDto
                {
                    FasonId = Convert.ToInt32(ordenFasonId),
                    PesadaNeto = recorrido.PesoNeto ?? 0,
                    PesadaTara = recorrido.PesoTara ?? 0,
                    FechaIngreso = Convert.ToString(recorrido.FechaInicio),
                    FechaEgreso = Convert.ToString(recorrido.FechaEgreso),
                    UniMedCant = "Kilogramos",
                    NroRemito = cartaDePorte?.NroCTG,
                };

                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        Fecha = DateTime.Now,
                        Comentario = req.ToJson(),
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });


                servicio.Ejecutar(new InformarViajeOrdenFason
                {
                    Dto = req
                });


                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenFasonOperaciones",
                        Fecha = DateTime.Now,
                        Comentario = "Se realizó el envio a operaciones de forma exitosa",
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
