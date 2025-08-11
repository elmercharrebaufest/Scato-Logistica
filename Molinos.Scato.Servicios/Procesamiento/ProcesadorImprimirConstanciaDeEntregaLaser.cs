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
    public class ProcesadorImprimirConstanciaDeEntregaLaser : ProcesadorComandoImpresion<ImprimirConstanciaDeEntregaLaser>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirConstanciaDeEntregaLaser(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirConstanciaDeEntregaLaser comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de ConstanciaDeEntregaLaser en la impresora: " + comando.Dto.Impresora);
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

        protected override int EjecutarSync(ImprimirConstanciaDeEntregaLaser comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpConstanciaDeEntregaLaserDto, ImpConstanciaDeEntregaLaser>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.ConstanciaDeEntregaLaser;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImprimirConstanciaDeEntregaLaser ");
                throw;
            }
        }

        protected override Func<MaterialPorWorkflow, bool> PropiedadConfiguracionDebeImprimir
        {
            get { return materialPorWorkflow => materialPorWorkflow.ImprimirTicketPesada; }
        }
    }
}
