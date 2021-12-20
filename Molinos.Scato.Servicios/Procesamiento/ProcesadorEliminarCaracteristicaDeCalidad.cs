using System;
using System.Collections.Generic;
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
    public class ProcesadorEliminarCaracteristicaDeCalidad : ProcesadorComando<EliminarCaracteristicaDeCalidad>
    {
        public ProcesadorEliminarCaracteristicaDeCalidad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(EliminarCaracteristicaDeCalidad comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                try
                {
                    var caracteristica = Repositorio.Obtener<CaracteristicaDeCalidad>(comando.Id);
                    if (caracteristica.Descuentos != null)
                    {
                        var descuentos = new List<Descuento>(caracteristica.Descuentos);
                        foreach (var descuento in descuentos)
                        {
                            Repositorio.Remover(descuento);
                        }
                    }
                    Repositorio.Remover(caracteristica);
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
                    resultado.Error("", Textos.Error_EliminarReferenciado);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(CaracteristicaDeCalidad).Name, comando.Id);
                    resultado.Error("", Textos.Error_ActualizarGenerico);
                }
            }

            return resultado;
        }

        private void Validar(EliminarCaracteristicaDeCalidad comando, Resultado resultado)
        {
        }
    }
}
