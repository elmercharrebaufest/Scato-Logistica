using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearEstadoPuerto : ProcesadorCrear<CrearEstadoPuerto, EstadoPuerto>
    {
        public ProcesadorCrearEstadoPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override EstadoPuerto CrearEntidad(CrearEstadoPuerto comando)
        {
            return Conversor.Convertir<EstadoPuertoDto, EstadoPuerto>(comando.Dto);
        }

        protected override void Validar(CrearEstadoPuerto comando, Resultado resultado)
        {

        }
    }
}