using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirEnvioLoteACamara : ProcesadorComando<ImprimirEnvioLoteACamara>
    {
        public ProcesadorImprimirEnvioLoteACamara(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirEnvioLoteACamara comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de IdentificacionEnvioLoteACamara en la impresora: " + comando.Dto.Impresora);

                var impresora = new IdentificacionEnvioLoteACamara(comando.Dto, comando.Dto.Impresora);

                for (var copia = 0; copia < comando.CantidadCopias; copia++)
                {
                    impresora.Print();
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                throw;
            }
            return new Resultado();

        }
    }
}
