using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarTipoComercialPorWf : ProcesadorComando<EliminarTipoComercialPorWf>
    {
        public ProcesadorEliminarTipoComercialPorWf(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarTipoComercialPorWf comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorEliminarTipoComercialPorWf con WorkflowId = {0} y TipoComercialId = {1}", comando.WorkflowId, comando.TipoComercialId);
                var workflow = Repositorio.Obtener<Workflow>(comando.WorkflowId);
                var tipo = Repositorio.Obtener<TipoComercial>(comando.TipoComercialId);
                workflow.TiposComercialesAsociados.Remove(tipo);
                Repositorio.GuardarCambios();
                var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof(LoguearEntidad));
                if (logueaEntidad)
                {
                    try
                    {
                        var logAbm = new LogABM
                        {
                            Pantalla = comando.GetType().Name,
                            Usuario = comando.Usuario,
                            Fecha = DateTime.Now,
                            Evento = EventoABM.Baja,
                            Entidad = comando.ToXml()
                        };
                        Repositorio.Agregar(logAbm);
                        Repositorio.GuardarCambios();
                    }
                    catch (Exception e)
                    {
                        Log.Warn(e, "Ocurrio un error al crear el log AMB Eliminar");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al borrar tipo comercial por workflow con WorkflowId = {0} y TipoComercialId = {1}", comando.WorkflowId, comando.TipoComercialId);
                resultado.Error("", Textos.Error_Generico);
            }

            return resultado;
        }
    }
}
