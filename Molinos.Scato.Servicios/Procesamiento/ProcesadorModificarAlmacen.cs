using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAlmacen : ProcesadorModificar<ModificarAlmacen>
    {
        public ProcesadorModificarAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAlmacen comando)
        {
            var almacenEditado = Repositorio.Obtener<Almacen>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, almacenEditado);
        }

        protected override void Validar(ModificarAlmacen comando, Resultado resultado)
        {
            if (Repositorio.Existe<Almacen>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
            }
        }
    }
}
