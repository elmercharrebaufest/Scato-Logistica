using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{

    public class ProcesadorImprimirCartaPorteUrenport : ProcesadorComando<ImprimirCartaPorteUrenport>
    {


        public ProcesadorImprimirCartaPorteUrenport(ILogger log)
            : base(log)
        {

        }

        public override Resultado Ejecutar(ImprimirCartaPorteUrenport comando)
        {
            try
            {
                Log.Debug("Iniciando impresión de ImprimirCartaPorteUrenport en la impresora: " + comando.Dto.Impresora);


                var impresora = new CartaPorteUrenport(comando.Dto, comando.Dto.Impresora);
                impresora.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;               
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
