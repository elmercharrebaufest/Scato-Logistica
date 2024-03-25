using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAutomatismoTipoVariedad : ProcesadorComando<CrearAutomatismoTipoVariedad>
    {
        public ProcesadorCrearAutomatismoTipoVariedad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearAutomatismoTipoVariedad comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearAutomatismoTipoVariedad con AutomatismoId = {0}", comando.IdAutomatismo);
                var automatismo = Repositorio.Obtener<AutomatismoGrano>(comando.IdAutomatismo);
                automatismo.TipoVariedades = new List<TipoVariedad>();

                foreach (var variedad in comando.TipoVariedades)
                {
                    if(variedad == 0)
                    {
                        continue;
                    }

                    var relacionAutomatismoVariedad = Repositorio.Obtener<TipoVariedad>(c => c.Id == variedad);

                    if (!automatismo.TipoVariedades.Contains(relacionAutomatismoVariedad))
                    {
                        automatismo.TipoVariedades.Add(relacionAutomatismoVariedad);
                        Repositorio.GuardarCambios();
                    }
                    else
                    {
                        resultado.Error("", Textos.Error_Generico);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearAutomatismoTipoVariedad con AutomatismoId = {0}", comando.IdAutomatismo);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}