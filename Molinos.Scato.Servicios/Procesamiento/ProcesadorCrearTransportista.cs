using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTransportista : ProcesadorCrear<CrearTransportista, Transportista>
    {
        public ProcesadorCrearTransportista(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Transportista CrearEntidad(CrearTransportista comando)
        {
            return new Transportista
            {
                Cuit = comando.Dto.Cuit,
                RazonSocial = comando.Dto.RazonSocial,
                Domicilio = comando.Dto.Domicilio,
                Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId),
                Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId),
                MedioDePago = comando.Dto.MedioDePago
            };
        }

        protected override void Validar(CrearTransportista comando, Resultado resultado)
        {
            if (Repositorio.Existe<Transportista>(x => x.Id != comando.Dto.Id && x.Cuit == comando.Dto.Cuit))
            {
                resultado.Error("Cuit", Textos.Transportista_CuitExistente);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId))
            {
                resultado.Error("LocalidadId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Provincia>(x => x.Id == comando.Dto.ProvinciaId))
            {
                resultado.Error("ProvinciaId", Textos.Error_Invalido);
            }
            if (comando.Dto.LocalidadId != 0 && comando.Dto.LocalidadId != null && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.LocalidadId && x.Provincia.Id == comando.Dto.ProvinciaId))
            {
                resultado.Error("LocalidadId", Textos.Error_LocalidadInvalida);
            }
        }
    }
}