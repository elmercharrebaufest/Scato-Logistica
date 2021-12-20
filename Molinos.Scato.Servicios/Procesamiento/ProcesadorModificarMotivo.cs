using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarMotivo : ProcesadorModificar<ModificarMotivo>
    {
        public ProcesadorModificarMotivo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarMotivo comando)
        {
            var tipoDocIdEntidad = Repositorio.Obtener<Motivo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto,tipoDocIdEntidad);
        }

        protected override void Validar(ModificarMotivo comando, Resultado resultado)
        {
            if (Repositorio.Existe<Motivo>(e => e.Descripcion == comando.Dto.Descripcion && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Descripcion));
            }
            if (Repositorio.Existe<Motivo>(e => e.DescripcionCorta == comando.Dto.DescripcionCorta && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("DescripcionCorta", string.Format(Textos.Error_Existente, Textos.DescripcionCorta));
            }
        }
    }
}
