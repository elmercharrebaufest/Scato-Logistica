using System;
using System.Linq;
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
    public class ProcesadorImprimirAsignacionDeRuta : ProcesadorImpresionAsync<ImprimirAsignacionDeRuta>
    {
        public ProcesadorImprimirAsignacionDeRuta(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
        }

        protected override void EjecutarAsync(ImprimirAsignacionDeRuta comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de AsignacionDeRuta en la impresora: " + comando.Dto.Impresora);

                servicioImpresor.Ejecutar(comando);
                
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirAsignacionDeRuta comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpAsignacionDeRutaDto, ImpAsignacionDeRuta>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.AsignacionDeRuta;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e,"Error al guardar ImpAsignacionDeRuta");
                throw;
            }
            
        }
    }
}
