using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarKmPorProveedor : ProcesadorModificar<ModificarKmPorProveedor>
    {
        public ProcesadorModificarKmPorProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarKmPorProveedor comando)
        {
            var kmPorProveedor = Repositorio.Obtener<KmPorProveedor>(comando.Dto.Id);

            Conversor.Convertir(comando.Dto, kmPorProveedor);
            if (kmPorProveedor.Centro.Id != comando.Dto.CentroId)
            {
                kmPorProveedor.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
            if (kmPorProveedor.Cliente.Id != comando.Dto.ClienteId)
            {
                kmPorProveedor.Cliente = Repositorio.Obtener<Cliente>(comando.Dto.ClienteId);
            }
            if (kmPorProveedor.Localidad.Provincia.Id != comando.Dto.ProvinciaId)
            {
                kmPorProveedor.Localidad.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            }
            if (kmPorProveedor.Localidad.Id != comando.Dto.LocalidadId)
            {
                kmPorProveedor.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            }
            if (kmPorProveedor.KmARecorrer != comando.Dto.KmARecorrer)
            {
                kmPorProveedor.KmARecorrer = comando.Dto.KmARecorrer;
            }
        }

        protected override void Validar(ModificarKmPorProveedor comando, Resultado resultado)
        {
            if (Repositorio.Existe<KmPorProveedor>(x => x.Id != comando.Dto.Id && (x.Centro.Id == comando.Dto.CentroId && x.Cliente.Id == comando.Dto.ClienteId && x.Localidad.Id == comando.Dto.LocalidadId)))
            {
                resultado.Error("KmPorProveedor", Textos.KmPorProveedorExistente);
            }
        }
    }
}
