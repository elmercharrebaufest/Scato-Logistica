using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarNotificaciones : ProcesadorComando<EliminarNotificaciones>
    {
        public ProcesadorEliminarNotificaciones(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(EliminarNotificaciones comando)
        {
            var resultado = new ResultadoEliminarNotificaciones();
            var grupos = comando.Grupos.Split(',');
            var notificaciones = Repositorio.Listar<Notificacion>(x => grupos.Any(y => y == x.Grupo));
            foreach (var notificacion in notificaciones)
            {
                Repositorio.Remover(notificacion);
            }
            try
            {
                Repositorio.GuardarCambios();
            }
            catch (EntidadReferenciadaException)
            {
                resultado.Error("", Textos.Error_EliminarReferenciado);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Grupo {1}", typeof(Notificacion).Name, comando.Grupos);
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            resultado.Grupos = notificaciones.Select(x => x.Grupo).ToList();
            return resultado;
        }
    }
}