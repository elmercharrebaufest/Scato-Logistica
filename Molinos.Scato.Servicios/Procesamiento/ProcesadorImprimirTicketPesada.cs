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
    public class ProcesadorImprimirTicketPesada : ProcesadorImpresionAsync<ImprimirTicketPesada>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirTicketPesada(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirTicketPesada comando, IServicioImpresion servicioImpresor)
        {
            try
            {
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
    }
}
