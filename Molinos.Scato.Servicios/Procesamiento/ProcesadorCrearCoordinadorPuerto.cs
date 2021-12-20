using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCoordinadorPuerto : ProcesadorCrear<CrearCoordinadorPuerto, CoordinadorPuerto>
    {
        public ProcesadorCrearCoordinadorPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override CoordinadorPuerto CrearEntidad(CrearCoordinadorPuerto comando)
        {
            return Conversor.Convertir<CoordinadorPuertoDto, CoordinadorPuerto>(comando.Dto);
        }

        protected override void Validar(CrearCoordinadorPuerto comando, Resultado resultado)
        {

        }
    }
}
