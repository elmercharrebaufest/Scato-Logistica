using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{

    public class ProcesadorImprimirReciboMunicipal : ProcesadorImpresionAsync<ImprimirReciboMunicipal>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirReciboMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirReciboMunicipal comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirReciboMunicipal en la impresora: " + comando.Dto.Impresora);
                var firma = firmaProvider.ObtenerFirmaSinLogo();
                comando.Firma = firma;
                Log.Debug("D-Ejecutando Impresion");
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirReciboMunicipal comando)
        {
            try
            {
                Log.Debug("D-Inicio para guardar ImpReciboMunicipal ");
                var entidad = Conversor.Convertir<ImpReciboMunicipalDto, ImpReciboMunicipal>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.ReciboMunicipal;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                Log.Debug("D-Finaliza Guardado");
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpReciboMunicipal ");
                throw;
            }
        }

        protected override bool DoDebeImprimir(ImprimirReciboMunicipal comando)
        {
            return !Repositorio.Existe<PagosTasaMunicipal>(p => p.IdInstance == comando.Dto.WorkflowId);
        }

        protected override bool EsFlujoAlterno(ImprimirReciboMunicipal comando)
        {
            return comando.IdPagoDigital > 0;
        }

        protected override int EjecutarFlujoAlternoSync(ImprimirReciboMunicipal comando)
        {
            try
            {
                Log.Debug("D-Inicio para guardar ImpReciboMunicipal ");
                GuardarRegistroImpresionPagoDigital(comando.IdPagoDigital, comando.Dto.TicketNro);
                var entidad = Conversor.Convertir<ImpReciboMunicipalDto, ImpReciboMunicipal>(comando.Dto);

                Impresion impresion = Conversor.Convertir<ImpReciboMunicipal, Impresion>(entidad);
                impresion.FechaImpresion = DateTime.Now;
                impresion.TipoImpresion = TipoImpresion.ReciboMunicipal;
                impresion.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(impresion);
                Repositorio.GuardarCambios();
                Log.Debug("D-Finaliza Guardado");
                return impresion.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpReciboMunicipal ");
                throw;
            }
        }

        private void GuardarRegistroImpresionPagoDigital(int idPago, string ticket)
        {
            var pago = Repositorio.Obtener<PagosTasaMunicipal>(idPago);
            if (pago == null)
            {
                Log.Error($"No se encontró el pago con ID {idPago} para registrar la impresión del recibo municipal.");
                new ArgumentException($"No se encontró el pago con ID {idPago} para registrar la impresión del recibo municipal.");
            }
            pago.NroRecibo = ticket;
        }
    }
}
