using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAsignacionAutomatismoNoGranoEnRecorrido : ProcesadorCrear<CrearAsignacionAutomatismoNoGranoEnRecorrido, AsignacionAutomatismoNoGranoEnRecorrido>
    {
        public ProcesadorCrearAsignacionAutomatismoNoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override AsignacionAutomatismoNoGranoEnRecorrido CrearEntidad(CrearAsignacionAutomatismoNoGranoEnRecorrido comando)
        {
            return this.Conversor.Convertir<AsignacionAutomatismoNoGranoEnRecorridoDto, AsignacionAutomatismoNoGranoEnRecorrido>(comando.Dto);
        }

        protected override void Validar(CrearAsignacionAutomatismoNoGranoEnRecorrido comando, Resultado resultado)
        {
            if (Repositorio.Existe<AsignacionAutomatismoGranoEnRecorrido>(x => x.RecorridoId == comando.Dto.RecorridoId))
            {
                resultado.Error(string.Empty, $"Ya existe el recorrido {comando.Dto.RecorridoId} en automatismo");
            }
        }
    }
}