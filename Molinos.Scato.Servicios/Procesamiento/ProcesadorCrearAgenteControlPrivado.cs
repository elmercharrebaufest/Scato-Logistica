using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAgenteControlPrivado : ProcesadorCrear<CrearAgenteControlPrivado, AgenteControlPrivado>
    {
        public ProcesadorCrearAgenteControlPrivado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AgenteControlPrivado CrearEntidad(CrearAgenteControlPrivado comando)
        {
            return Conversor.Convertir<AgenteControlPrivadoDto, AgenteControlPrivado>(comando.Dto);
        }

        protected override void Validar(CrearAgenteControlPrivado comando, Resultado resultado)
        {

        }
    }
}
