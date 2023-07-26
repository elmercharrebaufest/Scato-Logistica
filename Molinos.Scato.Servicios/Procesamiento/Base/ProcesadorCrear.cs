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
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public abstract class ProcesadorCrear<TComando, TEntidad> : ProcesadorComando<TComando> where TComando : Comando where TEntidad : class, IIdentificable
    {
        protected ProcesadorCrear(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(TComando comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Validar(comando, resultado);
                if (!resultado.HayErrores)
                {
                    var entidad = CrearEntidad(comando);

                    Repositorio.Agregar(entidad);
                    Repositorio.GuardarCambios();
                    resultado.Id = entidad.Id;
                    Finally(comando, entidad.Id);
                    var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof(LoguearEntidad));
                    if (logueaEntidad && !String.IsNullOrEmpty(comando.Usuario))
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
            }
            catch (CrearException e)
            {
                Log.Error(e, e.Message);
                resultado.Error("", e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al crear la entidad");
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }

        protected abstract TEntidad CrearEntidad(TComando comando);

        protected abstract void Validar(TComando comando, Resultado resultado);

        protected virtual void Finally(TComando comando, int id)
        { }
    }
}