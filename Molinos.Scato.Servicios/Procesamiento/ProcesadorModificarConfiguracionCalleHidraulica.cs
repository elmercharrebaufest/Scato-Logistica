using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarConfiguracionCalleHidraulica: ProcesadorModificar<ModificarConfiguracionCalleHidraulica>
    {
        public ProcesadorModificarConfiguracionCalleHidraulica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarConfiguracionCalleHidraulica comando)
        {
            var hidraulicaCalle = Repositorio.Obtener<ConfiguracionCalleHidraulica>(comando.Dto.Id);
            hidraulicaCalle.CodigoCamaraALPR = comando.Dto.CodigoCamaraALPR;
            hidraulicaCalle.CodigoCartel = comando.Dto.CodigoCartel;
            hidraulicaCalle.CodigoSensorCirculacion = comando.Dto.CodigoSensorCirculacion;
            hidraulicaCalle.CodigoSensorCamaraALPR = comando.Dto.CodigoSensorCamaraALPR;
            hidraulicaCalle.Calle = Repositorio.Obtener<Calle>(comando.Dto.CalleId);
        }

        protected override void Validar(ModificarConfiguracionCalleHidraulica comando, Resultado resultado)
        {
            if (Repositorio.Existe<ConfiguracionCalleHidraulica>(x => x.Calle.Id == comando.Dto.CalleId && x.Id != comando.Dto.Id))
            {
                resultado.Error("CalleId", string.Format(Textos.Error_Existente, Textos.Calle));
            }

        }
    }
}
