using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorMOAPayInformarPagoComoConsumido : ProcesadorComando<MOAPayInformarPagoComoConsumido>
    {
        public ProcesadorMOAPayInformarPagoComoConsumido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(MOAPayInformarPagoComoConsumido comando)
        {
            var resultado = new Resultado();

            try
            {
                var requestBody = new { comando.Id, comando.Disponible };

                var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
                var request = new RestRequest(RecursoWebAPI.MOAPAY.InformarPagoComoConsumido, Method.POST);
                request.AddJsonBody(requestBody);
                var responseWebAPI = client.Execute(request);

                if (responseWebAPI.IsSuccessful)
                {
                    var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoMOAPayInformarPagoComoConsumido>>(responseWebAPI.Content);

                    if (!response.IsValid || response.Data == null)
                    {
                        var firstMessage = response.Messages != null && response.Messages.Count > 0 ? response.Messages[0].Message : null;
                        Log.Error(firstMessage);
                        resultado.Errores.Add("Error", firstMessage);
                        return resultado;
                    }

                    if (!response.Data.Datos.Any())
                    {
                        Log.Error("No se encontraron datos en la respuesta de la API en MOAPayInformarPagoComoConsumido");
                        resultado.Errores.Add("Error", "No se encontraron datos en la respuesta de la API");
                        return resultado;
                    }

                    if (response.Data.Datos[0].FechaAcceso == null)
                    {
                        Log.Error("La fecha de acceso es nula en la respuesta de la API en MOAPayInformarPagoComoConsumido");
                        resultado.Errores.Add("Error", "La fecha de acceso es nula en la respuesta de la API");
                        return resultado;
                    }

                    if (response.Data.Datos[0].Id != comando.Id)
                    {
                        Log.Error($"El ID en la respuesta de la API ({response.Data.Datos[0].Id}) no coincide con el ID del comando ({comando.Id}) en MOAPayInformarPagoComoConsumido");
                        resultado.Errores.Add("Error", "El ID en la respuesta de la API no coincide con el ID del comando");
                        return resultado;
                    }

                    if (!resultado.HayErrores)
                    {
                        Log.Info($"MOAPayInformarPagoComoConsumido procesado correctamente para Id: {comando.Id}");
                        var recorrido = Repositorio.Obtener<Recorrido>(p => p.InstanciaWorkflow == comando.IdIntance);
                        var pagoTasaMunicipal = Repositorio.Obtener<PagosTasaMunicipal>(p => p.IdMOAPay == comando.Id);
                        if (recorrido != null && pagoTasaMunicipal != null)
                        {
                            recorrido.PagoTasaMunicipalInformado = true;
                            pagoTasaMunicipal.FechaAcceso = DateTime.Parse(response.Data.Datos[0].FechaAcceso);
                            Repositorio.GuardarCambios();
                        }
                    }
                }
                else
                {
                    Log.Error("Error en la comunicación con la API en MOAPayInformarPagoComoConsumido");
                    resultado.Errores.Add("Error", "Error en la comunicación con la API");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar MOAPayInformarPagoComoConsumido");
                var errorMessage = ex.InnerException != null ? ex.Message + " " + ex.InnerException.Message : ex.Message;
                resultado.Errores.Add("Error", errorMessage);
            }

            return resultado;
        }
    }
}