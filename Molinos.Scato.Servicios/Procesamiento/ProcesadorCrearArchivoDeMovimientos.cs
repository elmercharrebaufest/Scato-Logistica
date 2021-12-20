using System.Transactions;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearArchivoDeMovimientos : ProcesadorComando<CrearArchivoDeMovimientos>
    {
        private readonly IFirmaProvider firmaProvider;

        public ProcesadorCrearArchivoDeMovimientos(IRepositorio repositorio, IConversor conversor, ILogger log, IFirmaProvider firmaProvider)
            : base(repositorio, conversor, log)
        {
            this.firmaProvider = firmaProvider;
        }

        public override Resultado Ejecutar(CrearArchivoDeMovimientos comando)
        {
            var resultado = new ResultadoCrear();
            var firma = firmaProvider.ObtenerFirmaSinLogo();
            using (var transaction = new TransactionScope())
            {
                Log.Info("Iniciando Armado de Archivo de Movimiento de Terceros");
                resultado.Id = Repositorio.EjecutarComando(new Repositorio.ComandosEF.CrearArchivoDeMovimientos(
                                                comando.Dto.MaterialId, comando.Dto.TipoDeWorkflow.Value, comando.Dto.CentroId, comando.Dto.CentroDesc,
                                                comando.Dto.NombreUsuario, comando.Dto.Fecha, firma != null ? firma.DescripcionCorta : ""));

                Log.Info("Cambios guardados");
                transaction.Complete();
            }

            return resultado;
        }
    }
}
