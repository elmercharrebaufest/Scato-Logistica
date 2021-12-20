using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarNotificacion : ProcesadorModificar<ModificarNotificacion>
    {
        public ProcesadorModificarNotificacion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarNotificacion comando)
        {
            if (string.IsNullOrEmpty(comando.Grupos))
            {
                var notificacion = Repositorio.Obtener<Notificacion>(comando.Id);
                notificacion.Leido = true;
            }
            else
            {
                var grupos = comando.Grupos.Split(',');
                var notificaciones = Repositorio.Listar<Notificacion>(x => grupos.Any(y => y == x.Grupo) && x.TipoAlerta != TipoAlerta.Sobre && !x.Leido);
                foreach (var notificacion in notificaciones)
                {
                    notificacion.Leido = true;
                }
            }
            
        }

        protected override void Validar(ModificarNotificacion comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Notificacion>(x => x.Id == comando.Id))
            {
                resultado.Error("", Textos.Error_Generico);
            }
        }
    }
}
