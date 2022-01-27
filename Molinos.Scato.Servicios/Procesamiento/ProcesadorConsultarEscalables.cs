using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.AfipCTGWebService;
using Molinos.Scato.Servicios.Conversiones;
using Ninject;
using Ninject.Extensions.Logging;
using RestSharp;
using Newtonsoft.Json;
using System.Configuration;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarEscalables : ProcesadorComando<ConsultarEscalables>
    {
        private readonly string URL = ConfigurationManager.AppSettings["ServicioCNRTTipoVehiculoRest"];

        public ProcesadorConsultarEscalables(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ConsultarEscalables comando)
        {

            var resultado = new ResultadoEscalables();

            try
            {
                Log.Debug("Ejecutando ProcesadorConsultarEscalables");
                ValidarConsultarEscalables(comando, resultado);
                if (resultado.HayErrores)
                {
                    return resultado;
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var client = new RestClient(URL + FormatearParametrosConsulta(comando));
                var request = new RestRequest("", Method.GET);
                var response = client.Get(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var consulta = JsonConvert.DeserializeObject<ConsultaEscalablesDto>(response.Content);
                    resultado.Categoria = consulta.Data.MapeoCategoriaEscalado;

                }
                else
                {
                    Log.Error($"No se pudo consultar el tipo de vehiculo para la patente {comando.Patente} {response.StatusCode} {response.StatusDescription} {response.ErrorException} {response.ErrorMessage}  {response.Content}");
                    resultado.Error("respuestaAfip", "No pudimos conectarnos con CNRT para consultar el tipo de vehículo, deberá completarlo manualmente.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "No se pudo consultar el tipo de vehiculo para la patente {0}", comando.Patente);
                resultado.Errores.Add("2", Textos.Error_ConexionCnrt);
            }
            return resultado;
        }

        public void ValidarConsultarEscalables(ConsultarEscalables comando, ResultadoEscalables resultado)
        {
            if (string.IsNullOrEmpty(comando.Patente))
            {
                resultado.Error("Patente", "La patente es obligatoria");
                Log.Info("Error en ProcesadorConsultarEscalables, la patente es obligatoria.");
            }
        }

        private string FormatearParametrosConsulta(ConsultarEscalables comando)
        {
            var listaPatentes = new List<string>
            {
                comando.Patente
            };
            if (!string.IsNullOrEmpty(comando.Acoplado))
                listaPatentes.Add(comando.Acoplado);
            if (!string.IsNullOrEmpty(comando.Acoplado2))
                listaPatentes.Add(comando.Acoplado2);
            return string.Join(",", listaPatentes);
        }

    }
}
