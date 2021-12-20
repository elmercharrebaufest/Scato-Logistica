using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConversionCentro: ProcesadorCrear<CrearConversionCentro,ConversionCentro>
    {
        public ProcesadorCrearConversionCentro(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override ConversionCentro CrearEntidad(CrearConversionCentro comando)
        {
            return new ConversionCentro
                {
                    Camara = Repositorio.Obtener<Camara>(x => x.Id == comando.Dto.CamaraId),
                    Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId),
                    CodigoCamara = comando.Dto.CodigoCamara
                };
        }

        protected override void Validar(CrearConversionCentro comando, Resultado resultado)
        {
            if (comando.Dto.CamaraId != 0 && !Repositorio.Existe<Camara>(x => x.Id == comando.Dto.CamaraId))
            {
                resultado.Error("CamaraId", Textos.Error_Invalido);
            }
            if (comando.Dto.CentroId != 0 && !Repositorio.Existe<Centro>(x => x.Id == comando.Dto.CentroId))
            {
                resultado.Error("CentroId", Textos.Error_Invalido);
            }
            if (Repositorio.Existe<ConversionCentro>(x => x.Centro.Id == comando.Dto.CentroId && x.Camara.Id == comando.Dto.CamaraId))
            {
                resultado.Error("CentroId", String.Format(Textos.Error_Existente, Textos.Centro));
            }
        }
    }
}
