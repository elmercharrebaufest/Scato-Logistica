using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirCertificadoDeAnalisis : ProcesadorComando<ImprimirCertificadoDeAnalisis>
    {


        public ProcesadorImprimirCertificadoDeAnalisis(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirCertificadoDeAnalisis comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de CertificadoDeAnalisis en la impresora: " + comando.Dto.Impresora);


                var impresora = new CertificadoDeAnalisis(comando.Dto, comando.Dto.Impresora, comando.Firma);

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
