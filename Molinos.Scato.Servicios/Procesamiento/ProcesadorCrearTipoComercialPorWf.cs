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
    public class ProcesadorCrearTipoComercialPorWf : ProcesadorComando<CrearTipoComercialPorWf>
    {
        public ProcesadorCrearTipoComercialPorWf(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearTipoComercialPorWf comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearTipoComercialPorWf con WorkflowId = {0} y TipoComercialId = {1}", comando.Dto.WorkflowId, comando.Dto.TipoComercialId);
                var workflow = Repositorio.Obtener<Workflow>(comando.Dto.WorkflowId);
                var tipoComercial = Repositorio.Obtener<TipoComercial>(comando.Dto.TipoComercialId);

                if (!workflow.TiposComercialesAsociados.Contains(tipoComercial))
                {
                    workflow.TiposComercialesAsociados.Add(tipoComercial);
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
                                Evento = EventoABM.Alta,
                                Entidad = comando.ToXml()
                            };
                            Repositorio.Agregar(logAbm);
                            Repositorio.GuardarCambios();
                        }
                        catch (Exception e)
                        {
                            Log.Warn(e, "Ocurrio un error al crear el log AMB Crear");
                        }
                    }
                }
                else
                {
                    resultado.Error("", Textos.TipoComercialPorWf_ErrorExistente);
                }               
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearTipoComercialPorWf con WorkflowId = {0} y TipoComercialId = {1}", comando.Dto.WorkflowId, comando.Dto.TipoComercialId);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}
