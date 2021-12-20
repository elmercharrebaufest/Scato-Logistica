using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVehiculoDocumentoInterno : ProcesadorComando<ModificarVehiculoDocumentoInterno>
    {
        public ProcesadorModificarVehiculoDocumentoInterno(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarVehiculoDocumentoInterno comando)
        {
            var resultado = new ResultadoCrear();
            //Esta transacción es necesaria para que la actualización se ejecute incluso cuando falla la transacción padre.
            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarRecorridoDocumentoInterno con InstanceId = {0}, DocumentoInterno = {1} y Numero = {2}", comando.InstanceId, comando.DocumentoInternoSap, comando.NumeroDeDocumentoSap);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.DocumentoInternoSap = comando.DocumentoInternoSap;
                recorridoAEditar.NumeroDeDocumentoSap = comando.NumeroDeDocumentoSap;

                if (recorridoAEditar.Vehiculo != null)
                {
                    Log.Info("Se procederá a ejecutar ProcesadorModificarVehiculoDocumentoInterno con VehiculoId = {0}, DocumentoInterno = {1} y Numero = {2}", recorridoAEditar.Vehiculo.Id, comando.DocumentoInternoSap, comando.NumeroDeDocumentoSap);
                    recorridoAEditar.Vehiculo.DocumentoInternoSap = comando.DocumentoInternoSap;
                    recorridoAEditar.Vehiculo.NumeroDeDocumentoSap = comando.NumeroDeDocumentoSap;   
                }
                Repositorio.GuardarCambios();

                transaction.Complete();
            }
            return resultado;
        }
    }
}
