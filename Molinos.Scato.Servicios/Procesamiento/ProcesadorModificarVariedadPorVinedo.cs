using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarVariedadPorVinedo : ProcesadorModificar<ModificarVariedadPorVinedo>
    {
        public ProcesadorModificarVariedadPorVinedo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarVariedadPorVinedo comando)
        {
            var vinedo = Repositorio.Obtener<VariedadPorVinedo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, vinedo);
            vinedo.Variedad = Repositorio.Obtener<Variedad>(comando.Dto.VariedadId);
            vinedo.Vinedo = Repositorio.Obtener<Vinedo>(x => x.Id == comando.Dto.VinedoId);
        }

        protected override void Validar(ModificarVariedadPorVinedo comando, Resultado resultado)
        {
            if (Repositorio.Existe<VariedadPorVinedo>(e => e.Id != comando.Dto.Id && e.Variedad.Id == comando.Dto.VariedadId && e.Vinedo.Id == comando.Dto.VinedoId && e.Cosecha == comando.Dto.Cosecha))
            {
                resultado.Error("VariedadId", string.Format(Textos.Error_Existente, Textos.Material_Variedad));
            }
            if (!Repositorio.Existe<Variedad>(e => e.Id == comando.Dto.VariedadId))
            {
                resultado.Error("VariedadId", string.Format(Textos.Error_Requerido, Textos.Material_Variedad));
            }
        }
    }
}
