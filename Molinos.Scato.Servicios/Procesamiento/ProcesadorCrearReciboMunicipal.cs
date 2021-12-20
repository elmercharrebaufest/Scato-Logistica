using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearReciboMunicipal : ProcesadorCrear<CrearReciboMunicipal, ReciboMunicipal>
    {
        public ProcesadorCrearReciboMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override ReciboMunicipal CrearEntidad(CrearReciboMunicipal comando)
        {
            var reciboEditado = Conversor.Convertir<ReciboMunicipalDto, ReciboMunicipal>(comando.Dto);
            reciboEditado.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return reciboEditado;
        }

        protected override void Validar(CrearReciboMunicipal comando, Resultado resultado)
        {
            if (Repositorio.Existe<ReciboMunicipal>(e => e.FechaActivacion == comando.Dto.FechaActivacion && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Descripcion", Textos.ReciboMunicipal_OrdenanzaExistente);
            }
        }
    }
}
