using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarIngresosBodegaTransmisionASap : ProcesadorComando<ActualizarIngresosBodegaTransmisionASap>
    {
        public ProcesadorActualizarIngresosBodegaTransmisionASap(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarIngresosBodegaTransmisionASap comando)
        {
            var resultado = new ResultadoCrear();

            //Esta transacción es necesaria para que la actualización se ejecute incluso cuando falla la transacción padre.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Iniciando IngresosBodegaTransmisionASap");

                var transmision = Repositorio.Obtener<IngresosBodegaTransmisionASap>(x => x.Id == comando.Dto.Id);
                if (transmision == null)
                {
                    transmision = Repositorio.Obtener<IngresosBodegaTransmisionASap>(x => (x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow && x.FuncionSap == comando.Dto.FuncionSap && ((!string.IsNullOrEmpty(comando.Dto.Cuartel) & x.Cuartel == comando.Dto.Cuartel) || (!string.IsNullOrEmpty(comando.Dto.Tanque) & x.Tanque == comando.Dto.Tanque) || (comando.TipoBinId.HasValue & x.TipoBinId == comando.TipoBinId))));
                }
                if (transmision == null)
                {
                    Log.Info("Creando IngresosBodegaTransmisionASap");
                    Repositorio.Agregar(comando.Dto);
                    transmision = comando.Dto;
                }
                else
                {
                    Log.Info("Modificando IngresosBodegaTransmisionASap");
                    transmision.Estado = comando.Dto.Estado;
                    if (comando.Dto.MensajeError != null)
                    {
                        transmision.MensajeError = comando.Dto.MensajeError;
                    }
                }
                Log.Info("Se van a guardar los cambios en IngresosBodegaTransmisionASap");
                Repositorio.GuardarCambios();
                resultado.Id = transmision.Id;
                Log.Info("Cambios guardados");
                transaction.Complete();
            }

            return resultado;
        }
    }
}
