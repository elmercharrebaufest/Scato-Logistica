using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.ModuloImpresor.Impresion;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.ModuloImpresor.Procesamiento
{
    public class ProcesadorImprimirTarjetaDeAcceso : ProcesadorComando<ImprimirTarjetaDeAcceso>
    {
        public ProcesadorImprimirTarjetaDeAcceso(ILogger log)
            : base(log)
        {
        }

        public override Resultado Ejecutar(ImprimirTarjetaDeAcceso comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Debug("Iniciando impresión de ResumenDeRecepcion en la impresora: " + comando.Dto.Impresora + " OrigenImpresion: " + comando?.OrigenImpresion + " NumeroTarjeta: " + comando?.Dto?.Numero + " PuestroTrabajoId: " + comando?.Dto?.PuestoDeTrabajoId);

                var impresora = new TarjetaDeAcceso(comando.Dto, comando.Dto.Impresora);
                impresora.Print();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error al imprimir en la impresora: " + comando.Dto.Impresora);
                resultado.Errores.Add("", String.Format(Textos.ImpresoraNoConecta, comando.Dto.Impresora));
            }
            return resultado;
        }
    }
}
