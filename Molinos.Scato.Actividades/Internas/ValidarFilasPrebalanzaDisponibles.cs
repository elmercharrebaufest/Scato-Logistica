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
            var materialId = repositorio.ObtenerMaterialIdPorInstanceId(context.WorkflowInstanceId);
            var tieneFilaDisponible = repositorio.ExisteCalleConEspacioParaAsignarSegunTipoCalleYMaterial(TipoCalle.PreBalanzaGranos,materialId);
            if (!tieneFilaDisponible)
                resultado.Error("", "En este momento no existen filas disponibles");
            return resultado;
        }
    }
}