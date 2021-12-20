using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarInformarCupoTransmisionASap : ProcesadorComando<ActualizarInformarCupoTransmisionASap>
    {
        public ProcesadorActualizarInformarCupoTransmisionASap(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarInformarCupoTransmisionASap comando)
        {
            var resultado = new ResultadoCrear();
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Iniciando InformarCupoTransmisionASap");
                var transmision =
                    Repositorio.Obtener<InformarCupoTransmisionASap>(
                        x => x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow || x.Id == comando.Dto.Id);
                if (transmision == null)
                {
                    Log.Info("Creando InformarCupoTransmisionASap");
                    Repositorio.Agregar(comando.Dto);
                    transmision = comando.Dto;
                }
                else
                {
                    Log.Info("Modificando InformarCupoTransmisionASap");
                    transmision.Estado = comando.Dto.Estado;
                    if (comando.Dto.MensajeError != null)
                    {
                        transmision.MensajeError = comando.Dto.MensajeError;
                    }
                }
                Log.Info("Se van a guardar los cambios en InformarCupoTransmisionASap");
                Repositorio.GuardarCambios();
                resultado.Id = transmision.Id;
                Log.Info("Cambios guardados");
                transaction.Complete();
            }

            return resultado;
        }
    }
}
