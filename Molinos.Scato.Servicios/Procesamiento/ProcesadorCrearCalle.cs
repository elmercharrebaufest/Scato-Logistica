using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCalle : ProcesadorCrear<CrearCalle, Calle>
    {
        public ProcesadorCrearCalle(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Calle CrearEntidad(CrearCalle comando)
        {
            var calle = Conversor.Convertir<CalleDto, Calle>(comando.Dto);
            if(comando.Dto.MaterialId > 0)
                calle.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);

            return calle;
        }

        protected override void Validar(CrearCalle comando, Resultado resultado)
        {
            if (Repositorio.Existe<Calle>(e => e.Codigo == comando.Dto.Codigo && e.CentroId == comando.Dto.CentroId && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Codigo", Textos.Calle_CodigoExistente);
            }
        }
    }
}
