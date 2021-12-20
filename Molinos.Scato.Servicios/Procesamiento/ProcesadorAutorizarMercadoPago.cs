using System;
using System.Threading.Tasks;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorAutorizarMercadoPago : ProcesadorComando<AutorizarMercadoPago>
    {
        private readonly IServicioMercadoPago servicioMercadoPago;

        public ProcesadorAutorizarMercadoPago(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioMercadoPago servicioMercadoPago)
            : base(repositorio, conversor, log)
        {
            this.servicioMercadoPago = servicioMercadoPago;
        }

        public override Resultado Ejecutar(AutorizarMercadoPago comando)
        {
            var resultado = new ResultadoCrear();
            try
            {
                resultado.Mensaje = servicioMercadoPago.SolicitarCredencialesVendedor(comando.CodigoDeAutorizacion);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Erro al solicitar permisos al vendedor: {comando}");
                resultado.Error("", e.Message);
            }
            return resultado;
        }
    }
}
