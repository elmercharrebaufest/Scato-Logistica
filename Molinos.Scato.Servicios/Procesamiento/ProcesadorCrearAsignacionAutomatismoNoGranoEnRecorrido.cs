using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAsignacionAutomatismoNoGranoEnRecorrido : ProcesadorCrear<CrearAsignacionNoGranoEnRecorrido, AsignacionNoGranoEnRecorrido>
    {
        public ProcesadorCrearAsignacionAutomatismoNoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override AsignacionNoGranoEnRecorrido CrearEntidad(CrearAsignacionNoGranoEnRecorrido comando)
        {
            return this.Conversor.Convertir<AsignacionNoGranoEnRecorridoDto, AsignacionNoGranoEnRecorrido>(comando.Dto);
        }

        protected override void Validar(CrearAsignacionNoGranoEnRecorrido comando, Resultado resultado)
        {
            if (Repositorio.Existe<AsignacionNoGranoEnRecorrido>(x => x.RecorridoId == comando.Dto.RecorridoId))
            {
                resultado.Error(string.Empty, $"Ya existe el recorrido {comando.Dto.RecorridoId} en automatismo");
            }
        }
    }
}