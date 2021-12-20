using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorLiberarStockSojaEpa : ProcesadorComando<LiberarStockSojaEpa>
    {
        public ProcesadorLiberarStockSojaEpa(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(LiberarStockSojaEpa comando)
        {
            var resultado = new Resultado();
            //Obtengo el recorrido 
            if (!ValidaStockEpa(comando.InstanceId))
            {
                return resultado;
            }
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanceId);
            if (recorrido == null)
            {
                resultado.Error("", Textos.LiberarStockSojaEpaError_SinRecorrido);
                return resultado;
            }
            if (recorrido.Establecimiento == null)
            {
                Log.Error(Textos.LiberarStockSojaEpaError_SinSojaEPA, recorrido.InstanciaWorkflow);
                return resultado;
            }
            if (recorrido.Vehiculo == null || recorrido.Vehiculo.CartaPorte == null)
            {
                resultado.Error("", Textos.LiberarStockSojaEpaError_SinCartaPorte);
                return resultado;
            }
            var stockEstablecimiento = Repositorio.Listar<StockDeEstablecimiento>(x => x.CodigoEstablecimiento == recorrido.Establecimiento.CodigoDeEstablecimiento && x.Cosecha == recorrido.Vehiculo.CartaPorte.Cosecha).FirstOrDefault();
            if (stockEstablecimiento == null)
            {
                resultado.Error("", Textos.LiberarStockSojaEpaError_SinStockDeEstablecimiento);
                return resultado;
            }

            var pesoNeto = (recorrido.PesoBrutoOrigen ?? 0) - (recorrido.PesoTaraOrigen ?? 0);

            if (stockEstablecimiento.StockReservado - pesoNeto >= 0)
            {
                stockEstablecimiento.StockReservado = stockEstablecimiento.StockReservado - pesoNeto;
                Repositorio.GuardarCambios();
            }
            else
            {
                try
                {

                    Repositorio.Agregar(new ControlRecorrido
                    {
                        Actividad = "LiberarStockSojaEpa",
                        ActividadXaml = "LiberarStockSojaEpa",
                        Fecha = DateTime.Now,
                        Comentario = Textos.LiberarStockSojaEpaError_ErrorDescontarStockReservado,
                        NombreUsuario = "",
                        WorkflowInstanceId = comando.InstanceId,
                    });
                    Repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "LiberarStockSojaEpa: Ocurrió un problema con la instancia {0}", recorrido.InstanciaWorkflow);
                }
                Log.Error(Textos.LiberarStockSojaEpaError_ErrorDescontarStockReservado, recorrido.InstanciaWorkflow, recorrido.Patente);
            }

            return resultado;
        }

        private bool ValidaStockEpa(Guid instanceId)
        {
            return Repositorio.ObtenerProyeccion<Recorrido, bool>(x => x.InstanciaWorkflow == instanceId, f => f.TipoComercial.ValidaStockEPA);
        }
    }
}