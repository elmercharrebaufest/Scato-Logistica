using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAutomatismoNoGrano : ProcesadorModificar<ModificarAutomatismoNoGrano>
    {
        public ProcesadorModificarAutomatismoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAutomatismoNoGrano comando)
        {
            var automatismoNoGranoEditado = Repositorio.Obtener<AutomatismoNoGrano>(comando.Dto.Id);
            automatismoNoGranoEditado.Activo = comando.Dto.Activo;
            automatismoNoGranoEditado.AlmacenesAsociados.Clear();
            automatismoNoGranoEditado.PuntosDeCargaAsociados.Clear();
            IList<Almacen> almacenesAsociados = comando.Dto.AlmacenesId != null ? comando.Dto.AlmacenesId.Select(almacenId => Repositorio.ObtenerUnchanged<Almacen>(almacenId)).ToList() : comando.Dto.AlmacenesAsociados.Select(almacen => Repositorio.ObtenerUnchanged<Almacen>(almacen.Id)).ToList();
            IList<PuntoDeCarga> puntosDeCargaAsociados = comando.Dto.PuntosDeCargaId != null ? comando.Dto.PuntosDeCargaId.Select(puntoId => Repositorio.ObtenerUnchanged<PuntoDeCarga>(puntoId)).ToList() : comando.Dto.PuntosDeCargaAsociados.Select(punto => Repositorio.ObtenerUnchanged<PuntoDeCarga>(punto.Id)).ToList();
            automatismoNoGranoEditado.AlmacenesAsociados = almacenesAsociados;
            automatismoNoGranoEditado.PuntosDeCargaAsociados = puntosDeCargaAsociados;
            automatismoNoGranoEditado.CallePlanta = comando.Dto.CallePlantaId != 0 ? Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlantaId) : Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlanta.Id);
            automatismoNoGranoEditado.CallePlayaInterna = comando.Dto.CallePlayaInternaId != 0 ? Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlayaInternaId) : Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlayaInterna.Id);
        }

        protected override void Validar(ModificarAutomatismoNoGrano comando, Resultado resultado)
        {
            var puntosDeCargaIds = comando.Dto.PuntosDeCargaId ?? comando.Dto.PuntosDeCargaAsociados.Select(x => x.Id).ToList();
            var almacenesIds = comando.Dto.AlmacenesId ?? comando.Dto.AlmacenesAsociados.Select(x => x.Id).ToList();
            var callePlantaID = comando.Dto.CallePlantaId != 0 ? comando.Dto.CallePlantaId : comando.Dto.CallePlanta.Id;
            if (
                Repositorio.Existe<AutomatismoNoGrano>(x =>
               x.Id != comando.Dto.Id
            && x.CallePlanta.Id == callePlantaID
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