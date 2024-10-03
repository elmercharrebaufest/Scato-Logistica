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
            var ordenCargaInterna = repo.ObtenerOrdenCargaInternaPorInstanceId(context.WorkflowInstanceId);

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
                var req = new IngresosEgresosResiduosDto
                {
                    IdOperaciones = Convert.ToInt32(ordenCargaInterna.Id_operaciones),
                    PesadaNeto = recorrido.PesoNeto ?? 0,
                    PesadaTara = recorrido.PesoTara ?? 0,
                    FechaEntrada = recorrido.FechaInicio.ToString("yyyy/MM/dd HH:mm:ss"),
                    FechaSalida = recorrido.FechaEgreso?.ToString("yyyy/MM/dd HH:mm:ss"),
                    UniMedCant = "Kilogramos",
                    NroCertificacion = cartaDePorte?.NroCTG,
                    OrdenCargaInterna = Convert.ToInt32(ordenCargaInterna.NumeroOrden),
                    PesadaBruto = recorrido.PesoBruto ?? 0,
                    Balanza = recorrido.BalanzaTaraId.ToString()
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


                servicio.Ejecutar(new InformarViajeOrdenesResiduos
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
