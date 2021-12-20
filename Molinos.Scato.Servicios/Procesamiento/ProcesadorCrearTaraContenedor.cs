using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTaraContenedor : ProcesadorCrear<CrearTaraContenedor, TaraContenedor>
    {
        public ProcesadorCrearTaraContenedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TaraContenedor CrearEntidad(CrearTaraContenedor comando)
        {
            return Conversor.Convertir<TaraContenedorDto, TaraContenedor>(comando.Dto);
        }

        protected override void Validar(CrearTaraContenedor comando, Resultado resultado)
        {
            if (Repositorio.Existe<TaraContenedor>(e => e.CodigoContenedor == comando.Dto.CodigoContenedor && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("CodigoContenedor", string.Format(Textos.Error_Existente, Textos.CodigoContenedor ));
            }
        }
    }
}
