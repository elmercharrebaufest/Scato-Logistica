using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearVariedadPorVinedo : ProcesadorCrear<CrearVariedadPorVinedo, VariedadPorVinedo>
    {
        public ProcesadorCrearVariedadPorVinedo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override VariedadPorVinedo CrearEntidad(CrearVariedadPorVinedo comando)
        {
            var variedad = Repositorio.Obtener<Variedad>(x => x.Id == comando.Dto.VariedadId);
            var vinedo = Repositorio.Obtener<Vinedo>(x => x.Id == comando.Dto.VinedoId);
            return new VariedadPorVinedo
            {
                Variedad = variedad,
                Vinedo = vinedo,
                Cosecha = comando.Dto.Cosecha,
                Hectareas = comando.Dto.Hectareas,
                TopeHectarea = comando.Dto.TopeHectarea,
                AvisoCorte = comando.Dto.AvisoCorte,
            };
        }

        protected override void Validar(CrearVariedadPorVinedo comando, Resultado resultado)
        {
            if (Repositorio.Existe<VariedadPorVinedo>(e => e.Id != comando.Dto.Id && e.Variedad.Id == comando.Dto.VariedadId && e.Vinedo.Id == comando.Dto.VinedoId))
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