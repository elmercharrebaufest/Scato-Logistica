using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios.Behavior;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using NPOI.SS.Formula.Functions;
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
            var resultado = new Resultado();
            var lista = repositorio.ListarBandaHorariaStopRechazados("Blanco");
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
                    resultado.Error("Error", "Error en SincronizarBandaHorariaStop");                    
                }
            }
        }
    }
}