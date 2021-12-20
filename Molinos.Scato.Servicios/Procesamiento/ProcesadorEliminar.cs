using System;
using System.Linq;
using Molinos.Scato.Dominio;
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
    public abstract class ProcesadorEliminar<TComando, TEntidad> : ProcesadorComando<TComando> where TComando : Comando where TEntidad : class
    {
        protected ProcesadorEliminar(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(TComando comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                Repositorio.Remover<TEntidad>(IdEntidad(comando));
                try
                {
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
                                Entidad = "EntidadId: " + IdEntidad(comando)
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
                catch (EntidadReferenciadaException)
                {
                    resultado.Error("", Textos.Error_EliminarReferenciado);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(TEntidad).Name, IdEntidad(comando));
                    resultado.Error("", Textos.Error_ActualizarGenerico);
                }
            }

            return resultado;
        }

        protected abstract int IdEntidad(TComando comando);

        protected abstract void Validar(TComando comando, Resultado resultado);
    }
}
