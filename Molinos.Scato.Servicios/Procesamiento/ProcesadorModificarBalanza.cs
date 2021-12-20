using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarBalanza : ProcesadorModificar<ModificarBalanza>
    {
        public ProcesadorModificarBalanza(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarBalanza comando)
        {
            var balanza = Repositorio.Obtener<Balanza>(comando.Dto.Id);
            comando.Dto.EstaEnCero = ((balanza.CodigoCabezal == comando.Dto.CodigoCabezal) && balanza.EstaEnCero);

            Conversor.Convertir(comando.Dto, balanza);
            if (balanza.Centro.Id != comando.Dto.CentroId)
            {
                balanza.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            }
        }

        protected override void Validar(ModificarBalanza comando, Resultado resultado)
        {
            if (Repositorio.Existe<Balanza>(e => e.Nombre == comando.Dto.Nombre && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id))
            {
                resultado.Error("Nombre", Textos.Balanza_DescripcionExistente);
            }
            if (Repositorio.Existe<Balanza>(e => e.PuestoDeTrabajo == comando.Dto.PuestoDeTrabajo && e.Centro.Id == comando.Dto.CentroId && e.Id != comando.Dto.Id && e.TipoVehiculo == comando.Dto.TipoVehiculo))
            {
                resultado.Error("PuestoDeTrabajo", Textos.Balanza_PuestoDeTrabajoExistente);
            }
        }
    }
}
