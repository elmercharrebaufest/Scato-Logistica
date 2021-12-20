using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLoteAuditoria : ProcesadorComando<CrearLoteAuditoria>
    {
        public ProcesadorCrearLoteAuditoria(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }
        public override Resultado Ejecutar(CrearLoteAuditoria comando)
        {
            var resultado = new ResultadoCrear();

            using (var transaction = new TransactionScope())
            {
                Log.Info("Iniciando Armado de lote Auditoria");
                resultado.Id = Repositorio.EjecutarComando(new Repositorio.ComandosEF.CrearLoteAuditoria(
                                                comando.Dto.MaterialId, comando.Dto.CamaraId, comando.Dto.CentroId, comando.Dto.CentroDesc,
                                                comando.Dto.NombreUsuario, comando.Dto.Fecha));

                Log.Info("Cambios guardados");
                transaction.Complete();
            }

            return resultado;
        }
    }
}
