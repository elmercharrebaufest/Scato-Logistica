using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBodega : ProcesadorCrear<CrearBodega, Bodega>
    {
        public ProcesadorCrearBodega(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Bodega CrearEntidad(CrearBodega comando)
        {
            return Conversor.Convertir<BodegaDto, Bodega>(comando.Dto);
        }

        protected override void Validar(CrearBodega comando, Resultado resultado)
        {
            if (Repositorio.Existe<Bodega>(e => e.Nombre == comando.Dto.Nombre))
            {
                resultado.Error("Descripcion", Textos.Error_Existente);
            }
        }
    }
}
