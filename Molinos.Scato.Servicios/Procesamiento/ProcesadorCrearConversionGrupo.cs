using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConversionGrupo : ProcesadorCrear<CrearConversionGrupo, ConversionGrupo>
    {
        public ProcesadorCrearConversionGrupo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ConversionGrupo CrearEntidad(CrearConversionGrupo comando)
        {
            return new ConversionGrupo
            {
                Camara = Repositorio.Obtener<Camara>(x => x.Id == comando.Dto.CamaraId),
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
                CodigoSegunCamara = comando.Dto.CodigoSegunCamara
            };
        }

        protected override void Validar(CrearConversionGrupo comando, Resultado resultado)
        {
            if (comando.Dto.CamaraId != 0 && !Repositorio.Existe<Camara>(x => x.Id == comando.Dto.CamaraId))
            {
                resultado.Error("CamaraId", Textos.Error_Invalido);
            }
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id == comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
            if (Repositorio.Existe<ConversionGrupo>(x => x.CodigoSegunCamara == comando.Dto.CodigoSegunCamara && x.Camara.Id == comando.Dto.CamaraId && x.Material.Id == comando.Dto.MaterialId))
            {
                resultado.Error("CodigoSegunCamara", string.Format(Textos.Error_Existente, Textos.CodigoSegunCamara));
            }
        }
    }
}
