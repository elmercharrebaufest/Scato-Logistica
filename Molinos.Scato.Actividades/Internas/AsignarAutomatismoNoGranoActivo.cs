using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class AsignarAutomatismoNoGranoActivo : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<AutomatismoNoGranoDto> AutomatismoNoGrano { get; set; }

        public OutArgument<bool> AutomatismoNoGranoAsignado { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var comandos = context.GetExtension<IServicioComandos>();
            var automatismoNoGrano = AutomatismoNoGrano.Get<AutomatismoNoGranoDto>(context);

            try
            {
                var asignacion = new AsignacionDto()
                {
                    AlmacenId = automatismoNoGrano.AlmacenId,
                    CalleId = automatismoNoGrano.CallePlayaInternaId,
                    PuntoDeCargaId = automatismoNoGrano.PuntoDeCargaId,
                    InstanceIds = context.WorkflowInstanceId.ToString(),
                    HidraulicasId = new int[] {},
                };

                resultado = comandos.Ejecutar(new ActualizarPuestoComandoPuerto { Dto = asignacion });

                if (!resultado.HayErrores)
                    AutomatismoNoGranoAsignado.Set(context, true);
            }
            catch (Exception e)
            {
                resultado.Error(string.Empty, "Ocurrió un error. " + e.Message);
            }

            return resultado;
        }
    }
}