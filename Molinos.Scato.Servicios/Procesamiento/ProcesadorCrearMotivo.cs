using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMotivo : ProcesadorCrear<CrearMotivo, Motivo>
    {
        public ProcesadorCrearMotivo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Motivo CrearEntidad(CrearMotivo comando)
        {
            return new Motivo
                    {
                        DescripcionCorta = comando.Dto.DescripcionCorta,
                        Descripcion = comando.Dto.Descripcion
                    };
        }

        protected override void Validar(CrearMotivo comando, Resultado resultado)
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
