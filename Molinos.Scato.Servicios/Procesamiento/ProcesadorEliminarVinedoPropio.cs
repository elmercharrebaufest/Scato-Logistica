using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarVinedoPropio : ProcesadorComando<EliminarVinedoPropio>
    {
        public ProcesadorEliminarVinedoPropio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(EliminarVinedoPropio comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                try
                {
                    var vinedoPropio = Repositorio.Obtener<VinedoPropio>(comando.Id);
                    if (vinedoPropio.Cuarteles != null)
                    {
                        var cuarteles = new List<Cuartel>(vinedoPropio.Cuarteles);
                        foreach (var cuartel in cuarteles)
                        {
                            Repositorio.Remover(cuartel);
                        }
                    }
                    Repositorio.Remover(vinedoPropio);
                    Repositorio.GuardarCambios();
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

        private void Validar(EliminarVinedoPropio comando, Resultado resultado)
        {
        }
    }
}
