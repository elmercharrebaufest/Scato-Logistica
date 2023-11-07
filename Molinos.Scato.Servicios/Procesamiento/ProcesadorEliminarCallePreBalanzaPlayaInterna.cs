using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarCallePreBalanzaPlayaInterna : ProcesadorComando<EliminarCallePreBalanzaPlayaInterna>
    {
        public ProcesadorEliminarCallePreBalanzaPlayaInterna(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarCallePreBalanzaPlayaInterna comando)
        {
            Log.Debug("Ejecutando EliminarCallePreBalanzaPlayaInterna");
            var callePreBalanza = Repositorio.Obtener<Calle>(x => x.Id == comando.CallePreBalanzaId);
            if (callePreBalanza != null)
            {
                callePreBalanza.Bloqueada = false;
                callePreBalanza.FechaLLamada = null;
            }

            var callePreBalanzaPlayaInterna = Repositorio.Obtener<CallePreBalanzaPlayaInterna>(q => q.CallePlayaInternaId == comando.CallePlayaInternaId && q.CallePreBalanzaId == comando.CallePreBalanzaId);
            if (callePreBalanzaPlayaInterna != null)
                Repositorio.Remover(callePreBalanzaPlayaInterna);

            Repositorio.GuardarCambios();
            return new Resultado();
        }
    }
}