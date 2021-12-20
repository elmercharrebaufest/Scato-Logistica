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
    public class ProcesadorImprimirInformeDeRecepcion : ProcesadorImpresionAsync<ImprimirInformeDeRecepcion>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirInformeDeRecepcion(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirInformeDeRecepcion comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de InformeDeRecepcion en la impresora: " + comando.Impresora);
                var firma = firmaProvider.ObtenerFirmaSinLogo();
                comando.Firma = firma;
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirInformeDeRecepcion comando)
        {
            foreach (var item in comando.Dto)
            {
                try
                {
                    var entidad = Conversor.Convertir<ImpInformeDeRecepcionDto, ImpInformeDeRecepcion>(item);
                    entidad.FechaImpresion = DateTime.Now;
                    entidad.TipoImpresion = TipoImpresion.InformeDeRecepcion;
                    entidad.Codigo = item.Codigo;
                    Repositorio.Agregar(entidad);
                    Repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    Log.Error(e,"Error al guardar ImpInformeDeRecepcion");
                }
                
            }
            return 0;
        }
    }
}
