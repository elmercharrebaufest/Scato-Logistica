using System;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorMOAPaySincronizarEstadoDePagos : ProcesadorComando<MOAPaySincronizarEstadoDePagos>
    {
        public ProcesadorMOAPaySincronizarEstadoDePagos(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(MOAPaySincronizarEstadoDePagos comando)
        {
            var resultado = new Resultado();

            try
            {
                var requestBody = GenerarRequestBody(comando);

                var responseWebAPI = EnviarRequest(requestBody);

                if (responseWebAPI.IsSuccessful)
                {
                    ProcesarRespuestaExitosa(responseWebAPI, resultado);
                }
                else
                {
                    Log.Error("Error en la comunicación con la API en MOAPaySincronizarEstadoDePagos");
                    resultado.Errores.Add("Error", "Error en la comunicación con la API");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar MOAPaySincronizarEstadoDePagos");
                var errorMessage = ex.InnerException != null ? ex.Message + " " + ex.InnerException.Message : ex.Message;
                resultado.Errores.Add("Error", errorMessage);
            }

            Repositorio.GuardarCambios();
            return resultado;
        }

        private object GenerarRequestBody(MOAPaySincronizarEstadoDePagos comando)
        {
            return new
            {
                comando.Id,
                comando.Dominio,
                comando.NumeroDocumento,
                comando.TipoFecha,
                comando.FechaDesde,
                comando.FechaHasta,
                comando.Disponible,
                comando.Pagado
            };
        }

        private IRestResponse EnviarRequest(object requestBody)
        {
            var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
            var request = new RestRequest(RecursoWebAPI.MOAPAY.ObtenerPagos, Method.POST);
            request.AddJsonBody(requestBody);
            return client.Execute(request);
        }

        private void ProcesarRespuestaExitosa(IRestResponse responseWebAPI, Resultado resultado)
        {
            var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoMOAPayConsultarPagos>>(responseWebAPI.Content);

            if (response.IsValid)
            {
                ActualizarEntidadConRespuesta(response.Data);
            }
            else
            {
                Log.Error(response.Messages.FirstOrDefault()?.Message);
                resultado.Errores.Add("Error", response.Messages.FirstOrDefault()?.Message);
            }
        }

        private void ActualizarEntidadConRespuesta(ResultadoMOAPayConsultarPagos data)
        {
            foreach (var item in data.Datos)
            {
                var pagoTasaMunicipal = Repositorio.Obtener<PagosTasaMunicipal>(x => x.IdMOAPay == item.Id);
                if (pagoTasaMunicipal != null)
                {
                    pagoTasaMunicipal.NumeroDocumento = item.NumeroDocumento;
                    pagoTasaMunicipal.CuitInterviniente = item.CuitInterviniente;
                    pagoTasaMunicipal.Dominio = item.Dominio;
                }
                else
                {
                    var tipoDocumento = (!string.IsNullOrEmpty(item.NumeroDocumento) && item.NumeroDocumento.Contains("R"))
                     ? Constantes.MOAPay.TipoDocumento.REMITO
                     : Constantes.MOAPay.TipoDocumento.CTG;

                    var pago = new PagosTasaMunicipal
                    {
                        IdMOAPay = item.Id,
                        NumeroDocumento = item.NumeroDocumento,
                        CuitInterviniente = item.CuitInterviniente,
                        Dominio = item.Dominio,
                        TipoVehiculo = item.TipoVehiculo,
                        FechaPago = ParseFecha(item.FechaPago),
                        FechaEmision = (DateTime)ParseFecha(item.FechaEmision),
                        Importe = decimal.Parse(item.Importe, CultureInfo.InvariantCulture),
                        FechaAcceso = ParseFecha(item.FechaAcceso),
                        TipoDocumento = tipoDocumento,
                        Disponible = true
                    };

                    Repositorio.Agregar(pago);
                }
            }
            Repositorio.GuardarCambios();
        }

        private DateTime? ParseFecha(string fecha)
        {
            if (string.IsNullOrEmpty(fecha))
            {
                return null;
            }

            return DateTime.TryParseExact(
                fecha,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var result
            ) ? result : DateTime.MinValue;
        }
    }
}