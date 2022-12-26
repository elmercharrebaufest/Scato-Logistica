using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCallePreBalanzaPlayaInterna : ProcesadorComando<CrearCallePreBalanzaPlayaInterna>
    {
        public ProcesadorCrearCallePreBalanzaPlayaInterna(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearCallePreBalanzaPlayaInterna comando)
        {
            var resultado = new Resultado();

            var existeCallePreBalanzaPlayaInterna = Repositorio.Existe<CallePreBalanzaPlayaInterna>(q => q.CallePlayaInternaId == comando.CallePlayaInternaId && q.CallePreBalanzaId == comando.CallePreBalanzaId);

            if (!existeCallePreBalanzaPlayaInterna)
            {
                var callePreBalanza = Repositorio.Obtener<Calle>(x => x.Id == comando.CallePreBalanzaId);
                callePreBalanza.Bloqueada = true;
                callePreBalanza.FechaLLamada = DateTime.Now;

                var callePreBalanzaPlayaInternaEntity = new CallePreBalanzaPlayaInterna
                {
                    CallePreBalanzaId = comando.CallePreBalanzaId,
                    CallePlayaInternaId = comando.CallePlayaInternaId,
                    FechaLlamado = DateTime.Now
                };

                Repositorio.Agregar(callePreBalanzaPlayaInternaEntity);
                Repositorio.GuardarCambios();
            }

            return resultado;
        }
    }
}