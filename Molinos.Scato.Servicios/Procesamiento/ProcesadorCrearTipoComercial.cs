using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTipoComercial : ProcesadorCrear<CrearTipoComercial, TipoComercial>
    {
        public ProcesadorCrearTipoComercial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TipoComercial CrearEntidad(CrearTipoComercial comando)
        {
            return Conversor.Convertir<TipoComercialDto, TipoComercial>(comando.Dto);
        }

        protected override void Validar(CrearTipoComercial comando, Resultado resultado)
        {
            if (Repositorio.Existe<TipoComercial>(e => e.Descripcion == comando.Dto.Descripcion && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Descripcion ));
            }

            if (Repositorio.Existe<TipoComercial>(e => e.CodigoSap == comando.Dto.CodigoSap && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.TipoComercial_CodigoSap));
            }
        }
    }
}
