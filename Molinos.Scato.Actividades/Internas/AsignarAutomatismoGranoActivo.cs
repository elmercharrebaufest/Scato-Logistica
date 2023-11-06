using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class AsignarAutomatismoGranoActivo : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<AutomatismoGranoDto> AutomatismoGrano { get; set; }

        public OutArgument<bool> AutomatismoGranoAsignado { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var comandos = context.GetExtension<IServicioComandos>();
            var automatismoGrano = AutomatismoGrano.Get<AutomatismoGranoDto>(context);

            try
            {
                var asignacion = new AsignacionDto()
                {
                    AlmacenId = automatismoGrano.AlmacenId,
                    CalleId = automatismoGrano.CallePreHidraulicaId,
                    HidraulicasId = automatismoGrano.Hidraulicas.ToArray(),
                    InstanceIds = context.WorkflowInstanceId.ToString()
                };

                resultado = comandos.Ejecutar(new ActualizarPuestocomando { Dto = asignacion });
                if (!resultado.HayErrores)
                    AutomatismoGranoAsignado.Set(context, true);
            }
            catch (Exception e)
            {
                resultado.Error(string.Empty, "Ocurrió un error. " + e.Message);
            }

            return resultado;
        }
    }
}