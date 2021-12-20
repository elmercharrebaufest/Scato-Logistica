using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCosecha : ProcesadorCrear<CrearCosecha, Cosecha>
    {
        public ProcesadorCrearCosecha(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Cosecha CrearEntidad(CrearCosecha comando)
        {
            return Conversor.Convertir<CosechaDto, Cosecha>(comando.Dto);
        }

        protected override void Validar(CrearCosecha comando, Resultado resultado)
        {
            if (Repositorio.Existe<Cosecha>(x => x.Descripcion == comando.Dto.Descripcion))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Cosecha));
            }
        }
    }
}