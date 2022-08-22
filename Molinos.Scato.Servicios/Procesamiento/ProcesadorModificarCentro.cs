using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCentro : ProcesadorModificar<ModificarCentro>
    {
        public ProcesadorModificarCentro(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCentro comando)
        {
            var centroEditado = Repositorio.Obtener<Centro>(comando.Dto.Id);
            var avanzaCpeOriginal = centroEditado.AvanzaCpe;
            Conversor.Convertir(comando.Dto, centroEditado);
            centroEditado.AvanzaCpe = avanzaCpeOriginal;
            centroEditado.CamaraDefault = Repositorio.Obtener<Camara>(comando.Dto.CamaraId);
            centroEditado.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            centroEditado.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
        }

        protected override void Validar(ModificarCentro comando, Resultado resultado)
        {
            if (Repositorio.Existe<Centro>(x => x.Id != comando.Dto.Id && x.CodigoSAP == comando.Dto.CodigoSAP))
            {
                resultado.Error("CodigoSAP", Textos.Centro_CodigoSAPExistente);
            }
        }
    }
}
