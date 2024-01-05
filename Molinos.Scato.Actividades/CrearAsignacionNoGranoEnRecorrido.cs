using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Servicios;
using System;
using System.Activities;
using System.Linq;

namespace Molinos.Scato.Actividades
{
    public class CrearAsignacionNoGranoEnRecorrido : CodeActivity
    {                
        public InArgument<AutomatismoNoGranoDto> AutomatismoNoGrano { get; set; }

        public InArgument<bool> TieneAutomatismo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();

            var comandos = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var automatismoNoGrano = AutomatismoNoGrano.Get<AutomatismoNoGranoDto>(context);
            var tieneAutomatismo = TieneAutomatismo.Get<bool>(context);

            try
            {

                var recorrido = repositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);


                var asignacionNoGrano = new Dominio.Comandos.CrearAsignacionNoGranoEnRecorrido()
                {
                    Dto = new AsignacionNoGranoEnRecorridoDto()
                    {
                        CallePlantaId = tieneAutomatismo ? automatismoNoGrano.CallePlantaId : repositorio.ObtenerCallePlantaIdPorMaterialId(recorrido.Material.Id),
                        RecorridoId = recorrido.Id,
                    }
                };

              resultado =  comandos.Ejecutar(asignacionNoGrano);
            }
            catch (System.Exception ex)
            {

                resultado.Error(string.Empty, "Ocurrió un error. " + ex.Message);
            }

            if (resultado.HayErrores)
            {
                comandos.Ejecutar(new CrearControlRecorrido
                {
                    Dto = new ControlRecorridoDto
                    {
                        Actividad = "CrearAsignacionNoGranoEnRecorrido",
                        Fecha = DateTime.Now,
                        Comentario = resultado.Errores.Values.First(),
                        NombreUsuario = "",
                        WorkflowInstanceId = context.WorkflowInstanceId,
                    }
                });
            }

            

        }
    }
}