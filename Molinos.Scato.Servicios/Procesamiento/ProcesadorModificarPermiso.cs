using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarPermiso : ProcesadorModificar<ModificarPermiso>
    {
        public ProcesadorModificarPermiso(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarPermiso comando)
        {
            var permiso = Repositorio.Obtener<Permiso>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, permiso);
        }

        protected override void Validar(ModificarPermiso comando, Resultado resultado)
        {
            if (Repositorio.Existe<Permiso>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Permiso_DescripcionExistente);
            }
        }
    }
}
