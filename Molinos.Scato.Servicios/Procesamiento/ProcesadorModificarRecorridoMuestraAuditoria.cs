using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarRecorridoMuestraAuditoria : ProcesadorComando<ModificarRecorridoMuestraAuditoria>
    {
        public ProcesadorModificarRecorridoMuestraAuditoria(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarRecorridoMuestraAuditoria comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
                recorridoAEditar.EnvioMuestraAuditoria = true;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error("Error al modificar envia muestra de auditoria en {0}", comando.InstanceId);
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
