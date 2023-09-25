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
    public class ProcesadorCrearAutomatismoHidraulica : ProcesadorComando<CrearAutomatismoHidraulicas>
    {
        public ProcesadorCrearAutomatismoHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearAutomatismoHidraulicas comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearAutomatismoHidraulica con AutomatismoId = {0}", comando.IdAutomatismo);
                var automatismo = Repositorio.Obtener<AutomatismoGrano>(comando.IdAutomatismo);
                automatismo.Hidraulicas = new List<PuestosDeCargaDescarga>();

                foreach (var hidraulica in comando.Hidraulicas)
                {
                    var relacionAutomatismoHidraulica = Repositorio.Obtener<PuestosDeCargaDescarga>(c => c.Id == hidraulica);

                    if (!automatismo.Hidraulicas.Contains(relacionAutomatismoHidraulica))
                    {
                        automatismo.Hidraulicas.Add(relacionAutomatismoHidraulica);
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
                Log.Error(e, "Ocurrió un error en ProcesadorCrearAutomatismoHidraulica con AutomatismoId = {0}", comando.IdAutomatismo);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}