using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarIngresosPorCompraDeGranosTransmisionASap : ProcesadorComando<ActualizarIngresosPorCompraDeGranosTransmisionASap>
    {
        public ProcesadorActualizarIngresosPorCompraDeGranosTransmisionASap(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarIngresosPorCompraDeGranosTransmisionASap comando)
        {
            var resultado = new ResultadoCrear();
            //Esta transacción es necesaria para que la actualización se ejecute incluso cuando falla la transacción padre.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Iniciando IngresosPorCompraDeGranosTransmisionASap");
                var transmision = Repositorio.Obtener<IngresosPorCompraDeGranosTransmisionASap>(
                    x => (x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow && x.FuncionSap == comando.Dto.FuncionSap) || (x.Id == comando.Dto.Id));

                if (transmision == null)
                {
                    Log.Info("Creando IngresosPorCompraDeGranosTransmisionASap");
                    Repositorio.Agregar(comando.Dto);
                    transmision = comando.Dto;
                }
                else
                {
                    Log.Info("Modificando IngresosPorCompraDeGranosTransmisionASap");
                    transmision.Estado = comando.Dto.Estado;
                    if (comando.Dto.MensajeError != null)
                    {
                        transmision.MensajeError = comando.Dto.MensajeError;
                    }
                }
                Log.Info("Se van a guardar los cambios en IngresosPorCompraDeGranosTransmisionASap");
                Repositorio.GuardarCambios();
                Log.Info("Cambios guardados");
                resultado.Id = transmision.Id;
                transaction.Complete();
            }
            return resultado;
        }
    }
}