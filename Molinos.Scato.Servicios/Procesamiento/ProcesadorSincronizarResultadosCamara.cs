using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorSincronizarResultadosCamara : ProcesadorComando<SincronizarResultadosCamara>
    {

        public ProcesadorSincronizarResultadosCamara(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(SincronizarResultadosCamara comando)
        {
            Log.Debug("Obtengo muestras a actualizar");
            var muestras = comando.Resultados.Select(x => new MuestraEnvioACamaraAuditoria { NumeroDocumentoIngreso = x.NumeroCp, ValorCamara = x.Valor});
            Log.Debug("Muestras a actualizar" + muestras.Count());

            var propiedades = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ValorCamara", null), new KeyValuePair<string, string>("NumeroDocumentoIngreso", null)};
            Repositorio.ActualizarTodos<MuestraEnvioACamaraAuditoria>(muestras, propiedades, "NumeroDocumentoIngreso");
            Log.Debug("Muestras actualizadas");
            return new Resultado();
        }
    }
}