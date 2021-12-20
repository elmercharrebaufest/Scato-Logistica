using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTipoDocumentoIdentidad : ProcesadorModificar<ModificarTipoDocumentoIdentidad>
    {
        public ProcesadorModificarTipoDocumentoIdentidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTipoDocumentoIdentidad comando)
        {
            var tipoDocIdEntidad = Repositorio.Obtener<TipoDocumentoIdentidad>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto,tipoDocIdEntidad);
        }

        protected override void Validar(ModificarTipoDocumentoIdentidad comando, Resultado resultado)
        {
            if (Repositorio.Existe<TipoDocumentoIdentidad>(e => e.Descripcion == comando.Dto.Descripcion && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Descripcion));
            }
            if (Repositorio.Existe<TipoDocumentoIdentidad>(e => e.DescripcionCorta == comando.Dto.DescripcionCorta && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("DescripcionCorta", string.Format(Textos.Error_Existente, Textos.DescripcionCorta));
            }
            if (Repositorio.Existe<TipoDocumentoIdentidad>(e => e.CodigoSap == comando.Dto.CodigoSap && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.TipoDocumentoIdentidad_CodigoSAP));
            }
        }
    }
}
