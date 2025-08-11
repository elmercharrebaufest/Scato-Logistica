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
    public class ProcesadorImprimirMuestraInase : ProcesadorComandoImpresion<ImprimirMuestraInase>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirMuestraInase(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresorFactory)
            : base(repositorio, conversor, log, servicioImpresorFactory)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirMuestraInase comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirMuestraAuditoria en la impresora: " + comando.Dto.Impresora);
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

        protected override int EjecutarSync(ImprimirMuestraInase comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpEtiquetaMuestraInaseDto, ImpEtiquetaMuestraInase>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.ProductorCuit = comando.Dto.CuitProductor;
                entidad.Cpe = comando.Dto.NumeroCartaPorte;
                entidad.TipoImpresion = TipoImpresion.EtiquetaMuestraInase;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpIdentificacionMuestraAuditoria ");
                throw;
            }
        }

        protected override Func<MaterialPorWorkflow, bool> PropiedadConfiguracionDebeImprimir 
        {
            get { return materialPorWorkflow => materialPorWorkflow.ImprimirMuestraInase; }
        }
    }
}
