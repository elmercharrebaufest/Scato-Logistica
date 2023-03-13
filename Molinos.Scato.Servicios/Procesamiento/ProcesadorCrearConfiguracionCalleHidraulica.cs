
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearConfiguracionCalleHidraulica : ProcesadorCrear<CrearConfiguracionCalleHidraulica, ConfiguracionCalleHidraulica>
    {
        public ProcesadorCrearConfiguracionCalleHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ConfiguracionCalleHidraulica CrearEntidad(CrearConfiguracionCalleHidraulica comando)
        {
            return new ConfiguracionCalleHidraulica
            {
                CodigoCamaraALPR = comando.Dto.CodigoCamaraALPR,
                CodigoCartel = comando.Dto.CodigoCartel,
                CodigoSensorCirculacion = comando.Dto.CodigoSensorCirculacion,
                CodigoSensorCamaraALPR = comando.Dto.CodigoSensorCamaraALPR,
                Calle = Repositorio.Obtener<Calle>(comando.Dto.CalleId)
            };
        }

        protected override void Validar(CrearConfiguracionCalleHidraulica comando, Resultado resultado)
        {
            if (Repositorio.Existe<ConfiguracionCalleHidraulica>(x => x.Calle.Id == comando.Dto.CalleId))
            {
                resultado.Error("CalleId", string.Format(Textos.Error_Existente, Textos.Calle));
            }
        }
    }
}
