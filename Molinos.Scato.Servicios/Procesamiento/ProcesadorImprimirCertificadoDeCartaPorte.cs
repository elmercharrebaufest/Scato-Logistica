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
    public class ProcesadorImprimirCertificadoDeCartaPorte : ProcesadorImpresionAsync<ImprimirCertificadoDeCartaPorte>
    {
        public ProcesadorImprimirCertificadoDeCartaPorte(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
        }

        protected override void EjecutarAsync(ImprimirCertificadoDeCartaPorte comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                Log.Debug("Iniciando impresión de CertificadoDeCartaPorte en la impresora: " + comando.Dto.Impresora);

                servicioImpresor.Ejecutar(comando);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
        }

        protected override int EjecutarSync(ImprimirCertificadoDeCartaPorte comando)
        {
            try
            {
                var entidad = Conversor.Convertir<ImpCertificadoDeCartaPorteDto, ImpCertificadoDeCartaPorte>(comando.Dto);
                entidad.FechaImpresion = DateTime.Now;
                entidad.TipoImpresion = TipoImpresion.CertificadoDeCartaPorte;
                entidad.Codigo = comando.Dto.Codigo;
                Repositorio.Agregar(entidad);
                Repositorio.GuardarCambios();
                return entidad.Id;
            }
            catch (Exception e)
            {
                Log.Error(e,"Error al guardar ImpCertificadoDeCartaPorte");
                throw;
            }
            
        }
    }
}
