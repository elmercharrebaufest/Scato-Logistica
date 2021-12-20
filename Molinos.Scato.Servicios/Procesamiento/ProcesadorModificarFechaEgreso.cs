using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarFechaEgreso : ProcesadorComando<ModificarFechaEgreso>
    {
        public ProcesadorModificarFechaEgreso(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarFechaEgreso comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorModificarFechaEgreso con InstanciaWorkflow = {0}, Fecha = {1}", comando.WorkflowInstanciaId, comando.Fecha);
                var recorridoAEditar = Repositorio.Obtener<Recorrido>(r => r.InstanciaWorkflow == comando.WorkflowInstanciaId);
                recorridoAEditar.FechaEgreso = comando.Fecha;
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, "Ha ocurrido un error en ProcesadorModificarFechaEgreso con InstanciaWorkflow = {0}, Fecha = {1}", comando.WorkflowInstanciaId, comando.Fecha);
                resultado.Error("", Textos.Error_Generico);
            }
            return resultado;
        }
    }
}
