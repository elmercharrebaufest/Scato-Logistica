using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAutomatismoNoGrano : ProcesadorCrear<CrearAutomatismoNoGrano, AutomatismoNoGrano>
    {
        public ProcesadorCrearAutomatismoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AutomatismoNoGrano CrearEntidad(CrearAutomatismoNoGrano comando)
        {
            var AutomatismoNoGranoNuevo = new AutomatismoNoGrano
            {
                CallePlanta = Repositorio.Obtener<Calle>(x => x.Id == comando.Dto.CallePlantaId),
                CallePlayaInterna = Repositorio.Obtener<Calle>(x => x.Id == comando.Dto.CallePlayaInternaId),
                PuntoDeCarga = Repositorio.Obtener<PuntoDeCarga>(x => x.Id == comando.Dto.PuntoDeCargaId),
                Almacen = Repositorio.Obtener<Almacen>(x => x.Id == comando.Dto.AlmacenId)
            };
            return AutomatismoNoGranoNuevo;
        }

        protected override void Validar(CrearAutomatismoNoGrano comando, Resultado resultado)
        { 
            if (Repositorio.Existe<AutomatismoNoGrano>(x =>
               x.CallePlanta.Id == comando.Dto.CallePlantaId
            && x.PuntoDeCarga.Id == comando.Dto.CallePlantaId
            && x.Almacen.Id == comando.Dto.AlmacenId))
            {
                resultado.Error("AutomatismoCombinacionExistente", Textos.Automatismo_CombinacionExistente);
            }
        }
    }
}