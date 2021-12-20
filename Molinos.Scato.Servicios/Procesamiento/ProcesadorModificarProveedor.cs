using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarProveedor : ProcesadorModificar<ModificarProveedor>
    {
        public ProcesadorModificarProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarProveedor comando)
        {
            var proveedor = Repositorio.Obtener<Proveedor>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, proveedor);
        }

        protected override void Validar(ModificarProveedor comando, Resultado resultado)
        {
        }
    }
}
