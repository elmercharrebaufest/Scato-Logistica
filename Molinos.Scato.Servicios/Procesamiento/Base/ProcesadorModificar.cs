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
    public abstract class ProcesadorModificar<TComando> : ProcesadorComando<TComando>
        where TComando : Comando
    {
        protected ProcesadorModificar(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(TComando comando)
        {
            var resultado = new Resultado();
            try
            {
                Validar(comando, resultado);
                if (!resultado.HayErrores)
                {
                    ModificarEntidad(comando);
                    Repositorio.GuardarCambios();

                    try
                    {
                        var logueaEntidad = comando.GetType().GetCustomAttributes(true).Any(s => s.GetType() == typeof (LoguearEntidad));
                        if (logueaEntidad && !String.IsNullOrEmpty(comando.Usuario))
                        {
                            var logAbm = new LogABM
                                {
                                    Pantalla = comando.GetType().Name,
                                    Usuario = comando.Usuario,
                                    Fecha = DateTime.Now,
                                    Evento = EventoABM.Modificacion,
                                    Entidad = comando.ToXml()
                                };
                            Repositorio.Agregar(logAbm);
                            Repositorio.GuardarCambios();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warn(e, "Ocurrio un error al crear el log AMB Modificar");
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al modificar la entidad");
                resultado.Error("", Textos.Error_ActualizarGenerico);
                throw;
            }

            return resultado;
            }      

        protected abstract void ModificarEntidad(TComando comando);

        protected abstract void Validar(TComando comando, Resultado resultado);
    }

}
