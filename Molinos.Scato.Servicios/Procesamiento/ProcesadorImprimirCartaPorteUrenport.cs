using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Servicios.ServicioImpresion;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
   
    public class ProcesadorImprimirCartaPorteUrenport : ProcesadorImpresionAsync<ImprimirCartaPorteUrenport>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorImprimirCartaPorteUrenport(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
            this.firmaProvider = firmaProvider;
        }

        protected override void EjecutarAsync(ImprimirCartaPorteUrenport comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirCartaPorteUrenport en la impresora: " + comando.Dto.Impresora);
                var firma = firmaProvider.ObtenerFirmaSinLogo();
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirCartaPorteUrenport comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpCartaPorteUrenportDto, ImpCartaPorteUrenport>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.CartaPorteUrenport;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpCartaPorteUrenportDto ");
                throw;
            }
        }
    }
}
