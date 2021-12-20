using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMuestraDeNirs : ProcesadorCrear<CrearMuestraDeNirs, MuestraDeNirs>
    {
        public ProcesadorCrearMuestraDeNirs(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MuestraDeNirs CrearEntidad(CrearMuestraDeNirs comando)
        {
            var muestra = Conversor.Convertir<MuestraDeNirsDto, MuestraDeNirs>(comando.Dto);
            muestra.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            muestra.Nirs = Repositorio.Obtener<Nirs>(comando.Dto.NirsId);
            return muestra;
        }

        protected override void Validar(CrearMuestraDeNirs comando, Resultado resultado)
        {
        }
    }
}
