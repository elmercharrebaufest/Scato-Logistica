using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System.Configuration;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConsultarEscalables : ProcesadorComando<ConsultarEscalables>
    {
        private const string ErrorKeyCnrt = "CNRT";
        private readonly string _urlServicioCnrt = ConfigurationManager.AppSettings["ServicioCNRTTipoVehiculoRest"];

        private readonly IRepositorio _repositorio;
        private readonly ILogger _log;
        private readonly IServicioComandos _servicioComandos;
        private readonly HttpClient _httpClient;

        public ProcesadorConsultarEscalables(IRepositorio repositorio, IConversor conversor, ILogger log, HttpClient httpClient, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            _repositorio = repositorio;
            _log = log;
            _servicioComandos = servicioComandos;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public override Resultado Ejecutar(ConsultarEscalables comando)
        {
            var resultado = new ResultadoEscalables();

            Validar(comando, resultado);
            if (resultado.HayErrores)
                return resultado;

            return BuscarCategoriaVehiculoExistente(comando) ?? ConsultarCnrt(comando);
        }

        private ResultadoEscalables ConsultarCnrt(ConsultarEscalables comando)
        {
            var resultado = new ResultadoEscalables();

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var urlCompleta = _urlServicioCnrt + FormatearParametrosConsulta(comando);
                var response = _httpClient.GetAsync(urlCompleta).ConfigureAwait(false).GetAwaiter().GetResult();

               
                if (!response.IsSuccessStatusCode)
                {
                    _log.Error("Error en consulta CNRT. Patente: {0}, StatusCode: {1}, Reason: {2}",
                        comando.Patente, (int)response.StatusCode, response.ReasonPhrase);
                    resultado.Error(ErrorKeyCnrt, Textos.CategoriaEscalable_NoValidada);
                    return resultado;
                }

                var content = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                var consulta = JsonConvert.DeserializeObject<ConsultaEscalablesDto>(content);
                if (consulta?.Data == null)
                {
                    _log.Warn("CNRT retornó respuesta vacía o sin datos. Patente: {0}", comando.Patente);
                    resultado.Error(ErrorKeyCnrt, Textos.CategoriaEscalable_NoValidada);
                    return resultado;
                }
                resultado.Categoria = consulta.Data.MapeoCategoriaEscalado;

                if (resultado.Categoria.HasValue)
                {
                    RegistrarCategoriaVehiculo(comando, (int)resultado.Categoria.Value);
                    _log.Info("Categoría obtenida desde CNRT y registrada. Patente: {0}, Acoplado: {1}, Acoplado2: {2}, TipoVehiculo: {3}",
                        comando.Patente, comando.Acoplado, comando.Acoplado2, resultado.Categoria);
                }
                else
                {
                    _log.Warn("CNRT no retornó categoría válida. Patente: {0}, Acoplado: {1}, Acoplado2: {2}",
                        comando.Patente, comando.Acoplado, comando.Acoplado2);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error de conexión con CNRT. Patente: {0}", comando.Patente);
                resultado.Error(ErrorKeyCnrt, Textos.Error_ConexionCnrt);
            }

            return resultado;
        }

        private ResultadoEscalables BuscarCategoriaVehiculoExistente(ConsultarEscalables comando)
        {
            Expression<Func<CategoriaVehiculo, bool>> filter =
                x => x.Patente == comando.Patente
                    && (string.IsNullOrEmpty(comando.Acoplado) || x.PatenteAcoplado == comando.Acoplado)
                    && (string.IsNullOrEmpty(comando.Acoplado2) || x.PatenteAcoplado2 == comando.Acoplado2);

            var categoriaExistente = _repositorio.ObtenerPrimero(filter);

            if (categoriaExistente == null)
                return null;

            _log.Info("Categoría obtenida desde caché local. Patente: {0}, Acoplado: {1}, Acoplado2: {2}, TipoVehiculo: {3}",
                    comando.Patente, comando.Acoplado, comando.Acoplado2, categoriaExistente.TipoVehiculo);

            return new ResultadoEscalables { Categoria = (TipoVehiculo)categoriaExistente.TipoVehiculo };
        }

        private void RegistrarCategoriaVehiculo(ConsultarEscalables comando, int tipoVehiculo)
        {
            try
            {
                var nuevaCategoria = new CategoriaVehiculoDto
                {
                    Patente = comando.Patente,
                    PatenteAcoplado = comando.Acoplado,
                    PatenteAcoplado2 = comando.Acoplado2,
                    TipoVehiculo = tipoVehiculo
                };

                _servicioComandos.Ejecutar(new CrearCategoriaVehiculo { Dto = nuevaCategoria });
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Error al persistir CategoriaVehiculo. Patente: {0}, Acoplado: {1}, Acoplado2: {2}, TipoVehiculo: {3}",
                    comando.Patente, comando.Acoplado, comando.Acoplado2, tipoVehiculo);
            }
        }

        private void Validar(ConsultarEscalables comando, ResultadoEscalables resultado)
        {
            if (string.IsNullOrEmpty(comando.Patente))
                resultado.Error("Patente", "La patente es obligatoria");
        }

        private string FormatearParametrosConsulta(ConsultarEscalables comando)
        {
            var patentes = new List<string> { comando.Patente };

            if (!string.IsNullOrEmpty(comando.Acoplado))
                patentes.Add(comando.Acoplado);

            if (!string.IsNullOrEmpty(comando.Acoplado2))
                patentes.Add(comando.Acoplado2);

            return string.Join(",", patentes);
        }
    }
}
