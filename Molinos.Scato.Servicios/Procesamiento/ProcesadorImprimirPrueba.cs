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
    public class ProcesadorImprimirPrueba : ProcesadorImpresionAsync<ImprimirPrueba>
   {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirPrueba(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirPrueba comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                servicioImpresor.Ejecutar(comando);

            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.NombreImpresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirPrueba comando)
        {
            return 0;
        }
    }
}
