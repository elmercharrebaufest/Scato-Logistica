using System;
using System.Configuration;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImprimirResumenHojaDeRuta : ProcesadorImpresionAsync<ImprimirResumenHojaDeRuta>
    {
        public ProcesadorImprimirResumenHojaDeRuta(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
        }

        public IServicioImpresion ServicioImpresion { get; }

        protected override void EjecutarAsync(ImprimirResumenHojaDeRuta comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirResumenHojaDeRuta en la impresora: " + comando.Dto.Impresora);
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirResumenHojaDeRuta comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpResumenHojaDeRutaDto, ImpResumenHojaDeRuta>(comando.Dto);
                
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.ResumenHojaDeRuta;
                entidad.Codigo = comando.Dto.Codigo;
                entidad.Patente = string.Empty;

                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpEtiquetaRubrosAnalizarDto ");
                throw;
            }
        }
    }
}
