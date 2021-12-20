using System.Activities;
using Molinos.Scato.Actividades.Behaviour;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    /// <summary>
    /// Expone el Id de Instancia del workflow. Este Id es usado como correlation handle en los workflows, por ser de fácil acceso.
    /// </summary>
    public class ObtenerCentro : CodeActivity<CentroDto>
    {
        protected override CentroDto Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioRepositorio>();
            var centroId = context.GetExtension<ScatoPersistenceParticipant>().CentroId;
            return centroId.HasValue ? servicio.ObtenerCentro(centroId.Value) : null;
        }
    }
}