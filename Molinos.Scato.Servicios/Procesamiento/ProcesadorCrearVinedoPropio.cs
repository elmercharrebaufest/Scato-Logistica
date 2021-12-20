using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearVinedoPropio : ProcesadorCrear<CrearVinedoPropio, VinedoPropio>
    {
        public ProcesadorCrearVinedoPropio(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
            //TODO
        }

        protected override VinedoPropio CrearEntidad(CrearVinedoPropio comando)
        {
            var vinedoPropio = Conversor.Convertir<VinedoPropioDto, VinedoPropio>(comando.Dto);
            vinedoPropio.Proveedor = Repositorio.Obtener<Proveedor>(x => x.CodigoSap == comando.CodigoSapProveedor);
            var subZona = Repositorio.Obtener<SubZona>(comando.Dto.SubZonaId);
            vinedoPropio.SubZona = subZona;
            return vinedoPropio;
        }

        protected override void Validar(CrearVinedoPropio comando, Resultado resultado)
        {        
            if (Repositorio.Existe<VinedoPropio>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.VinedoPropioDescripcionExistente);
            }
            if (!comando.Dto.Cuarteles.Any())
            {
                resultado.Errores.Add("cuarteles", Textos.Error_VinedoPropio_CuartelRequerido);
            }

        }
    }
}
