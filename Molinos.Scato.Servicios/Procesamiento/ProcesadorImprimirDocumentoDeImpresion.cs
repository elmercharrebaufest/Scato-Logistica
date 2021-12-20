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
    public class ProcesadorImprimirDocumentoDeImpresion : ProcesadorImpresionAsync<ImprimirDocumentoDeImpresion>
    {
        public ProcesadorImprimirDocumentoDeImpresion(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
        }

        protected override void EjecutarAsync(ImprimirDocumentoDeImpresion comando, IServicioImpresion servicioImpresor)
        {
            if (comando.FormatoDeImpresion != null)
            {
                try
                {
                    Log.Debug("Iniciando impresión de DocumentoDeImpresion en la impresora: " +
                              comando.Dto.Impresora);

                    servicioImpresor.Ejecutar(comando);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                    throw;
                }
            }
            else
            {
                Log.Error("Error al imprimir en la impresora: codigo de impresion no encontrado");
            }
        }

        protected override int EjecutarSync(ImprimirDocumentoDeImpresion comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpImpresionGenericaDto, ImpImpresionGenerica>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.ImpresionGenerica;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al guardar ImpImpresionGenerica ");
                throw;
            }
        }
    }
}
