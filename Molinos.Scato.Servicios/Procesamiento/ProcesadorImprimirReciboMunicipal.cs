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
    }
}
