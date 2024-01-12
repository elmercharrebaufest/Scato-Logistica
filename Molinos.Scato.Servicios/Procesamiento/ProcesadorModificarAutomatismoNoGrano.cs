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
            automatismoNoGranoEditado.CallePlanta = comando.Dto.CallePlantaId != 0 ? Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlantaId) : Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlanta.Id);
            automatismoNoGranoEditado.CallePlayaInterna = comando.Dto.CallePlayaInternaId != 0 ? Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlayaInternaId) : Repositorio.ObtenerUnchanged<Calle>(comando.Dto.CallePlayaInterna.Id);
            automatismoNoGranoEditado.Almacen = comando.Dto.AlmacenId != 0 ? Repositorio.ObtenerUnchanged<Almacen>(comando.Dto.AlmacenId) : Repositorio.ObtenerUnchanged<Almacen>(comando.Dto.Almacen.Id);
            automatismoNoGranoEditado.PuntoDeCarga = comando.Dto.PuntoDeCargaId != 0 ? Repositorio.ObtenerUnchanged<PuntoDeCarga>(comando.Dto.PuntoDeCargaId) : Repositorio.ObtenerUnchanged<PuntoDeCarga>(comando.Dto.PuntoDeCarga.Id);
        }

        protected override void Validar(ModificarAutomatismoNoGrano comando, Resultado resultado)
        {
            var puntoDeCargaId = comando.Dto.PuntoDeCargaId != 0 ? comando.Dto.PuntoDeCargaId : comando.Dto.PuntoDeCarga.Id;
            var almacenId = comando.Dto.AlmacenId !=0 ? comando.Dto.AlmacenId: comando.Dto.Almacen.Id;
            var callePlantaId = comando.Dto.CallePlantaId != 0 ? comando.Dto.CallePlantaId : comando.Dto.CallePlanta.Id;
            if (
                Repositorio.Existe<AutomatismoNoGrano>(x =>
               x.Id != comando.Dto.Id
            && x.CallePlanta.Id == callePlantaId
            && x.PuntoDeCarga.Id == puntoDeCargaId
            && x.Almacen.Id == almacenId)
                )
            {
                resultado.Error("AutomatismoCombinacionExistente", Textos.Automatismo_CombinacionExistente);
            }
        }
    }
}