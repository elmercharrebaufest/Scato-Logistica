using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Impl
{
    public class ServicioCircular : IServicioCircular
    {
        private readonly HttpClient httpClient;
        private readonly ILogger log;

        private readonly Uri urlBaseCircularApp = new Uri(ConfigurationManager.AppSettings["urlBaseCircularApp"]);
        private readonly string apiKeyCircularApp = ConfigurationManager.AppSettings["apiTokenCircularApp"];

        public ServicioCircular(ILogger log, HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.log = log;
        }

        public void EnviarNotificacionCamionero(string cartaDePorte, string mensaje)
        {
            var uri = new Uri(urlBaseCircularApp, "mensajes/notificar");

            var contenidoRequest = new
            {
                transportes = new string[] { cartaDePorte },
                text = mensaje
            };

            var response = PeticionApi(uri, JsonConvert.SerializeObject(contenidoRequest), HttpMethod.Post);


            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var objResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);
            }
            else
            {
                throw new Exception("Hubo un error enviando un mensaje al camionero desde Circular.");
            }
        }

        public ArriboCircularResponseDto InformarArribo(string cartaPorte, string patente, string codigoEspecieMaterial)
        {
            var uri = new Uri(urlBaseCircularApp, $"tracking/{cartaPorte}/arrival");

            var contenidoRequest = new
            {
                plates = patente,
                productId = codigoEspecieMaterial,
                eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz")
            };

            var jsonContent = JsonConvert.SerializeObject(contenidoRequest);

            var response = PeticionApi(uri, jsonContent, HttpMethod.Post);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var objResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);

                var resp = new ArriboCircularResponseDto
                {
                    SacoTurnoConCircular = (bool)objResponseContent.foundAppointment, // devuelve true si se encontró un turno para esa patente
                    LlegoEnHorario = (bool)objResponseContent.isActive //devuelve true si tiene turno y llegó en horario
                };

                return resp;
            }
            else
            {
                throw new Exception("Hubo un error informando arribo del camión a Circular.");
            }
        }

        public void CamionSalioDePlanta(string cartaPorte, bool salidaConExcepcion)
        {
            if (salidaConExcepcion)
            {
                CamionSalioDePlanta($"tracking/{cartaPorte}/annulled");
            }
            else
            {
                CamionSalioDePlanta($"tracking/{cartaPorte}/exit");
            }
        }

        public void InformarEstadoCalado(string cartaDePorte, IList<AnalisisPorCaracteristicaDto> resultadosCalado, string estado)
        {
            var uri = new Uri(urlBaseCircularApp, $"tracking/{cartaDePorte}/sampling");
            var calidadDatos = resultadosCalado.ToDictionary(x => x.Caracteristica, x => x.Valor);

            var contenidoRequest = new
            {
                qualityData = calidadDatos,
                state = estado,
                eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz")
            };

            var response = PeticionApi(uri, JsonConvert.SerializeObject(contenidoRequest), HttpMethod.Post);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var objResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);
            }
            else
            {
                throw new Exception("Hubo un error informando Estado de Calado a Circular.");
            }
        }
        private void CamionSalioDePlanta(string url)
        {
            var uri = new Uri(urlBaseCircularApp, url);

            var contenidoRequest = new
            {
                eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz")
            };

            var response = PeticionApi(uri, JsonConvert.SerializeObject(contenidoRequest), HttpMethod.Post);


            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var objResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);
            }
            else
            {
                throw new Exception("Hubo un error informando la salida del camión a Circular App.");
            }
        }

        public void InformarPesoCircular(string cartaPorte, TipoPesada tipoPesada, int peso)
        {

            if (tipoPesada == TipoPesada.Bruto)
            {
                var uri = new Uri(urlBaseCircularApp, $"tracking/{cartaPorte}/ladenWeight");
                informarPeso(uri, peso);
            }
            else
            {
                var uri = new Uri(urlBaseCircularApp, $"tracking/{cartaPorte}/unladenWeight");
                informarPeso(uri, peso);
            }
        }

        private void informarPeso(Uri uri, int? peso)
        {
            var contenidoRequest = new
            {
                weight = peso ?? 0,
                eventTime = DateTime.Now.ToString("yyyy-MM-ddTHH\\:mm\\:sszzz")
            };

            var response = PeticionApi(uri, JsonConvert.SerializeObject(contenidoRequest), HttpMethod.Post);


            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var objResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);
            }
            else
            {
                throw new Exception("Hubo un error informando la salida del camión a Circular App.");
            }
        }

        public void InformarDestinoCamionero(string cartaDePorte, string destino, string fila = "")
        {
            //solo cambia el content y url con la funcion EnviarNotificacionCamionero()
            var uri = new Uri(urlBaseCircularApp, "mensajes/destino");

            var contenidoRequest = new
            {
                transportes = new string[] { cartaDePorte },
                destino = destino,
                fila = fila
            };

            var response = PeticionApi(uri, JsonConvert.SerializeObject(contenidoRequest), HttpMethod.Post);


            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var objResponseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);
            }
            else
            {
                throw new Exception("Hubo un error enviando un mensaje al camionero desde Circular.");
            }
        }

        private HttpResponseMessage PeticionApi(Uri uri, string jsonContent, HttpMethod httpMethod)
        {
            log.Debug($"Enviando Request API Circular: url={uri}, httpMethod= {httpMethod}, Content= {jsonContent}");

            try
            {
                var contenido = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = httpMethod,
                    Headers = { { "x-api-key", apiKeyCircularApp } },
                    RequestUri = uri,
                    Content = contenido
                };

                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var response = httpClient.SendAsync(request).Result;

                return response;
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Error de conexión en la Solicitud {uri}: {e.Message}", e);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception($"{uri} enviada sin Argumentos/contenido: {e.Message}", e);
            }
        }
    }
}
