using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConversionCaracteristica : ProcesadorCrear<CrearConversionCaracteristica, ConversionCaracteristica>
    {
        public ProcesadorCrearConversionCaracteristica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ConversionCaracteristica CrearEntidad(CrearConversionCaracteristica comando)
        {
            return new ConversionCaracteristica
            {
                Camara = Repositorio.Obtener<Camara>(x => x.Id == comando.Dto.CamaraId),
                CodigoCamara = comando.Dto.CodigoCamara,
                Material = Repositorio.Obtener<Material>(x => x.Id == comando.Dto.MaterialId),
                Caracteristica = Repositorio.Obtener<CaracteristicaDeCalidad>(x => x.Id == comando.Dto.CaracteristicaId),
            };
        }

        protected override void Validar(CrearConversionCaracteristica comando, Resultado resultado)
        {
            if (comando.Dto.CamaraId != 0 && !Repositorio.Existe<Camara>(x => x.Id == comando.Dto.CamaraId))
            {
                resultado.Error("CamaraId", Textos.Error_Invalido);
            }
            if (Repositorio.Existe<ConversionCaracteristica>(x => x.Material.Id == comando.Dto.MaterialId && x.Camara.Id == comando.Dto.CamaraId && x.Caracteristica.Id == comando.Dto.CaracteristicaId))
            {
                resultado.Error("CaracteristicaId", String.Format(Textos.Error_Existente, Textos.Caracteristica));
            }
        }
    }
}
