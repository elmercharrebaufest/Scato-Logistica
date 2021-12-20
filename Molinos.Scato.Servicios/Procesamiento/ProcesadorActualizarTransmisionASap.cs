using System;
using System.Linq.Expressions;
using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Repositorio.ConsultasEF;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarTransmisionASap : ProcesadorComando<ActualizarTransmisionASap>
    {
        public ProcesadorActualizarTransmisionASap(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarTransmisionASap comando)
        {
            var resultado = new ResultadoCrear();
            //Esta transacción es necesaria para que la actualización se ejecute incluso cuando falla la transacción padre.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Iniciando ActualizarTransmisionASap");

                if (!comando.Dto.Id.HasValue)
                {
                    comando.Dto.Id = Repositorio.ObtenerProyeccion<TransmisionASap, int>(x =>(x.InstanciaWorkflow == comando.Dto.InstanciaWorkflow && x.FuncionSap == comando.Dto.FuncionSap), x => x.Id);
                }
                TransmisionASap transmision = null;
                if (comando.Dto.Id.HasValue)
                {
                    transmision = Repositorio.ObtenerConsultaEscalar(new ObtenerTransmisionASap(comando.Dto.Id.Value));
                }

                if (transmision == null)
                {
                    Log.Info("Creando ActualizarTransmisionASap");
                    transmision = Conversor.Convertir<TransmisionASapDto, TransmisionASap>(comando.Dto);

                    Repositorio.Agregar(transmision);
                }
                else
                {
                    Log.Info("Modificando ActualizarTransmisionASap");
                    transmision.Estado = comando.Dto.Estado;                
                    if (!string.IsNullOrEmpty(comando.Dto.MensajeError))
                    {
                        transmision.MensajeError = comando.Dto.MensajeError;
                    }
                }
                Log.Info("Se van a guardar los cambios en ActualizarTransmisionASap");
                Repositorio.GuardarCambios();
                resultado.Id = transmision.Id;
                Log.Info("Cambios guardados");
                transaction.Complete();
            }

            return resultado;
        }
    }
}