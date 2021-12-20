using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAlmacen : ProcesadorCrear<CrearAlmacen, Almacen>
    {
        public ProcesadorCrearAlmacen(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Almacen CrearEntidad(CrearAlmacen comando)
        {
            var almacenEditado = Conversor.Convertir<AlmacenDto, Almacen>(comando.Dto);
            almacenEditado.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return almacenEditado;
        }

        protected override void Validar(CrearAlmacen comando, Resultado resultado)
        {
            if (Repositorio.Existe<Almacen>(e => e.Descripcion == comando.Dto.Descripcion && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.Almacen_DescripcionExistente);
            }
        }
    }
}
