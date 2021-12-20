using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEstablecimiento : ProcesadorModificar<ModificarEstablecimiento>
    {
        public ProcesadorModificarEstablecimiento(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEstablecimiento comando)
        {
            var establecimiento = Repositorio.Obtener<Establecimiento>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, establecimiento);
            if (establecimiento.Proveedor == null || establecimiento.Proveedor.Id != comando.Dto.ProveedorId)
            {
                establecimiento.Proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
            }
            if (establecimiento.Localidad == null || establecimiento.Localidad.Id != comando.Dto.LocalidadId)
            {
                establecimiento.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            }
            if (establecimiento.Provincia == null || establecimiento.Provincia.Id != comando.Dto.ProvinciaId)
            {
                establecimiento.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            }
        }

        protected override void Validar(ModificarEstablecimiento comando, Resultado resultado)
        {
            if (Repositorio.Existe<Establecimiento>(x => x.Id != comando.Dto.Id && x.Proveedor.Id == comando.Dto.ProveedorId && x.NombreDeEstablecimiento == comando.Dto.NombreDeEstablecimiento))
            {
                resultado.Error("NombreDeEstablecimiento", string.Format(Textos.Error_Existente, Textos.Establecimiento_Nombre));
            }
        }
    }
}
