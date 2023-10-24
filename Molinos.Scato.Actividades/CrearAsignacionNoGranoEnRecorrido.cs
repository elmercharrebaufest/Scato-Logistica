using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class CrearAsignacionNoGranoEnRecorrido : CodeActivity
    {                
        public InArgument<AutomatismoNoGranoDto> AutomatismoNoGrano { get; set; }

        public InArgument<bool> TieneAutomatismo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var comandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var automatismoNoGrano = AutomatismoNoGrano.Get<AutomatismoNoGranoDto>(context);
            var tieneAutomatismo= TieneAutomatismo.Get<bool>(context);

            
            var recorrido = repositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);


            var asignacionNoGrano = new Dominio.Comandos.CrearAsignacionNoGranoEnRecorrido()
            {
                Dto = new AsignacionNoGranoEnRecorridoDto()
                {
                    CallePlantaId = tieneAutomatismo ? automatismoNoGrano.CallePlantaId : repositorio.ObtenerCallePlantaIdPorMaterialId(recorrido.Material.Id),
                    RecorridoId = recorrido.Id,
                }
            };

            comandos.Ejecutar(asignacionNoGrano);
           
        }
    }
}