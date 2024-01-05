using Molinos.Scato.Dominio;
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
            var resultado = new Resultado();
            if (Validar(comando, resultado))
                return resultado;

            if(comando.CodigoAutomatismoTipoLlamado != Constantes.AutomatismoTipoLlamado.UnoAUno)
            {
                var callePreBalanza = Repositorio.Obtener<Calle>(x => x.Id == comando.CallePreBalanzaId);
                callePreBalanza.Bloqueada = false;
                callePreBalanza.FechaLLamada = null;
            }

            var callePreBalanzaPlayaInterna = comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno
                                                ? Repositorio.Obtener<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.RecorridoId == comando.RecorridoId.Value)
                                                : Repositorio.Obtener<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.CallePreBalanzaId == comando.CallePreBalanzaId);
            Repositorio.Remover(callePreBalanzaPlayaInterna);
            Repositorio.GuardarCambios();

            return resultado;
        }

        private bool Validar(EliminarCallePreBalanzaPlayaInterna comando, Resultado resultado)
        {
            if (comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno && !comando.RecorridoId.HasValue)
                resultado.Error(string.Empty, "El Recorrido es requerido");

            if (comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno && comando.RecorridoId.HasValue && !Repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.RecorridoId == comando.RecorridoId.Value))
                resultado.Error(string.Empty, "No existe el Camion PreBalanza Llamado");

            if (comando.CodigoAutomatismoTipoLlamado != Constantes.AutomatismoTipoLlamado.UnoAUno && !Repositorio.Existe<Calle>(x => x.Id == comando.CallePreBalanzaId))
                resultado.Error(string.Empty, "No existe la Calle PreBalanza");

            if (comando.CodigoAutomatismoTipoLlamado != Constantes.AutomatismoTipoLlamado.UnoAUno && !Repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.CallePreBalanzaId == comando.CallePreBalanzaId))
                resultado.Error(string.Empty, "No existe la Calle PreBalanza Llamada");

            return resultado.HayErrores;
        }
    }
}