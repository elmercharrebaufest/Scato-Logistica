using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBalanza : ProcesadorCrear<CrearBalanza, Balanza>
    {
        public ProcesadorCrearBalanza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Balanza CrearEntidad(CrearBalanza comando)
        {
            var balanza = Conversor.Convertir<BalanzaDto, Balanza>(comando.Dto);
            balanza.Centro = Repositorio.Obtener<Centro>(x => x.Id == comando.Dto.CentroId);
            return balanza;
        }

        protected override void Validar(CrearBalanza comando, Resultado resultado)
        {
            if (Repositorio.Existe<Balanza>(e => e.Nombre == comando.Dto.Nombre && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Nombre", Textos.Balanza_DescripcionExistente);
            }

            if (Repositorio.Existe<Balanza>(e => e.PuestoDeTrabajo == comando.Dto.PuestoDeTrabajo && e.Centro.Id == comando.Dto.CentroId && e.TipoVehiculo == comando.Dto.TipoVehiculo))
            {
                resultado.Error("PuestoDeTrabajo", Textos.Balanza_PuestoDeTrabajoExistente);
            }
        }
    }
}
