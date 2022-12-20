using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

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

            var existeCallePreBalanzaPlayaInterna = Repositorio.Existe<CallePreBalanzaPlayaInterna>(q => q.CallePlayaInternaId == comando.CallePlayaInternaId && q.CallePreBalanzaId == comando.CallePreBalanzaId);

            if (existeCallePreBalanzaPlayaInterna)
            {
                var callePreBalanzaPlayaInterna = Repositorio.Obtener<CallePreBalanzaPlayaInterna>(q => q.CallePlayaInternaId == comando.CallePlayaInternaId && q.CallePreBalanzaId == comando.CallePreBalanzaId);
                var callePreBalanza = Repositorio.Obtener<Calle>(x => x.Id == callePreBalanzaPlayaInterna.CallePreBalanza.Id);
                callePreBalanza.Bloqueada = false;
                callePreBalanza.FechaLLamada = null;

                Repositorio.Remover(callePreBalanzaPlayaInterna);
                Repositorio.GuardarCambios();
            }

            return resultado;
        }
    }
}