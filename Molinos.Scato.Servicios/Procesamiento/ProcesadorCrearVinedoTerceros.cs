using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearVinedoTerceros : ProcesadorCrear<CrearVinedoTerceros, VinedoTerceros>
    {
        public ProcesadorCrearVinedoTerceros(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
            //TODO
        }

        protected override VinedoTerceros CrearEntidad(CrearVinedoTerceros comando)
        {
            return new VinedoTerceros
            {
                Descripcion = comando.Dto.Descripcion,
                NumeroINV = comando.Dto.NumeroINV,
                CUIT_Titular = comando.Dto.CUITTitular,
                IngresosBrutos = comando.Dto.IngresosBrutos,
                Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId),
                SubZona = Repositorio.Obtener<SubZona>(comando.Dto.SubZonaId),
                Calidad = comando.Dto.Calidad
            };
        }

        protected override void Validar(CrearVinedoTerceros comando, Resultado resultado)
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
