using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using Newtonsoft.Json;
using System.Configuration;
using Molinos.Scato.Dominio;
using Molinos.Scato.Servicios.Orquestador;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarEscalables : ProcesadorComando<ConsultarEscalables>
    {
        private readonly string URL = ConfigurationManager.AppSettings["ServicioCNRTTipoVehiculoRest"];
        private readonly IRepositorio _repositorio;
        private readonly IConversor _conversor;
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;

        public ProcesadorConsultarEscalables(IRepositorio repositorio, IConversor conversor, ILogger log , HttpClient httpClient)
            : base(repositorio, conversor, log)
        {
            _repositorio = repositorio;
            _conversor = conversor;
            _log = log;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override Resultado Ejecutar(ConsultarEscalables comando)
        {
            var resultado = new ResultadoEscalables();

            try
            {
                _log.Debug("Ejecutando ProcesadorConsultarEscalables");
                

                ValidarConsultarEscalables(comando, resultado);
                if (resultado.HayErrores)
                {
                    return resultado;
                }
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                 
                    
                    var response = _httpClient.GetAsync(URL +  FormatearParametrosConsulta(comando)).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var consulta = JsonConvert.DeserializeObject<ConsultaEscalablesDto>(content);
                        resultado.Categoria = consulta.Data.MapeoCategoriaEscalado;
                    }
                    else
                    {
                        _log.Error($"No se pudo consultar el tipo de vehiculo para la patente {comando.Patente} {response.StatusCode} {response.ReasonPhrase}");
                        resultado.Error("respuestaAfip", "No pudimos conectarnos con CNRT para consultar el tipo de vehículo, deberá completarlo manualmente.");
                    }
                
            }
            catch (Exception e)
            {
                _log.Error(e, "No se pudo consultar el tipo de vehiculo para la patente {0}", comando.Patente);
                resultado.Errores.Add("2", Textos.Error_ConexionCnrt);
            }
            return resultado;
        }

        public void ValidarConsultarEscalables(ConsultarEscalables comando, ResultadoEscalables resultado)
        {
            if (string.IsNullOrEmpty(comando.Patente))
            {
                resultado.Error("Patente", "La patente es obligatoria");
                _log.Info("Error en ProcesadorConsultarEscalables, la patente es obligatoria.");
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
            return ValidarDummyActivo() ? string.Empty : string.Join(",", listaPatentes);
        }

        private bool ValidarDummyActivo()
        {
            var configuracionGeneral = _repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason && x.Nombre == Constantes.ConfiguracionGeneral.CNRT.CNRTDummy && x.CentroId == null);
            return configuracionGeneral is null ? false : bool.Parse(configuracionGeneral.Valor);
        }
    }
}
