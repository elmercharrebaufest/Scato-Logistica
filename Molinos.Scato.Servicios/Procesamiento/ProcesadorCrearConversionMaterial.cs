using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConversionMaterial : ProcesadorCrear<CrearConversionMaterial, ConversionMaterial>
    {
        public ProcesadorCrearConversionMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ConversionMaterial CrearEntidad(CrearConversionMaterial comando)
        {
            return new ConversionMaterial
            {
                Camara = Repositorio.Obtener<Camara>(x => x.Id == comando.Dto.CamaraId),
                CodigoCamara = comando.Dto.CodigoCamara,
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
            };
        }

        protected override void Validar(CrearConversionMaterial comando, Resultado resultado)
        {
            if (comando.Dto.CamaraId != 0 && !Repositorio.Existe<Camara>(x => x.Id == comando.Dto.CamaraId))
            {
                resultado.Error("CamaraId", Textos.Error_Invalido);
            }
            if (comando.Dto.MaterialId != 0 && !Repositorio.Existe<Material>(x => x.Id == comando.Dto.MaterialId))
            {
                resultado.Error("MaterialId", Textos.Error_Invalido);
            }
            if (Repositorio.Existe<ConversionMaterial>(x => x.Material.Id == comando.Dto.MaterialId && x.Camara.Id == comando.Dto.CamaraId))
            {
                resultado.Error("MaterialId", String.Format(Textos.Error_Existente, Textos.Material));
            }
        }
    }
}
