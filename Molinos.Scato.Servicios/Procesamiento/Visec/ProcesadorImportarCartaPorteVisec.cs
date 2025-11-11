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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImportarCartaPorteVisec : ProcesadorComando<ImportarCartaPorteVisec>
    {
        public ProcesadorImportarCartaPorteVisec(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ImportarCartaPorteVisec comando)
        {
            var resultado = new Resultado();

            try
            {
                var entidad = PrepararEntidad(comando.Id);
                var requestBody = GenerarRequestBody(entidad);

                var responseWebAPI = EnviarRequest(requestBody);

                if (responseWebAPI.IsSuccessful)
                {
                    ProcesarRespuestaExitosa(entidad, responseWebAPI,resultado);
                }
                else
                {
                    resultado.Errores.Add("Error", "Error en la comunicación con la API");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al procesar ImportarCartaPorteVisec");
                var errorMessage = ex.InnerException != null ? ex.Message + " " + ex.InnerException.Message : ex.Message;
                resultado.Errores.Add("Error", errorMessage);
            }

            Repositorio.GuardarCambios();
            return resultado;
        }

        private VisecTransmision PrepararEntidad(int id)
        {
            var includes = new List<Expression<Func<VisecTransmision, object>>> { x => x.VisecTransmisionMovimientos };
            var entidad = Repositorio.Obtener(includes, x => x.Id == id);
            entidad.Estado = (int)EstadoTransmisionAVisec.Error;
            entidad.FechaTransaccion = DateTime.Now;
            return entidad;
        }

        private object GenerarRequestBody(VisecTransmision entidad)
        {
            return new
            {
                entidad.CUITEmpresa,
                entidad.FechaHoraMovimiento,
                entidad.FechaCPE,
                entidad.NumeroCPE,
                entidad.NumeroCTG,
                entidad.CUITTitular,
                entidad.NumeroRUCAOrigen,
                entidad.CUITDestinatario,
                entidad.CUITDestino,
                entidad.NumeroRUCADestino,
                entidad.Producto,
                entidad.Campania,
                entidad.PesoNetoCargaKg,
                MovimientoMercaderiaUP = entidad.VisecTransmisionMovimientos.Select(x => new
                {
                    x.NumeroRENSPA,
                    x.NumeroCTGAsignado,
                    x.PesoNetoCargaKgPorUP,
                    x.PesoNetoDescargaKgPorUP,
                    x.PesoIngresoStockKg,
                    x.UltimoAlmacenamiento,
                    x.TipoMovimiento
                }).ToList()
            };
        }

        private IRestResponse EnviarRequest(object requestBody)
        {
            var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
            var request = new RestRequest(RecursoWebAPI.VISEC.ImportacionCartaPorte, Method.POST);
            request.AddJsonBody(requestBody);
            return client.Execute(request);
        }

        private void ProcesarRespuestaExitosa(VisecTransmision entidad, IRestResponse responseWebAPI, Resultado resultado)
        {
            var response = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoImportacionCartaPorteVisec>>(responseWebAPI.Content);

            if (response.IsValid)
            {
                ActualizarEntidadConRespuesta(entidad, response.Data);
            }
            else
            {
                entidad.DetalleTransaccion = response.Messages.FirstOrDefault()?.Message;
                resultado.Errores.Add("Error", response.Messages.FirstOrDefault()?.Message);
            }
        }

        private void ActualizarEntidadConRespuesta(VisecTransmision entidad, ResultadoImportacionCartaPorteVisec data)
        {
            var numero = data.Numero.ToString();
            entidad.NumeroProceso = numero;
            entidad.DetalleTransaccion = JsonConvert.SerializeObject(data);
            entidad.Estado = (int)EstadoTransmisionAVisec.EnviadoAVisec;

            if (string.IsNullOrEmpty(entidad.HistorialProcesos))
            {
                entidad.HistorialProcesos = numero;
            }
            else if (!entidad.HistorialProcesos.Split(',').Contains(numero))
            {
                entidad.HistorialProcesos += $",{numero}";
            }
        }
    }
}
