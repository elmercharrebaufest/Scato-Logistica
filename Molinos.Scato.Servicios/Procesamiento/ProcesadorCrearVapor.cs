using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearVapor : ProcesadorCrear<CrearVapor, Vapor>
    {
        public ProcesadorCrearVapor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Vapor CrearEntidad(CrearVapor comando)
        {
            return Conversor.Convertir<VaporDto, Vapor>(comando.Dto);
        }

        protected override void Validar(CrearVapor comando, Resultado resultado)
        {
            if (Repositorio.Existe<Vapor>(e => e.Nombre == comando.Dto.Nombre))
            {
                resultado.Error("Descripcion", Textos.Error_Existente);
            }
        }
    }
}
