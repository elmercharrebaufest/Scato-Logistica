using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto.WebAPI;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorConfirmarCTGVencidas : ProcesadorComando<ConfirmarCTGVencidas>
    {
        private readonly IServicioComandos servicioComandos;

        public ProcesadorConfirmarCTGVencidas(IRepositorio repositorio,
                                            IConversor conversor,
                                            ILogger log,
                                            IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(ConfirmarCTGVencidas comando)
        {
            var resultado = new Resultado();
            Log.Debug("ProcesadorConfirmarCTGVencidas Inicio");
            try
            {
                var client = WebAPIRestClientFactory.GenerarClienteWebAPI();
                var ctgsVencidos = ObtenerCTGVencidos(comando, client);
                Log.Debug($"CTGDG Vencidos: {ctgsVencidos.ToJson()}");
                foreach (var ctg in ctgsVencidos)
                {
                    var resultadoCpe = ObtenerDatosCPEDG(comando.CentroId, ctg);
                    if (!resultadoCpe.HayErrores)
                        ConfirmarCPEDescargadoEnDestino(client, resultadoCpe);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error al Confirmar CTG Vencido ${ex.Message}");
            }
            Log.Debug("ProcesadorConfirmarCTGVencidas Fin");
            return resultado;
        }

        private List<long> ObtenerCTGVencidos(ConfirmarCTGVencidas comando, RestClient client)
        {
            var ctgs = new List<long>();
            try
            {
                var resultado = new ResponseWebAPIDto<List<ResultadoCPEPendienteResolucion>>();
                var centro = Repositorio.Obtener<Centro>(comando.CentroId);
                var data = new { Planta = centro.PlantaDG, Perfil = comando.TipoPerfil };
                var request = new RestRequest(RecursoWebAPI.AFIP.ConsultarCPEPendienteResolucion, Method.POST);
                request.AddJsonBody(data);
                var responseWebAPI = client.Execute(request);

                if (responseWebAPI.IsSuccessful)
                    resultado = JsonConvert.DeserializeObject<ResponseWebAPIDto<List<ResultadoCPEPendienteResolucion>>>(responseWebAPI.Content);

                if (resultado.IsValid)
                    ctgs.AddRange(resultado.Data.Where(x => x.NroCTG.ToString().StartsWith("3")).Select(x => x.NroCTG));
            }
            catch (Exception ex)
            {
                Log.Error($"Error al Obtener CTG Vencidos CentroId {comando.CentroId} Perfil {comando.TipoPerfil} - {ex.Message}");
                throw;
            }
            return ctgs;
        }

        private void ConfirmarCPEDescargadoEnDestino(RestClient client, ResultadoConsultaCpeAutomotorDG cpe)
        {
            try
            {
                var resultado = new ResponseWebAPIDto<ResultadoConfirmacionDescargadoDestino>();
                var data = new { cpe.NroOrden, cpe.Sucursal, cpe.TipoCPE };
                var request = new RestRequest(RecursoWebAPI.AFIP.ConfirmarDescargadoDestinoCPE, Method.POST);
                request.AddJsonBody(data);
                var responseWebAPI = client.Execute(request);

                if (responseWebAPI.IsSuccessful)
                    resultado = JsonConvert.DeserializeObject<ResponseWebAPIDto<ResultadoConfirmacionDescargadoDestino>>(responseWebAPI.Content);

                if (resultado.IsValid)
                    Log.Debug($"Se confirmó la CPE Descargado Destino - {resultado.Data}");
            }
            catch (Exception ex)
            {
                Log.Error($"Error al Confirmar CPE ${cpe.ToJson()} como Descargado Destino ${ex.Message}");
                throw;
            }
        }

        private ResultadoConsultaCpeAutomotorDG ObtenerDatosCPEDG(int centroId, long nroCTG)
        {
            var result = servicioComandos.Ejecutar(new ConsultarCPEAutomotorDG
            {
                CentroId = centroId,
                NumeroCTG = nroCTG
            }) as ResultadoConsultaCpeAutomotorDG;
            return result;
        }
    }
}