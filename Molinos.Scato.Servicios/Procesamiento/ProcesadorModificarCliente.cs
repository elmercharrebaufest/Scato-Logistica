using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCliente : ProcesadorModificar<ModificarCliente>
    {
        public ProcesadorModificarCliente(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCliente comando)
        {
            var cliente = Repositorio.Obtener<Cliente>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, cliente);
            cliente.Bloqueado = comando.Dto.Bloqueado;
            cliente.Descripcion = comando.Dto.Descripcion;
            cliente.Cuit = comando.Dto.Cuit;
            cliente.Direccion = comando.Dto.Direccion;
            cliente.Localidad = comando.Dto.Localidad;
            cliente.Provincia = comando.Dto.Provincia;
            cliente.Activo = comando.Dto.Activo;
            cliente.CodigoSap = comando.Dto.CodigoSap;
            cliente.EsClienteProvisorio = comando.Dto.EsClienteProvisorio;
        
        }

        protected override void Validar(ModificarCliente comando, Resultado resultado)
        {

        }
    }
}
