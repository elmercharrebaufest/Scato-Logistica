using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBajaCTG : ProcesadorCrear<CrearBajaCTG, BajaCTG>
    {
        public ProcesadorCrearBajaCTG(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override BajaCTG CrearEntidad(CrearBajaCTG comando)
        {
            var baja = Conversor.Convertir<BajaCTGDto, BajaCTG>(comando.Dto);
            baja.CartaPorte = Repositorio.Obtener<CartaPorte>(x => x.Id == comando.Dto.CartaPorteId);
            return baja;
        }

        protected override void Validar(CrearBajaCTG comando, Resultado resultado)
        {
        }
    }
}
