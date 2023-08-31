using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarExcepcionAlControl : ProcesadorModificar<ModificarExcepcionAlControl>
    {
        public ProcesadorModificarExcepcionAlControl(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarExcepcionAlControl comando)
        {
            var excepcionAlControl = Repositorio.Obtener<ExcepcionAlControl>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, excepcionAlControl);
            if (excepcionAlControl.Material.Id != comando.Dto.MaterialId)
            {
                excepcionAlControl.Material =
                    Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            }
            if (excepcionAlControl.Transportista.Id != comando.Dto.TransportistaId)
            {
                excepcionAlControl.Transportista =
                    Repositorio.Obtener<Transportista>(comando.Dto.TransportistaId);
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
            if (excepcionAlControl.ProveedorDestino == null || excepcionAlControl.ProveedorDestino.Id != comando.Dto.ProveedorDestinoId)
            {
                excepcionAlControl.ProveedorDestino = Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorDestinoId);
            }
            excepcionAlControl.Motivo = MotivoExcepcionAlControl.M;
        }

        protected override void Validar(ModificarExcepcionAlControl comando, Resultado resultado)
        {
        }
    }
}
