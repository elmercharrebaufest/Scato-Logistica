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
                PuntosDeCargaAsociados = comando.Dto.PuntosDeCargaId.Select(puntoId => Repositorio.ObtenerUnchanged<PuntoDeCarga>(puntoId)).ToList(),
                AlmacenesAsociados = comando.Dto.AlmacenesId.Select(almacenId => Repositorio.ObtenerUnchanged<Almacen>(almacenId)).ToList(),
            };
            return AutomatismoNoGranoNuevo;
        }

        protected override void Validar(CrearAutomatismoNoGrano comando, Resultado resultado)
        {
            var puntosDeCargaIds = comando.Dto.PuntosDeCargaId;
            var almacenesIds = comando.Dto.AlmacenesId;
            if (
                Repositorio.Existe<AutomatismoNoGrano>(x =>
               x.CallePlanta.Id == comando.Dto.CallePlantaId
            && x.PuntosDeCargaAsociados.Any(puntoDeCarga => puntosDeCargaIds.Contains(puntoDeCarga.Id))
            && x.AlmacenesAsociados.Any(almacen => almacenesIds.Contains(almacen.Id))
                    )
                )
            {
                resultado.Error("AutomatismoCombinacionExistente", Textos.Automatismo_CombinacionExistente);
            }
        }
    }
}