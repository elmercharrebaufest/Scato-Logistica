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
    public class ProcesadorImprimirMicromuestra : ProcesadorImpresionAsync<ImprimirMicromuestra>
    {
        public ProcesadorImprimirMicromuestra(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
        }

        protected override void EjecutarAsync(ImprimirMicromuestra comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirMicromuestra en la impresora: " + comando.Dto.Impresora);
                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirMicromuestra comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpIdentificacionMicromuestraDto, ImpIdentificacionMicromuestra>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.IdentificacionMicromuestra;
                entidad.Codigo = comando.Dto.Codigo;
                if (!String.IsNullOrEmpty(comando.Dto.NroCasillero))
                {
                    entidad.NroCasillero = comando.Dto.NroCasillero;
                }
                entidad.TipoMicromuestra = comando.TipoMicromuestra;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpIdentificacionMicromuestra ");
                throw;
            }
        }
    }
}
