using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearATAPuerto : ProcesadorCrear<CrearATAPuerto, ATAPuerto>
    {
        public ProcesadorCrearATAPuerto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ATAPuerto CrearEntidad(CrearATAPuerto comando)
        {
            return Conversor.Convertir<ATAPuertoDto, ATAPuerto>(comando.Dto);
        }

        protected override void Validar(CrearATAPuerto comando, Resultado resultado)
        {

        }
    }
}
