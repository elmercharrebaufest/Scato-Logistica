using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMuestraEnvioACamara : ProcesadorModificar<ModificarEnvioACamara>
    {
        public ProcesadorModificarMuestraEnvioACamara(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEnvioACamara comando)
        {
            var muestraEnvioACamara = Repositorio.Obtener<MuestraEnvioACamara>(comando.Dto.Id);
            muestraEnvioACamara.PesoNeto = comando.Dto.PesoNeto;
            muestraEnvioACamara.FechaDescarga = comando.Dto.FechaDescarga;
            muestraEnvioACamara.TieneAnalisisInterno = comando.Dto.TieneAnalisisInterno;
            muestraEnvioACamara.GeneroMicroMuestras = comando.Dto.GeneroMicroMuestras;
        }

        protected override void Validar(ModificarEnvioACamara comando, Resultado resultado)
        {

        }
    }
}
