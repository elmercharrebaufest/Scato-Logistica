using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class InformarOrdenInsumosOperaciones : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> IdOrdenInsumos { get; set; }


        protected override void Execute(CodeActivityContext context)
        {
            var repo = context.GetExtension<IServicioRepositorio>();
            var servicio = context.GetExtension<IServicioComandos>();

            var recorrido = repo.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);
            var cartaDePorte = repo.ObtenerCartaPorteDerivadoGranarioPorGuid(context.WorkflowInstanceId);
            var ordenInsumosId = IdOrdenInsumos.Get<String>(context);

            try
            {
                servicio.Ejecutar(new CrearLogActividad
                {
                    Dto = new LogActividadDto
                    {
                        Actividad = "InformarOrdenInsumosOperaciones",
                        ActividadXaml = "InformarOrdenInsumosOperaciones",
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
                        Actividad = "InformarOrdenInsumosOperaciones",
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
                    FasonId = Convert.ToInt32(ordenInsumosId),
                    PesadaNeto = recorrido.PesoNeto ?? 0,
                    PesadaTara = recorrido.PesoTara ?? 0,
                    FechaIngreso = recorrido.FechaInicio.ToString("yyyy/MM/dd HH:mm:ss"),
                    FechaEgreso = recorrido.FechaEgreso?.ToString("yyyy/MM/dd HH:mm:ss"),
                    UniMedCant = "Kilogramos",
                    NroRemito = cartaDePorte?.NroCTG,
                };

                servicio.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "InformarOrdenInsumosOperaciones",
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
                        Actividad = "InformarOrdenInsumosOperaciones",
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
                        Actividad = "InformarOrdenInsumosOperaciones",
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
