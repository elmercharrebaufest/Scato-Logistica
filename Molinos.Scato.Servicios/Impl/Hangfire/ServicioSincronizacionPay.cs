using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.Behavior;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static Molinos.Scato.Dominio.Constantes;

namespace Molinos.Scato.Servicios.Impl
{
    [AiErrorHandlerBehaviorAttribute]
    public class ServicioSincronizacionPay : IServicioSincronizacionPay
    {
        private readonly IServicioRepositorio repositorio;
        private readonly ILogger log;
        private readonly IServicioComandos comandos;
        private readonly IServicioOperaciones operaciones;

        public ServicioSincronizacionPay(IServicioRepositorio repositorio, ILogger log, IServicioComandos comandos, IServicioOperaciones operaciones)
        {
            this.repositorio = repositorio;
            this.log = log;
            this.comandos = comandos;
            this.operaciones = operaciones;
        }

        public void SincronizarMOAPayEstadoDePagos()
        {
            var fechaDesde = repositorio.ObtenerUltimaFechaDePagoTasaMunicipal();
            comandos.Ejecutar(new MOAPaySincronizarEstadoDePagos
            {
                Disponible = MOAPay.Filtros.SI,
                Pagado = MOAPay.Filtros.SI,
                TipoFecha = MOAPay.Filtros.FECHAPAGO,
                FechaDesde = fechaDesde ?? DateTime.Now.AddDays(-30),
                FechaHasta = DateTime.Now,
            });
        }

        public void SincronizarMOAPayCPE()
        {
            var cpeList = repositorio.ListarCPEFiltradasPorFechaDeCacheado(DateTime.Now.AddHours(-1), DateTime.Now);

            foreach (var cpe in cpeList)
            {
                var tipoDeVehiculo = repositorio.ObtenerTipodVehiculoPorPesoBruto(cpe.PesoBruto, 5);
                comandos.Ejecutar(new MOAPayCrearModificarCPE
                {
                    NumeroDocumento = cpe.NroCtg.ToString(),
                    Dominio = cpe.Dominio,
                    TipoDeVehiculo = ObtenerMOAPayTipoVehiculo(tipoDeVehiculo),
                    CuitInterviniente = cpe.CuitTransportista.ToString()
                });
            }
        }

        public void SincronizarMOAPayOperacionesFason()
        {
            var ordenes = operaciones.ObtenerOrdenesDeCarga(null);

            SincronizarMOAPayOperaciones(
                ordenes.ToList(),
                Job.SincronizarMOAPayOperacionesFason,
                MOAPay.TipoDeRemito.Fason
            );
        }

        public void SincronizarMOAPayOperacionesFas()
        {
            var ordenes = operaciones.ObtenerOrdenesDeCargaFas(null);

            SincronizarMOAPayOperaciones(
                ordenes.ToList(),
                Job.SincronizarMOAPayOperacionesFas,
                MOAPay.TipoDeRemito.Fas
            );
        }

        public void SincronizarMOAPayOperacionesResiduos()
        {
            var residuos = operaciones.ObtenerOrdenesResiduos(null);
            var ordenes = residuos.Select(o => new OrdenDeCargaDto
            {
                Id = o.Id,
                PatenteChasis = o.PatenteChasis,
                PatenteAcoplado = o.PatenteAcoplado,
                CUITTransporte = o.CUITTransporte,
                FechaCreacion = o.FechaCreacion
            }).ToList();

            SincronizarMOAPayOperaciones(
                ordenes,
                Job.SincronizarMOAPayOperacionesResiduos,
                MOAPay.TipoDeRemito.Residuos
            );
        }

        private void ProcesarMOAPayOperacion(int id, string patenteChasis, string patenteAcoplado, string cuitTransporte, string tipoRemito)
        {
            var tipoDeVehiculo = new TipoVehiculo();

            var resultadoEscalables = comandos.Ejecutar(new ConsultarEscalables { Patente = patenteChasis, Acoplado = patenteAcoplado }) as ResultadoEscalables;
            if (!resultadoEscalables.HayErrores)
            {
                tipoDeVehiculo = resultadoEscalables.Categoria ?? new TipoVehiculo();
            }

            comandos.Ejecutar(new MOAPayCrearModificarCPE
            {
                NumeroDocumento = id.ToString().PadLeft(9, '0') + tipoRemito,
                Dominio = patenteChasis,
                TipoDeVehiculo = ObtenerMOAPayTipoVehiculo(tipoDeVehiculo),
                CuitInterviniente = cuitTransporte
            });
        }

        private string ObtenerMOAPayTipoVehiculo(TipoVehiculo tipoVehiculo)
        {
            switch (tipoVehiculo)
            {
                case TipoVehiculo.Bitren:
                case TipoVehiculo.CamiónC:
                case TipoVehiculo.CamiónD:
                case TipoVehiculo.CamiónE:
                    return MOAPay.TipoDeVehiculo.ESCALABLE;

                default:
                    return MOAPay.TipoDeVehiculo.COMUN;
            }
        }

        private void ActualizarRegistroJobEjecucion(string nombreProceso, DateTime? fechaCreacionMasReciente)
        {
            if (fechaCreacionMasReciente != null)
            {
                comandos.Ejecutar(new ActualizarRegistroJobEjecucion
                {
                    Dto = new RegistroJobEjecucionDto
                    {
                        NombreProceso = nombreProceso,
                        Descripcion = $"Fecha del registro más reciente obtenido de la operación a través del servicio {nombreProceso}",
                        FechaEjecucion = fechaCreacionMasReciente.Value
                    }
                });
            }
        }

        private bool ParseFechaCreacion(string fechaCreacionStr, out DateTime fechaCreacion)
        {
            return DateTime.TryParseExact(
                fechaCreacionStr,
                "d/M/yyyy H:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out fechaCreacion
            );
        }

        private void SincronizarMOAPayOperaciones(List<OrdenDeCargaDto> ordenes, string nombreProceso, string tipoRemito)
        {
            var registroJob = repositorio.ObtenerRegistroJobEjecucionPorProceso(nombreProceso);
            DateTime? fechaCreacionOrdenMasReciente = null;

            foreach (var orden in ordenes)
            {
                if (!ParseFechaCreacion(orden.FechaCreacion, out DateTime fechaCreacionOrden))
                    continue;

                if (registroJob != null && fechaCreacionOrden <= registroJob.FechaEjecucion)
                    continue;

                if (fechaCreacionOrdenMasReciente == null || fechaCreacionOrden > fechaCreacionOrdenMasReciente)
                    fechaCreacionOrdenMasReciente = fechaCreacionOrden;

                ProcesarMOAPayOperacion(
                    orden.Id,
                    orden.PatenteChasis,
                    orden.PatenteAcoplado,
                    orden.CUITTransporte,
                    tipoRemito
                );
            }

            ActualizarRegistroJobEjecucion(
                nombreProceso,
                fechaCreacionOrdenMasReciente
            );
        }
    }
}