using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTransportista : ProcesadorModificar<ModificarTransportista>
    {
        public ProcesadorModificarTransportista(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTransportista comando)
        {
            var transportista = Repositorio.Obtener<Transportista>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, transportista);
            if (transportista.Localidad == null || transportista.Localidad.Id != comando.Dto.LocalidadId)
            {
                transportista.Localidad =
                    Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            }
            if (transportista.Provincia == null || transportista.Provincia.Id != comando.Dto.ProvinciaId)
            {
                transportista.Provincia =
                    Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            }
        }

        protected override void Validar(ModificarTransportista comando, Resultado resultado)
        {
            if (Repositorio.Existe<Transportista>(x => x.Id != comando.Dto.Id && x.Cuit == comando.Dto.Cuit))
            {
                resultado.Error("Cuit", Textos.Transportista_CuitExistente);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id != comando.Dto.LocalidadId))
            {
                resultado.Error("LocalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Provincia>(x => x.Id != comando.Dto.ProvinciaId))
            {
                resultado.Error("ProvinciaId", Textos.Error_Invalido);
            }
        }
    }
}
