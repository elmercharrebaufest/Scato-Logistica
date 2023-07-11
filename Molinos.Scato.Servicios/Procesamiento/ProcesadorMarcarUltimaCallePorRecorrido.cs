using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorMarcarUltimaCallePorRecorrido : ProcesadorComando<MarcarUltimaCallePorRecorrido>
    {

        public ProcesadorMarcarUltimaCallePorRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(MarcarUltimaCallePorRecorrido comando)
        {
            var ultimoCamion = Repositorio.ObtenerMayor<CallePorRecorrido, int>(x => x.FechaEgreso == null && x.Calle.Id == comando.CalleId, x => x.Id);
            if(ultimoCamion != null)
            {
                ultimoCamion.UltimoDeLaFila = true;
                
                Repositorio.GuardarCambios();
            }
            return new Resultado();
        }
    }
}