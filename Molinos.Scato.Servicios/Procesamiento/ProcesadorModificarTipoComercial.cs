using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTipoComercial : ProcesadorModificar<ModificarTipoComercial>
    {
        public ProcesadorModificarTipoComercial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTipoComercial comando)
        {
            var tipoDocIdEntidad = Repositorio.Obtener<TipoComercial>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto,tipoDocIdEntidad);
        }

        protected override void Validar(ModificarTipoComercial comando, Resultado resultado)
        {
            if (Repositorio.Existe<TipoComercial>(e => e.Descripcion == comando.Dto.Descripcion && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Descripcion));
            }

            if (Repositorio.Existe<TipoComercial>(e => e.CodigoSap == comando.Dto.CodigoSap && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoSap", string.Format(Textos.Error_Existente, Textos.TipoComercial_CodigoSap));
            }
        }
    }
}
