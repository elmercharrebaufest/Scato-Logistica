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
    public class ProcesadorEliminarFormatoDeImpresion : ProcesadorComando<EliminarFormatoDeImpresion>
    {
        public ProcesadorEliminarFormatoDeImpresion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarFormatoDeImpresion comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                try
                {
                    var formato = Repositorio.Obtener<FormatoDeImpresion>(comando.Id);
                    var campos = formato.FormatosDeCampo.ToList();
                    foreach (var formatoDeCampo in campos)
                    {
                        Repositorio.Remover(formatoDeCampo);
                    }

                    Repositorio.Remover(formato);
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
                catch (EntidadReferenciadaException)
                {
                    resultado.Error("", Textos.Error_EliminarFormatoReferenciado);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(FormatoDeImpresion).Name, comando.Id);
                    resultado.Error("", Textos.Error_ActualizarGenerico);
                }
            }

            return resultado;
        }

        private void Validar(EliminarFormatoDeImpresion comando, Resultado resultado)
        {
        }
    }
}
