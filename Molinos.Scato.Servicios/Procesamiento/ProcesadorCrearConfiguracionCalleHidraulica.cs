
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
                CodigoCartel = comando.Dto.CodigoCartel,
                CodigoSensorCamaraALPR = comando.Dto.CodigoSensorCamaraALPR,
                CodigoSensorCirculacion = comando.Dto.CodigoSensorCirculacion,
                CodigoCamaraALPR = comando.Dto.CodigoCamaraALPR
             };
        }

        protected override void Validar(CrearConfiguracionCalleHidraulica comando, Resultado resultado)
        {
            //if (Repositorio.Existe<ConfiguracionCalleHidraulica>(e => e.Codigo == comando.Dto.Codigo && e.Centro.Id == comando.Dto.CentroId && (e.Id != comando.Dto.Id)))
            //{
            //    resultado.Error("Codigo", Textos.Hidraulica_CodigoExistente);
            //}
            //if (Repositorio.Existe<ConfiguracionCalleHidraulica>(e => e.CodigoSensorBajada == comando.Dto.CodigoSensorBajada))
            //{
            //    resultado.Error("CodigoSensorBajada", string.Format(Textos.Error_Existente, Textos.PuestosDeCargaDescarga_CodigoSensorBajada));
            //}
        }
    }
}
