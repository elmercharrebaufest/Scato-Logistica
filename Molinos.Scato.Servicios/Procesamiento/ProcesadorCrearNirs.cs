using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearNirs : ProcesadorCrear<CrearNirs, Nirs>
    {
        public ProcesadorCrearNirs(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Nirs CrearEntidad(CrearNirs comando)
        {
            var nirs = Conversor.Convertir<NirsDto, Nirs>(comando.Dto);
            nirs.Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId);
            return nirs;
        }

        protected override void Validar(CrearNirs comando, Resultado resultado)
        {
            if (Repositorio.Existe<Nirs>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.Nirs_DescripcionExistente);
            }

            if (Repositorio.Existe<Nirs>(e => e.PuestoDeTrabajo == comando.Dto.PuestoDeTrabajo && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("PuestoDeTrabajo", Textos.Nirs_PuestoDeTrabajoExistente);
            }
        }
    }
}
