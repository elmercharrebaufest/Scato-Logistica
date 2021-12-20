using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarBocaDestino : ProcesadorModificar<ModificarBocaDestino>
    {
        public ProcesadorModificarBocaDestino(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarBocaDestino comando)
        {
            var bocaDestino = Repositorio.Obtener<BocaDestino>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, bocaDestino);
            if (bocaDestino.Proveedor == null || bocaDestino.Proveedor.Id != comando.Dto.ProveedorId)
            {
                bocaDestino.Proveedor =
                    Repositorio.Obtener<Proveedor>(comando.Dto.ProveedorId);
            }
            if (bocaDestino.Pais == null || bocaDestino.Pais.Id != comando.Dto.PaisId)
            {
                bocaDestino.Pais =
                    Repositorio.Obtener<Pais>(comando.Dto.PaisId);
            }
            if (bocaDestino.Localidad == null || bocaDestino.Localidad.Id != comando.Dto.LocalidadId)
            {
                bocaDestino.Localidad =
                    Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            }
            if (bocaDestino.Provincia == null || bocaDestino.Provincia.Id != comando.Dto.ProvinciaId)
            {
                bocaDestino.Provincia =
                    Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            }
        }

        protected override void Validar(ModificarBocaDestino comando, Resultado resultado)
        {
            if (comando.Dto.LocalidadId != 0 && !Repositorio.Existe<Localidad>(x => x.Id != comando.Dto.LocalidadId))
            {
                resultado.Error("LocalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && !Repositorio.Existe<Provincia>(x => x.Id != comando.Dto.ProvinciaId))
            {
                resultado.Error("ProvinciaId", Textos.Error_Invalido);
            }
        }
    }
}
