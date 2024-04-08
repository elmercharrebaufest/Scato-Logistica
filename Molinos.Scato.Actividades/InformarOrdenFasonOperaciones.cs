using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

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
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(ordenFasonId)) {
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
                            NroRemito = cartaDePorte.NroCTG
                        }
                    });
                }
                catch (Exception)
                {
                    throw;
                }
            }

           


        }
    }
}
