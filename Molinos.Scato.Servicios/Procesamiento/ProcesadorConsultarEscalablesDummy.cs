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
    public class ProcesadorConsultarEscalablesDummy : ProcesadorComando<ConsultarEscalablesDummy>
    {
        private readonly string URL = ConfigurationManager.AppSettings["ServicioCNRTTipoVehiculoRest"];
        private readonly IRepositorio _repositorio;
        private readonly IConversor _conversor;
        private readonly ILogger _log;
        private readonly HttpClient _httpClient;

        public ProcesadorConsultarEscalablesDummy(IRepositorio repositorio, IConversor conversor, ILogger log , HttpClient httpClient)
            : base(repositorio, conversor, log)
        {
            _repositorio = repositorio;
            _conversor = conversor;
            _log = log;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override Resultado Ejecutar(ConsultarEscalablesDummy comando)
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

                string categoria = ObtenerTipoVehiculoDummy();

                if(categoria != string.Empty)
                {
                    var rto = new ConsultaEscalableRtoDto()
                    {
                        CantEjes = categoria.Equals("B") ? "7" : "2"
                    };
                    var dominio = new ConsultaEscalableDominioDto()
                    {
                        Dominio = comando.Patente,
                        Rto = rto
                    };

                    ConsultaEscalablesDto consulta = new ConsultaEscalablesDto()
                    {
                        Data = new ConsultaEscalableValoresDto()
                        {
                            CategoriaEscalado = categoria,
                            Dominios = new List<ConsultaEscalableDominioDto>()
                            {
                                dominio
                            }
                        }

                    };
                   
                    resultado.Categoria = consulta.Data.MapeoCategoriaEscalado;
                }
                else
                {
                    _log.Error($"No se pudo consultar el tipo de vehiculo para la patente {comando.Patente} Dummy");
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

        public void ValidarConsultarEscalables(ConsultarEscalablesDummy comando, ResultadoEscalables resultado)
        {
            if (string.IsNullOrEmpty(comando.Patente))
            {
                resultado.Error("Patente", "La patente es obligatoria");
                _log.Info("Error en ProcesadorConsultarEscalables, la patente es obligatoria.");
            }
        }


        private string ObtenerTipoVehiculoDummy()
        {
            var configuracionGeneral = _repositorio.Obtener<ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.IngresarOrdenCargaInternaFason && x.Nombre == Constantes.ConfiguracionGeneral.CNRT.VehiculoDummy && x.CentroId == null);
            return configuracionGeneral.Valor;
        }
       
    }
}
