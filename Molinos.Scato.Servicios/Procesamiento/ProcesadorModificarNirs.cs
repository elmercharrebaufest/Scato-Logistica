using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarNirs : ProcesadorModificar<ModificarNirs>
    {
        public ProcesadorModificarNirs(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarNirs comando)
        {
            var Nirs = Repositorio.Obtener<Nirs>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, Nirs);
        }

        protected override void Validar(ModificarNirs comando, Resultado resultado)
        {
            if (Repositorio.Existe<Nirs>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("Descripcion", Textos.Nirs_DescripcionExistente);
            }
            if (Repositorio.Existe<Nirs>(e => e.PuestoDeTrabajo == comando.Dto.PuestoDeTrabajo && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("PuestoDeTrabajo", Textos.Nirs_PuestoDeTrabajoExistente);
            }
        }
    }
}
