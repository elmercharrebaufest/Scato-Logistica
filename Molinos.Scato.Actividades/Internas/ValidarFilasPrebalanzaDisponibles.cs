using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades.Internas
{
    public class ValidarFilasPrebalanzaDisponibles : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var configuracionGeneral = repositorio.ObtenerConfiguracionGeneral(Constantes.ConfiguracionGeneral.Pantalla.TableroComandoLogistica, Constantes.ConfiguracionGeneral.LlamadoAutomatico.PreBalanza);
            if (configuracionGeneral == null
                || string.IsNullOrEmpty(configuracionGeneral.Valor)
                || !bool.TryParse(configuracionGeneral.Valor, out bool llamadoPreBalanzaActivo)
                || !llamadoPreBalanzaActivo)
            {
                resultado.Error("", "La configuracion del Llamado Automático de Pre Balanza se encuentra apagado");
                return resultado;
            }
            var materialId = repositorio.ObtenerMaterialIdPorInstanceId(context.WorkflowInstanceId);

            var asignacionAutomatismo = repositorio.ObtenerAsignacionAutomatismoGranoEnRecorrido(context.WorkflowInstanceId);
            if(asignacionAutomatismo != null)
            {
                if (!repositorio.ValidarEspacioDisponibleEnCalle(asignacionAutomatismo.CallePreBalanzaId))
                    resultado.Error("", "En este momento la fila asignada por automatismo está llena");

                return resultado;
            }

            var tieneFilaDisponible = repositorio.ExisteCalleConEspacioParaAsignarSegunTipoCalleYMaterial(TipoCalle.PreBalanzaGranos, materialId);
            if (!tieneFilaDisponible)
                resultado.Error("", "En este momento no existen filas disponibles");
            return resultado;
        }
    }
}