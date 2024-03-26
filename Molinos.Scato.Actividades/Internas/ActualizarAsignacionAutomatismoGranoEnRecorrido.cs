using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ActualizarAsignacionAutomatismoGranoEnRecorrido : CodeActivity
    {
        [RequiredArgument]
        public InArgument<AutomatismoGranoDto> AutomatismoGrano { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var comandos = context.GetExtension<IServicioComandos>();
            var automatismoGrano = AutomatismoGrano.Get<AutomatismoGranoDto>(context);

            var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
            var recorridoEnAutomatismo = new Dominio.Comandos.ActualizarAsignacionAutomatismoGranoEnRecorrido
            {
                RecorridoId = recorridoId,
                CallePreBalanzaId = automatismoGrano.CallePreBalanzaId,
                CallePreHidraulicaId = automatismoGrano.CallePreHidraulicaId,
            };
            comandos.Ejecutar(recorridoEnAutomatismo);
        }
    }
}