using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVinedoTerceros : ProcesadorModificar<ModificarVinedoTerceros>
    {
        public ProcesadorModificarVinedoTerceros(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarVinedoTerceros comando)
        {

            var vinedoTerceros = Repositorio.Obtener<VinedoTerceros>(comando.Dto.Id);
            vinedoTerceros.Descripcion = comando.Dto.Descripcion;
            vinedoTerceros.IngresosBrutos = comando.Dto.IngresosBrutos;
            vinedoTerceros.NumeroINV = comando.Dto.NumeroINV;
            vinedoTerceros.Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId);
            vinedoTerceros.SubZona = Repositorio.Obtener<SubZona>(comando.Dto.SubZonaId);
            vinedoTerceros.Calidad = comando.Dto.Calidad;

        }

        protected override void Validar(ModificarVinedoTerceros comando, Resultado resultado)
        {
            if (Repositorio.Existe<VinedoTerceros>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.VinedoTercerosDescripcionExistente);
            }
            if (!comando.Dto.esProveedorPR)
            {
                resultado.Error("Proveedor", Textos.VinedoTerceros_ProveedorInvalido);
            }
        }
    
    }
}
