using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Impl;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImprimirTicketPesada : ProcesadorComandoImpresion<ImprimirTicketPesada>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirTicketPesada(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresorFactory)
            : base(repositorio, conversor, log, servicioImpresorFactory)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirTicketPesada comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de TicketPesada en la impresora: " + comando.Dto.Impresora);

                var firma = firmaProvider.ObtenerFirmaSinLogo();
                comando.Firma = firma;
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirTicketPesada comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpTicketPesadaDto, ImpTicketPesada>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.TicketPesada;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpTicketPesada ");
                throw;
            }
        }

        protected override Func<MaterialPorWorkflow, bool> PropiedadConfiguracionDebeImprimir
        {
            get { return materialPorWorkflow => materialPorWorkflow.ImprimirTicketPesada; }
        }
    }
}
