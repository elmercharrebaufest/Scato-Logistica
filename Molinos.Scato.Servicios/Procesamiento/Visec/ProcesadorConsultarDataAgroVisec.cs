using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.DataAgroService;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarDataAgroVisec : ProcesadorComando<ConsultarDataAgroVisec>
    {
        private readonly IDataAgroServices _dataAgroServices;

        public ProcesadorConsultarDataAgroVisec(IRepositorio repositorio, IConversor conversor, ILogger log, IDataAgroServices dataAgroServices)
            : base(repositorio, conversor, log)
        {
            _dataAgroServices = dataAgroServices;
        }

        public override Resultado Ejecutar(ConsultarDataAgroVisec comando)
        {
            var resultado = new ResultadoConsultarDataAgroVisec();
            try
            {

                CupoSapTerceroDto response = _dataAgroServices.DatosCupoSap(comando.Cupo);
                if (response == null)
                {
                    resultado.Error("Error", "No se encontraron datos para el cupo especificado.");
                    return resultado;
                }
                Log.Debug($"Response: {JsonConvert.SerializeObject(response, Formatting.Indented)}");
                resultado.CodigoVariedad = response.EPA && response.EUDR ? Constantes.TipoVariedadMaterial.EPAyEUDR
                    : response.EPA ? Constantes.TipoVariedadMaterial.EPA
                    : response.EUDR ? Constantes.TipoVariedadMaterial.EUDR
                    : Constantes.TipoVariedadMaterial.Estandar;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar ConsultarDataAgroVisec");
                resultado.Error("Error", ex.Message);
            }
            return resultado;
        }
    }
}