using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConversionProcedencia : ProcesadorCrear<CrearConversionProcedencia, ConversionProcedencia>
    {
        public ProcesadorCrearConversionProcedencia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ConversionProcedencia CrearEntidad(CrearConversionProcedencia comando)
        {
            return new ConversionProcedencia
            {
                Camara = Repositorio.Obtener<Camara>(x => x.Id == comando.Dto.CamaraId),
                CodigoCamara = comando.Dto.CodigoCamara,
                Procedencia = Repositorio.Obtener<Localidad>(x => x.Id == comando.Dto.ProcedenciaId),
            };
        }

        protected override void Validar(CrearConversionProcedencia comando, Resultado resultado)
        {
            if (comando.Dto.CamaraId != 0 && !Repositorio.Existe<Camara>(x => x.Id == comando.Dto.CamaraId))
            {
                resultado.Error("CamaraId", Textos.Error_Invalido);
            }
            if (comando.Dto.ProcedenciaId != 0 && !Repositorio.Existe<Localidad>(x => x.Id == comando.Dto.ProcedenciaId))
            {
                resultado.Error("ProcedenciaId", Textos.Error_Invalido);
            }
            if (Repositorio.Existe<ConversionProcedencia>(x => x.Procedencia.Id == comando.Dto.ProcedenciaId && x.Camara.Id == comando.Dto.CamaraId))
            {
                resultado.Error("ProcedenciaId", String.Format(Textos.Error_Existente, Textos.Procedencia));
            }
        }
    }
}
