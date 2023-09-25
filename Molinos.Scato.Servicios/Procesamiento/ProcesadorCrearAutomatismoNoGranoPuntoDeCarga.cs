using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAutomatismoNoGranoPuntoDeCarga : ProcesadorComando<CrearAutomatismoNoGranoPuntoDeCarga>
    {
        public ProcesadorCrearAutomatismoNoGranoPuntoDeCarga(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearAutomatismoNoGranoPuntoDeCarga comando)
        {
            var resultado = new Resultado();
            try
            {
                Log.Info("Se está ejecutando ProcesadorCrearAutomatismoNoGranoPuntoDeCarga con AutomatismoNoGranoId = {0} y PuntoDeCargaId = {1}", comando.Dto.AutomatismoNoGranoId, comando.Dto.PuntoDeCargaId);
                var automatismo = Repositorio.Obtener<AutomatismoNoGrano>(comando.Dto.AutomatismoNoGranoId);
                var puntoDeCarga = Repositorio.Obtener<PuntoDeCarga>(comando.Dto.PuntoDeCargaId);

                if (!automatismo.PuntosDeCargaAsociados.Contains(puntoDeCarga))
                {
                    automatismo.PuntosDeCargaAsociados.Add(puntoDeCarga);
                    Repositorio.GuardarCambios();
                }
                else
                {
                    resultado.Error("", Textos.Error_Generico);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error en ProcesadorCrearAutomatismoNoGranoPuntoDeCarga con AutomatismoNoGranoId = {0} y PuntoDeCargaId = {1}", comando.Dto.AutomatismoNoGranoId, comando.Dto.PuntoDeCargaId);
                resultado.Error("", e.Message);
            }

            return resultado;
        }
    }
}