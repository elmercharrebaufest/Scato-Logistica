using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorRestaurarBalanzadasPerdidas : ProcesadorComando<RestaurarBalanzadasPerdidas>
    {
        private readonly IServicioCarga servicioCarga;

        public ProcesadorRestaurarBalanzadasPerdidas(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioCarga servicioCarga) : base(repositorio, conversor, log)
        {
            this.servicioCarga = servicioCarga;
        }

        public override Resultado Ejecutar(RestaurarBalanzadasPerdidas comando)
        {
            var resultado = new Resultado();
            servicioCarga.RestaurarBalanzadasPerdidas(comando.NumeroBalanza, comando.Desde, comando.Hasta);
            return resultado;
        }
    }
}
