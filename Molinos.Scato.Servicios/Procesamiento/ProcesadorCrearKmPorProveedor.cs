using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearKmPorProveedor : ProcesadorCrear<CrearKmPorProveedor, KmPorProveedor>
    {
        public ProcesadorCrearKmPorProveedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override KmPorProveedor CrearEntidad(CrearKmPorProveedor comando)
        {
            return new KmPorProveedor
            {
                Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                Cliente = Repositorio.Obtener<Cliente>(x => x.Id == comando.Dto.ClienteId),
                Localidad = Repositorio.Obtener<Localidad>(x => x.Id == comando.Dto.LocalidadId),
                KmARecorrer = comando.Dto.KmARecorrer
            };
        }

        protected override void Validar(CrearKmPorProveedor comando, Resultado resultado)
        {
            if(Repositorio.Existe<KmPorProveedor>(x => x.Centro.Id == comando.Dto.CentroId && x.Cliente.Id == comando.Dto.ClienteId && x.Localidad.Id == comando.Dto.LocalidadId))
            {
                resultado.Error("KmPorProveedor", Textos.KmPorProveedorExistente);
            }
        }
    }
}