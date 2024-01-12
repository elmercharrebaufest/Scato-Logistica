using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;
using System;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarAutomatismoNoGranoActivo : CodeActivity<Resultado>
    {
        public OutArgument<AutomatismoNoGranoDto> AutomatismoNoGrano { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            try
            {
                var configuracionGeneral = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoPuerto, Constantes.ConfiguracionGeneral.LlamadoAutomatico.NoGranos);
                if (configuracionGeneral == null
                    || string.IsNullOrEmpty(configuracionGeneral.Valor)
                    || !bool.TryParse(configuracionGeneral.Valor, out bool automatismoGranoGeneral)
                    || !automatismoGranoGeneral)
                {
                    resultado.Error(string.Empty, "El Automatismo no Grano General está desactivado");
                    return resultado;
                }

                var recorrido = repositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);

                var automatismoNoGranos = repositorio.ObtenerAutomatismoNoGranoActivoPorMaterialId(recorrido.Material.Id);

                if (automatismoNoGranos == null)
                {
                    resultado.Error(string.Empty, "No existe Automatismo No Grano con las características de este camión");
                    return resultado;
                }

                AutomatismoNoGrano.Set(context, automatismoNoGranos);
            }
            catch (Exception e)
            {
                resultado.Error(string.Empty, "Ocurrió un error. " + e.Message);
            }

            return resultado;
        }
    }
}