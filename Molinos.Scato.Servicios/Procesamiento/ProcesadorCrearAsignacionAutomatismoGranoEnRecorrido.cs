using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAsignacionAutomatismoGranoEnRecorrido : ProcesadorCrear<CrearAsignacionAutomatismoGranoEnRecorrido, AsignacionAutomatismoGranoEnRecorrido>
    {
        public ProcesadorCrearAsignacionAutomatismoGranoEnRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override AsignacionAutomatismoGranoEnRecorrido CrearEntidad(CrearAsignacionAutomatismoGranoEnRecorrido comando)
        {
            var recorridoEnAutomatismo = new AsignacionAutomatismoGranoEnRecorrido
            {
                RecorridoId = comando.RecorridoId,
                CallePreBalanzaId = comando.CallePreBalanzaId,
                CallePreHidraulicaId = comando.CallePreHidraulicaId,
            };

            return recorridoEnAutomatismo;
        }

        protected override void Validar(CrearAsignacionAutomatismoGranoEnRecorrido comando, Resultado resultado)
        {
            if (Repositorio.Existe<AsignacionAutomatismoGranoEnRecorrido>(x => x.RecorridoId == comando.RecorridoId))
            {
                resultado.Error(string.Empty, $"Ya existe el recorrido {comando.RecorridoId} en automatismo");
            }
        }
    }
}