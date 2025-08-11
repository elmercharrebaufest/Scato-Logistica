using System;
using System.Linq;
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
    public class ProcesadorImprimirAsigRecorrCtrolCalid : ProcesadorComandoImpresion<ImprimirAsigRecorrCtrolCalid>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirAsigRecorrCtrolCalid(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresorFactory)
            : base(repositorio, conversor, log, servicioImpresorFactory)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirAsigRecorrCtrolCalid comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de AsigRecorrCtrolCalid en la impresora: " + comando.Dto.Impresora);
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

        protected override int EjecutarSync(ImprimirAsigRecorrCtrolCalid comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpAsigRecorrCtrolCalidDto, ImpAsigRecorrCtrolCalid>(comando.Dto);
                var caracteristicasAnalisis = entidad.AnalisisPorCaracteristicas.Select(x => x.Id);
                var caracteristicasCalado = entidad.CaladoPorCaracteristicas.Select(x => x.Id);

                if (entidad.AnalisisPorCaracteristicas.Count > 0)
                {
                    entidad.AnalisisPorCaracteristicas =
                        Repositorio.Listar<AnalisisPorCaracteristica>(
                            x => caracteristicasAnalisis.Count(y => x.Id == y) > 0);
                }
                if (entidad.CaladoPorCaracteristicas.Count > 0)
                {
                    entidad.CaladoPorCaracteristicas =
                        Repositorio.Listar<CaladoPorCaracteristica>(x => caracteristicasCalado.Count(y => x.Id == y) > 0);
                }
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.AsigRecorrCtrolCalid;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e,"Error al guardar ImpAsigRecorrCtrolCalid");
                throw;
            }
            
        }

        protected override Func<MaterialPorWorkflow, bool> PropiedadConfiguracionDebeImprimir
        {
            get { return materialPorWorkflow => materialPorWorkflow.ImprimirAsignacionRuta; }
        }
    }
}
