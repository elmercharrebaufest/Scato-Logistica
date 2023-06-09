using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;
using System.Activities;

namespace Molinos.Scato.Actividades
{
    public class ConfirmarCTGVencidos : CodeActivity<Resultado>
    {
        protected override Resultado Execute(CodeActivityContext context)
        {
            var resultado = new Resultado();
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var centroId = repositorio.ObtenerCentroIdPorInstanceId(context.WorkflowInstanceId);

            servicio.Ejecutar(new ConfirmarCTGVencidas
            {
                CentroId = centroId,
                TipoPerfil = TipoPerfil.Solicitante,
            });

            return resultado;
        }
    }
}