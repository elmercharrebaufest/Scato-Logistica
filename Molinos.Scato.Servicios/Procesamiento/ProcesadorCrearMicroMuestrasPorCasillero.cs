using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMicroMuestrasPorCasillero : ProcesadorCrear<CrearMicroMuestrasPorCasillero, MicroMuestrasPorCasillero>
    {
        public ProcesadorCrearMicroMuestrasPorCasillero(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MicroMuestrasPorCasillero CrearEntidad(CrearMicroMuestrasPorCasillero comando)
        {
            return new MicroMuestrasPorCasillero
            {
                Muestra = Repositorio.Obtener<MuestraEnvioACamara>(comando.Dto.MuestraId),
                Casillero = Repositorio.Obtener<Casillero>(comando.Dto.CasilleroId),
                Fecha = comando.Dto.Fecha
            };
        }

        protected override void Validar(CrearMicroMuestrasPorCasillero comando, Resultado resultado)
        {
            if (Repositorio.Obtener<MuestraEnvioACamara>(comando.Dto.MuestraId).GeneroMicroMuestras)
            {
                resultado.Error("ExisteGeneracion", Textos.MicroMuestra_ExisteGeneracion);
            }
        }
    }
}
