using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
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

        protected override void Finally(CrearNotificacion comando, int id)
        {
            if (comando.Dto.TipoAlerta != TipoAlerta.Automatica
                || !comando.Dto.PuestoId.HasValue
                || !comando.Dto.Leido)
                return;

            var puestoId = comando.Dto.PuestoId.Value;
            var previas = Repositorio.Listar<Notificacion>(
                x => x.PuestoId == puestoId
                  && x.TipoAlerta == TipoAlerta.Automatica
                  && !x.Leido
                  && x.Id != id);

            if (!previas.Any())
                return;

            foreach (var notificacion in previas)
            {
                notificacion.Leido = true;
            }
            Repositorio.GuardarCambios();
        }
    }
}