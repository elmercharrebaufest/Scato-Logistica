using System.Activities;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class NotificarMensaje : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Mensaje { get; set; }

        [RequiredArgument]
        public InArgument<string> NombreUsuario { get; set; }

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }

        [RequiredArgument]
        public InArgument<TipoAlerta> TipoAlerta { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            try
            {
                var servicio = context.GetExtension<IServicioNotificarUsuario>();
                var mensaje = Mensaje.Get<string>(context);
                var nombreUsuario = NombreUsuario.Get<string>(context);
                var centroId = CentroId.Get<int>(context);
                var tipoAlerta = TipoAlerta.Get<TipoAlerta>(context);


                servicio.Notificar(new NotificacionDto
                    {
                        Mensaje = mensaje,
                        Grupo = centroId + "|" + nombreUsuario,
                        TipoAlerta = tipoAlerta
                    });
            }
            catch
            {
                
            }
        }
    }
}
