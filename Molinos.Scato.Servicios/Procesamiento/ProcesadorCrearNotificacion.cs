using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearNotificacion : ProcesadorCrear<CrearNotificacion, Notificacion>
    {
        public ProcesadorCrearNotificacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Notificacion CrearEntidad(CrearNotificacion comando)
        {
            return new Notificacion
            {
                Grupo = comando.Dto.Grupo,
                Mensaje = comando.Dto.Mensaje,
                Leido = comando.Dto.Leido,
                Hora =  comando.Dto.Hora,
                TipoAlerta = comando.Dto.TipoAlerta,
                PuestoId = comando.Dto.PuestoId
            };
        }

        protected override void Validar(CrearNotificacion comando, Resultado resultado)
        {

        }
    }
}