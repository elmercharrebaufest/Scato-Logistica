using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearMuestraDeHumedad : ProcesadorCrear<CrearMuestraDeHumedad, MuestraDeHumedad>
    {
        public ProcesadorCrearMuestraDeHumedad(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override MuestraDeHumedad CrearEntidad(CrearMuestraDeHumedad comando)
        {
            var muestra = Conversor.Convertir<MuestraDeHumedadDto, MuestraDeHumedad>(comando.Dto);
            muestra.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            muestra.Humedimetro = Repositorio.Obtener<Humedimetro>(comando.Dto.HumedimetroId);
            muestra.MotivoHumedadManual = Repositorio.Obtener<MotivoHumedadManual>(comando.Dto.MotivoHumedadManualId);
            return muestra;
        }

        protected override void Validar(CrearMuestraDeHumedad comando, Resultado resultado)
        {
        }
    }
}
