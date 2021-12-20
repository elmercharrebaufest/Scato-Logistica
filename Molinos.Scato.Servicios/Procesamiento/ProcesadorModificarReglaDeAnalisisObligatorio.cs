using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarReglaDeAnalisisObligatorio : ProcesadorModificar<ModificarReglaDeAnalisisObligatorio>
    {
        public ProcesadorModificarReglaDeAnalisisObligatorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarReglaDeAnalisisObligatorio comando)
        {
            var reglaDeAnalisisObligatorio = Repositorio.Obtener<ReglaDeAnalisisObligatorio>(comando.Dto.Id);
            reglaDeAnalisisObligatorio.Cosecha = comando.Dto.Cosecha;
            reglaDeAnalisisObligatorio.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            reglaDeAnalisisObligatorio.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            reglaDeAnalisisObligatorio.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            reglaDeAnalisisObligatorio.CantidadAnalisis = comando.Dto.CantidadAnalisis;

        }

        protected override void Validar(ModificarReglaDeAnalisisObligatorio comando, Resultado resultado)
        {

        }
    }
}
