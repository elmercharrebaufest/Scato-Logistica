using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarExcepcionAlControlProveedor : ProcesadorModificar<ModificarExcepcionAlControlProveedor>
    {
        public ProcesadorModificarExcepcionAlControlProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarExcepcionAlControlProveedor comando)
        {
            var excepcionAlControl = Repositorio.Obtener<ExcepcionAlControlProveedor>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, excepcionAlControl);
            if (excepcionAlControl.Material.Id != comando.Dto.MaterialId)
            {
                excepcionAlControl.Material =
                    Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            }
            if (excepcionAlControl.Proveedor.Id != comando.Dto.ProveedorId)
            {
                excepcionAlControl.Proveedor =
                    Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
            }
            if (excepcionAlControl.Centro.Id != comando.Dto.CentroId)
            {
                excepcionAlControl.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
            if (excepcionAlControl.CentroDestino == null || excepcionAlControl.CentroDestino.Id != comando.Dto.CentroDestinoId)
            {
                excepcionAlControl.CentroDestino = Repositorio.Obtener<Centro>(comando.Dto.CentroDestinoId);
            }
            if (excepcionAlControl.ClienteDestino == null || excepcionAlControl.ClienteDestino.Id != comando.Dto.ClienteDestinoId)
            {
                excepcionAlControl.ClienteDestino = Repositorio.Obtener<Cliente>(comando.Dto.ClienteDestinoId);
            }
            excepcionAlControl.Motivo = MotivoExcepcionAlControl.M;
        }

        protected override void Validar(ModificarExcepcionAlControlProveedor comando, Resultado resultado)
        {
        }
    }
}
