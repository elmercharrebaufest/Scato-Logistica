using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Threading;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorInformarViajeOrdenesResiduos : ProcesadorComando<InformarViajeOrdenesResiduos>
    {
        private readonly ILogger log;
        private readonly IServicioOperaciones servicioOperaciones;


        public ProcesadorInformarViajeOrdenesResiduos(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOperaciones servicioOperaciones) 
            : base(repositorio, conversor, log)
        {
            this.log = log;
            this.servicioOperaciones = servicioOperaciones;
        }

        public override Resultado Ejecutar(InformarViajeOrdenesResiduos comando)
        {
            log.Debug("Iniciando Informe de Finalizacion al servicio de Operaciones para la orden No: {0}", comando.Dto.IdOperaciones);

            if(comando.Dto.IdOperaciones <= 0)
            {
                throw new ArgumentException("No se encontró orden de operaciones ");
            }

            int maxIntentos = 2; 
            int intentoActual = 0; 
            TimeSpan retraso = TimeSpan.FromSeconds(5);

            while (intentoActual < maxIntentos)
            {
                try
                {
                    servicioOperaciones.InformarViajeOrdenesResiduos(comando.Dto);
                    log.Debug("Orden No: {0} Completada satisfactoriamente.", comando.Dto.IdOperaciones);
                    return new Resultado();
                }
                catch (Exception)
                {
                    intentoActual++; 
                    log.Debug("Error: al enviar la solicitud al servicio en la orden Orden No: {0}. Intento {1} de {2}.", comando.Dto.IdOperaciones, intentoActual, maxIntentos);

                    if (intentoActual >= maxIntentos)
                    {
                        log.Debug("Se han superado los máximos intentos permitidos. No se pudo completar la operación para la orden No: {0}", comando.Dto.IdOperaciones);
                        throw; 
                    }

                    Thread.Sleep(retraso);
                    retraso = TimeSpan.FromTicks(retraso.Ticks * 2); 
                }
            }

            throw new InvalidOperationException("Un error inesperado ha ocurrido en el procesador InformarViajeOrdenFason.");
        }
    }
}
