using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioSincronizacionBandaHorariaStopRechazados : IServicioSincronizacionBandaHorariaStopRechazados
    {
        private readonly IServicioRepositorio repositorio;
        private readonly ILogger Log;        
        private readonly IServicioComandos comandos;
        private const string SemaforoBlanco = "Blanco";
        private const string AsuntoMailFallidos = "Banda Horaria Stop - Fallidos";

        public ServicioSincronizacionBandaHorariaStopRechazados(IServicioRepositorio repositorio, ILogger log, IServicioComandos comandos)
        {
            this.repositorio = repositorio;
            this.Log = log;
            this.comandos = comandos;
        }

        public void SincronizarBandaHorariaStop()
        {
            /*agregar sleep y control de excepciones*/
            Log.Info("STOP - Paso 3: SincronizarBandaHorariaStop()");            
            var lista = repositorio.ListarBandaHorariaStopRechazados(SemaforoBlanco);
            foreach (var dto in lista)
            {
                Log.Info("Camiones Semaforo en Blanco: " + dto.Patente);
                try
                {
                    //Log.Info("STOP - Paso 5: ValidarAccesoStopBandasHorarias");
                    comandos.Ejecutar(new ValidarAccesoStopBandasHorarias
                    {
                        Id = dto.Id,
                        CTG = dto.CTG,
                        Patente = dto.Patente,
                        Fecha = dto.FechaIngreso,
                        Reintentos = dto.Reintentos
                    });

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error al procesar ValidarAccesoBandaHoraria");
                }
            }

            EnviarMailSiSuperaUmbralDeFallidos();
        }

        private void EnviarMailSiSuperaUmbralDeFallidos()
        {
            var cantidadFallidos = repositorio.ListarBandaHorariaStopRechazados(SemaforoBlanco).Count;
            var umbralConfigurado = repositorio.ObtenerConfiguracionGeneral(
                ConfiguracionGeneral.Pantalla.BandaHorariaStop,
                ConfiguracionGeneral.BandaHorariaStop.Umbral);

            if (umbralConfigurado == null || !int.TryParse(umbralConfigurado.Valor, out int umbral))
            {
                Log.Error($"STOP - Umbral de fallidos inválido para BandaHorariaStop.Umbral. Valor: '{umbralConfigurado?.Valor ?? "null"}'.");
                return;
            }

            if (cantidadFallidos < umbral)
            {
                Log.Info($"STOP - Fallidos semáforo blanco: {cantidadFallidos}. Umbral configurado: {umbral}. No se envía mail.");
                return;
            }

            var configuracionDestinatarios = repositorio.ObtenerConfiguracionGeneral(
                ConfiguracionGeneral.Pantalla.BandaHorariaStop,
                ConfiguracionGeneral.BandaHorariaStop.ListaDeDistribucion);

            var destinatarios = ParsearDestinatarios(configuracionDestinatarios?.Valor);
            if (!destinatarios.Any())
            {
                Log.Error("STOP - No hay destinatarios configurados para BandaHorariaStop.ListaDeDistribucion. No se envía mail.");
                return;
            }

            var resultado = comandos.Ejecutar(new EnvioMail
            {
                Destinatarios = destinatarios,
                Titulo = AsuntoMailFallidos,
                Cuerpo = $"Hay {cantidadFallidos} registros fallidos.<br/>Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}<br/>Verificar Sincronizacion Banda Horaria STOP."
            });

            if (resultado.HayErrores)
            {
                Log.Error($"STOP - Error al enviar mail de fallidos Banda Horaria STOP: {string.Join(" | ", resultado.Errores.Select(e => e.Value))}");
                return;
            }

            Log.Info($"STOP - Mail de fallidos enviado. Cantidad de registros semáforo blanco: {cantidadFallidos}. Umbral: {umbral}.");
        }

        private static List<string> ParsearDestinatarios(string destinatarios)
        {
            if (string.IsNullOrWhiteSpace(destinatarios))
            {
                return new List<string>();
            }

            return destinatarios
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();
        }
    }
}