using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class CrearAsignacionAutomatismoNoGranoEnRecorrido : CodeActivity
    {
        [RequiredArgument]
        public InArgument<AutomatismoNoGranoDto> AutomatismoNoGrano { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var comandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var automatismoNoGrano = AutomatismoNoGrano.Get<AutomatismoNoGranoDto>(context);

            var recorridoId = repositorio.ObtenerRecorridoIdPorGuid(context.WorkflowInstanceId);
            var asignacionAutomatismoNoGrano = new Dominio.Comandos.CrearAsignacionAutomatismoNoGranoEnRecorrido()
            {
                Dto = new AsignacionAutomatismoNoGranoEnRecorridoDto()
                {
                    CallePlantaId = automatismoNoGrano.CallePlantaId,
                    RecorridoId = recorridoId,
                }
            };
            comandos.Ejecutar(asignacionAutomatismoNoGrano);
        }
    }
}