using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBocaDestino : ProcesadorCrear<CrearBocaDestino, BocaDestino>
    {
        public ProcesadorCrearBocaDestino(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override BocaDestino CrearEntidad(CrearBocaDestino comando)
        {
            return new BocaDestino
            {
                Proveedor = Repositorio.Obtener<Proveedor>(x => x.Id == comando.Dto.ProveedorId),
                NombreBocaDeDestino = comando.Dto.NombreBocaDeDestino,
                Pais = Repositorio.Obtener<Pais>(x => x.Id == comando.Dto.PaisId),
                Provincia = Repositorio.Obtener<Provincia>(x => x.Id == comando.Dto.ProvinciaId),
                Localidad = Repositorio.Obtener<Localidad>(x => x.Id == comando.Dto.LocalidadId),
                Domicilio = comando.Dto.Domicilio,
                CodigoPostal = comando.Dto.CodigoPostal,
                CodigoONCCA = comando.Dto.CodigoONCCA,
            };
        }

        protected override void Validar(CrearBocaDestino comando, Resultado resultado)
        {
            if (comando.Dto.LocalidadId != 0 && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId))
            {
                resultado.Error("LocalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && !Repositorio.Existe<Provincia>(x => x.Id == comando.Dto.ProvinciaId))
            {
                resultado.Error("ProvinciaId", Textos.Error_Invalido);
            }
        }
    }
}