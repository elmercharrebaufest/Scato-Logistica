using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAdjunto : ProcesadorModificar<ModificarAdjunto>
    {
        public ProcesadorModificarAdjunto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAdjunto comando)
        {
            var Adjunto = Repositorio.Obtener<Adjunto>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, Adjunto);
        }

        protected override void Validar(ModificarAdjunto comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Adjunto>(x => x.Archivo == comando.Dto.Archivo))
            {
                resultado.Error("Adjunto", Textos.Adjunto_YaExisteAdjunto);
            }
        }
    }
}
