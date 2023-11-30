using Molinos.Scato.Dominio;
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
            Validar(comando, resultado);

            if (resultado.HayErrores)
                return resultado;

            if(comando.CodigoAutomatismoTipoLlamado != Constantes.AutomatismoTipoLlamado.UnoAUno)
            {
                var callePreBalanza = Repositorio.Obtener<Calle>(x => x.Id == comando.CallePreBalanzaId);
                callePreBalanza.Bloqueada = true;
                callePreBalanza.FechaLLamada = DateTime.Now;
            }

            var entidad = new CallePreBalanzaPlayaInterna
            {
                CallePreBalanzaId = comando.CallePreBalanzaId,
                CallePlayaInternaId = comando.CallePlayaInternaId,
                FechaLlamado = DateTime.Now,
                CodigoAutomatismoTipoLlamado = comando.CodigoAutomatismoTipoLlamado,
                EsCamionEnEspera = comando.EsCamionEnEspera,
            };

            if (comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno && comando.RecorridoId.HasValue)
                entidad.RecorridoId = comando.RecorridoId.Value;

            Repositorio.Agregar(entidad);
            Repositorio.GuardarCambios();


            return resultado;
        }

        private void Validar(CrearCallePreBalanzaPlayaInterna comando, Resultado resultado)
        {
            if (comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.PaseDirecto && Repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.PaseDirecto))
                resultado.Error(string.Empty, "Ya existe una Calle PreBalanza llamada por Pase Directo");

            if (!comando.EsCamionEnEspera && comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno && Repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && !x.EsCamionEnEspera && x.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno))
                resultado.Error(string.Empty, "Ya existe un Camion Llamado por 1 a 1");

            if (comando.EsCamionEnEspera && comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno && Repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.EsCamionEnEspera && x.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.UnoAUno))
                resultado.Error(string.Empty, "Ya existe un Camion En Espera por 1 a 1");

            if (comando.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.PorFila && Repositorio.Existe<CallePreBalanzaPlayaInterna>(x => x.CallePlayaInternaId == comando.CallePlayaInternaId && x.CallePreBalanzaId == comando.CallePreBalanzaId && x.CodigoAutomatismoTipoLlamado == Constantes.AutomatismoTipoLlamado.PorFila))
                resultado.Error(string.Empty, "La Calle PreBalanza ya ha sido llamada por Fila");
        }
    }
}