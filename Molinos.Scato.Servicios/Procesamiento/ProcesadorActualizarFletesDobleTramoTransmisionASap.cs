using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarFletesDobleTramoTransmisionASap : ProcesadorComando<ActualizarFletesDobleTramoTransmisionASap>
    {
        public ProcesadorActualizarFletesDobleTramoTransmisionASap(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarFletesDobleTramoTransmisionASap comando)
        {
            var resultado = new ResultadoCrear();
            //Esta transacción es necesaria para que la actualización se ejecute incluso cuando falla la transacción padre.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Iniciando FletesDobleTramoTransmisionASap");
                var transmision = Repositorio.Obtener<FletesDobleTramoTransmisionASap>(
                    x => (x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow && x.FuncionSap == comando.Dto.FuncionSap) || (x.Id == comando.Dto.Id));

                if (transmision == null)
                {
                    Log.Info("Creando FletesDobleTramoTransmisionASap");
                    Repositorio.Agregar(comando.Dto);
                    transmision = comando.Dto;
                }
                else
                {
                    Log.Info("Modificando FletesDobleTramoTransmisionASap");
                    transmision.Estado = comando.Dto.Estado;
                    if (comando.Dto.MensajeError != null)
                    {
                        transmision.MensajeError = comando.Dto.MensajeError;
                    }
                }
                Log.Info("Se van a guardar los cambios en FletesDobleTramoTransmisionASap");
                Repositorio.GuardarCambios();
                resultado.Id = transmision.Id;
                Log.Info("Cambios guardados");
                transaction.Complete();
            }

            return resultado;
        }
    }
}
