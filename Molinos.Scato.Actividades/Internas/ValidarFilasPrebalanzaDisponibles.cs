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
            
            var asignacionAutomatismo = repositorio.ObtenerAsignacionAutomatismoGranoEnRecorrido(context.WorkflowInstanceId);

            if(asignacionAutomatismo != null)
            {
                if (!repositorio.ValidarDisponibilidadAsignacionEnCallePreBalanza(asignacionAutomatismo.CallePreBalanzaId, asignacionAutomatismo.CallePreHidraulicaId))
                    resultado.Error("", "No es posible ingresar a la fila asignada por automatismo");

                var callePb = repositorio.ObtenerCalle(asignacionAutomatismo.CallePreBalanzaId);

                var recorrido = repositorio.ObtenerRecorridoPorGuid(context.WorkflowInstanceId);

                if(callePb.MaterialId != recorrido.Material.Id)
                {
                    resultado.Error("", "No es posible ingresar a la fila asignada por automatismo porque el material no coincide");
                }


                return resultado;
            }

            var tieneFilaDisponible = repositorio.ExisteCalleConEspacioParaAsignarSegunTipoCalleYMaterial(TipoCalle.PreBalanzaGranos, context.WorkflowInstanceId);
            if (!tieneFilaDisponible)
                resultado.Error("", "En este momento no existen filas disponibles");
            return resultado;
        }
    }
}