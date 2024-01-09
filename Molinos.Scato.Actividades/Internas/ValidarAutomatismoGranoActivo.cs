using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarAutomatismoGranoActivo : CodeActivity<Resultado>
    {
        public OutArgument<AutomatismoGranoDto> AutomatismoGrano { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var configuracionGeneral = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.Granos);
                if (configuracionGeneral == null
                    || string.IsNullOrEmpty(configuracionGeneral.Valor)
                    || !bool.TryParse(configuracionGeneral.Valor, out bool automatismoGranoGeneral)
                    || !automatismoGranoGeneral)
                {
                    resultado.Error(string.Empty, "El Automatismo Grano General está desactivado");
                    return resultado;
                }

                var automatismo = repositorio.ObtenerAutomatismoGranoPorRecorridoGuid(context.WorkflowInstanceId);
                if (automatismo == null)
                {
                    resultado.Error(string.Empty, "No existe Automatismo Grano con las características de este camión");
                    return resultado;
                }

                AutomatismoGrano.Set(context, automatismo);
            }
            catch (Exception e)
            {
                resultado.Error(string.Empty, "Ocurrió un error. " + e.Message);
            }

            return resultado;
        }
    }
}