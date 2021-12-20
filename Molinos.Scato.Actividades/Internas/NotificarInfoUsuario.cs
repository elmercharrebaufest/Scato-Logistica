using System.Activities;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades.Internas
{
    public class NotificarInfoUsuario : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Mensaje { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<string> Grupo { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicio = context.GetExtension<IServicioNotificarUsuario>();
                var mensaje = Mensaje.Get<string>(context);
                var centroId = CentroId.Get<int>(context);
                var grupo = Grupo.Get<string>(context);

                servicio.Notificar(new NotificacionDto { Grupo = centroId +  "|" + grupo, Mensaje = mensaje, TipoAlerta = TipoAlerta.Advertencia});
            }
            catch
            {
                
            }
        }
    }
}
