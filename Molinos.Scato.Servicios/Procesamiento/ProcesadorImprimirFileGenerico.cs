using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.ServicioImpresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorImprimirFileGenerico : ProcesadorImpresionAsync<ImprimirFileGenerico>
    {
        public ProcesadorImprimirFileGenerico(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioImpresorFactory servicioImpresion)
            : base(repositorio, conversor, log, servicioImpresion)
        {
        }

        protected override void EjecutarAsync(ImprimirFileGenerico comando, IServicioImpresion servicioImpresor)
        {
            try
            {
                if (!string.IsNullOrEmpty(comando.Impresora))
                {
                    try
                    {
                        Log.Debug($"Iniciando impresión de {comando?.CodigoDocumentoImpresion} en la impresora: " +
                                  comando.Impresora);

                        servicioImpresor.Ejecutar(comando);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error al imprimir en la impresora: " + comando.Impresora);
                    }
                }
                else
                {
                    Log.Error("Error al imprimir en la impresora: codigo de impresora no encontrado");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en modulo ProcesadorImprimirFileGenerico");
            }
        }

        protected override int EjecutarSync(ImprimirFileGenerico comando)
        {
            return 1;
        }
    }
}
