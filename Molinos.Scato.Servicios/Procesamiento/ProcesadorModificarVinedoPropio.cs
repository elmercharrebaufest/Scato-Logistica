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
    public class ProcesadorModificarVinedoPropio : ProcesadorModificar<ModificarVinedoPropio>
    {
        public ProcesadorModificarVinedoPropio(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarVinedoPropio comando)
        {
            var cuarteles=Conversor.ConvertirList<CuartelDto,Cuartel >(comando.Dto.Cuarteles);
            var cuartelesNuevos = cuarteles.Where(x => x.Id == 0).ToList();
            var cuartelesNoEliminados = cuarteles.Where(x => x.Id != 0).ToList();

            var vinedoPropio = Repositorio.Obtener<VinedoPropio>(comando.Dto.Id);
            var subZona = Repositorio.Obtener<SubZona>(comando.Dto.SubZonaId);
            vinedoPropio.Descripcion = comando.Dto.Descripcion;
            vinedoPropio.IngresosBrutos = comando.Dto.IngresosBrutos;
            vinedoPropio.NumeroINV = comando.Dto.NumeroINV;
            vinedoPropio.IngresosBrutos = comando.Dto.IngresosBrutos;
            vinedoPropio.Calidad = comando.Dto.Calidad;
            vinedoPropio.Proveedor = Repositorio.Obtener<Proveedor>(x => x.CodigoSap == comando.CodigoSapProveedor);
            vinedoPropio.SubZona = subZona;
            vinedoPropio.CentroOperativo = comando.Dto.CentroOperativo;
            var cuartelesEliminados = vinedoPropio.Cuarteles.Where(x => cuartelesNoEliminados.All(y => y.Id != x.Id)).ToList();
            foreach (var cuartelEliminado in cuartelesEliminados)
            {
                Repositorio.Remover(cuartelEliminado);
            }
            foreach (var cuartel in cuartelesNuevos)
            {
                cuartel.VinedoPropio = vinedoPropio;
                Repositorio.Agregar(cuartel);
            }
            foreach (var cuartel in cuartelesNoEliminados)
            {
                Repositorio.Obtener<Cuartel>(cuartel.Id).Activo = cuartel.Activo;
            }
            
            
        }

        protected override void Validar(ModificarVinedoPropio comando, Resultado resultado)
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
