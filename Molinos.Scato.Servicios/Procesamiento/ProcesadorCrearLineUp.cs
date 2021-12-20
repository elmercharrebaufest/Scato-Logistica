using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLineUp : ProcesadorComando<CrearLineUp>
    {
        public ProcesadorCrearLineUp(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearLineUp comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                Log.Info("Se procederá a ejecutar ProcesadorIngresarOrdenCargaInternaFason para la instancia de workflow = {0}", comando.InstanciaWorkflowId);
                var lineup = Repositorio.Obtener<LineUp>(x => x.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflowId);

                if(lineup == null)
                {
                    var embarque = Repositorio.Obtener<Embarque>(x => x.Recorrido.InstanciaWorkflow == comando.InstanciaWorkflowId);
                    lineup = new LineUp
                    {
                        Recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflowId),
                        Embarque = embarque,
                        PlanoDeCarga = new PlanoDeCarga(),
                        ModuloDeCarga = new ModuloDeCarga(),
                        Orden = embarque != null ? embarque.Id : int.MaxValue
                    };
                    comando.LineUp.Ubicacion = lineup.Embarque.Ubicacion;
                    Repositorio.Agregar(lineup);
                }
                else
                {
                    Conversor.Convertir(comando.LineUp, lineup);
                }
                lineup.Embarque.Ubicacion = comando.LineUp.Ubicacion;
                Repositorio.GuardarCambios();
                resultado.Id = (int)lineup.GetType().GetProperty("Id").GetValue(lineup, null);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al intentar crear la orden de carga interna fason para la instancia de workflow {0}", comando.InstanciaWorkflowId);
                resultado.Error("", Textos.OrdenCargaInterna_Error);
            }

            return resultado;
        }
    }
}
