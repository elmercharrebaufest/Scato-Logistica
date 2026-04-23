using System;
using System.Threading;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorObtenerPesada : ProcesadorComando<ObtenerPesada>
    {
        private readonly IServicioOrquestador orquestador;

        public ProcesadorObtenerPesada(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioOrquestador orquestador) : base(repositorio, conversor, log)
        {
            this.orquestador = orquestador;
        }        

        public override Resultado Ejecutar(ObtenerPesada comando)
        {
            ResultadoEjecutar resultado = null;
            var hayPesaje = false;
            var res = new ResultadoPesaje(); 
            var contador = 0;
            var balanza = Repositorio.ObtenerProyeccion<PuestoDeTrabajo, BalanzaDto>(x => x.Id == comando.Recorrido.PuestoDeTrabajoId,x=>new BalanzaDto {CodigoCabezal= x.Balanza.CodigoCabezal, Id = x.Balanza.Id });
            if (string.IsNullOrEmpty(balanza.CodigoCabezal))
            {
                res.Error("", "El puesto no tiene Balanza");
            }

            var ejecutarPesaje = new EjecutarPesaje { CodigoDispositivo = balanza.CodigoCabezal };
            while (contador++ < 5 && !hayPesaje)
            {
                try
                {
                    Log.Debug("[ANALISIS_BARRERA] {0} Intento={1} Cabezal={2}", comando.Recorrido.WorkflowInstanceId, contador, balanza.CodigoCabezal);
                    resultado = orquestador.Ejecutar(ejecutarPesaje);
                    hayPesaje = resultado.Mensaje.Codigo == 0;
                    Log.Debug("[ANALISIS_BARRERA] {0} Cabezal={1} HayPeso={2} Codigo={3}", comando.Recorrido.WorkflowInstanceId, balanza.CodigoCabezal, hayPesaje, resultado.Mensaje.Codigo);
                    if (!hayPesaje)
                    {
                        Thread.Sleep(50);
                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error en tomar peso para la balanza con cabezal {0}", balanza);
                    if (res.Errores.ContainsKey(balanza.CodigoCabezal)) res.Errores[balanza.CodigoCabezal] = ex.Message;
                    else res.Error(balanza.CodigoCabezal, ex.Message);
                    resultado = null;
                }

            }
            if (!res.HayErrores)
            {
                if (resultado?.Mensaje.Codigo != 0)
                {
                    res.Error(resultado.Mensaje.Codigo.ToString(), resultado.Mensaje.Descripcion);
                }
                else
                {
                    res.Peso = (int)resultado.Valores["Pesaje"];
                    res.BalanzaId = balanza.Id;
                }
            }            
            return res;
        }


    }
}
