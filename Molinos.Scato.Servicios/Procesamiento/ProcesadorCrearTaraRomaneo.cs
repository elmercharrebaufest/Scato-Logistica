using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTaraRomaneo : ProcesadorCrear<CrearTaraRomaneo,TaraRomaneo>
    {
        public ProcesadorCrearTaraRomaneo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TaraRomaneo CrearEntidad(CrearTaraRomaneo comando)
        {
            var taraEditada = Conversor.Convertir<TaraRomaneoDto,TaraRomaneo>(comando.Dto);
            taraEditada.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return taraEditada;
        }

        protected override void Validar(CrearTaraRomaneo comando, Resultado resultado)
        {
            if (Repositorio.Existe<TaraRomaneo>(e => e.Codigo == comando.Dto.Codigo && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Codigo", Textos.Romaneo_CodigoExistente);
            }
        }
    }
}
