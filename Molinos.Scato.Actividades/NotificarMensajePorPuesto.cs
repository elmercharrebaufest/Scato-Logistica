using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class NotificarMensajePorPuesto : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Mensaje { get; set; }

        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

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
                var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
                var centroId = CentroId.Get<int>(context);
                var tipoAlerta = TipoAlerta.Get<TipoAlerta>(context);


                servicio.Notificar(new NotificacionDto
                    {
                        Mensaje = mensaje,
                        Grupo = centroId + "|" + puestoDeTrabajoId.ToString(CultureInfo.CurrentCulture),
                        TipoAlerta = tipoAlerta
                    });
            }
            catch
            {
                
            }
        }
    }
}
