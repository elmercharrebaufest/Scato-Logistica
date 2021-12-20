using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTaraRomaneo : ProcesadorModificar<ModificarTaraRomaneo>
    {

        public ProcesadorModificarTaraRomaneo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }


        protected override void ModificarEntidad(ModificarTaraRomaneo comando)
        {
            var taraEditado = Repositorio.Obtener<TaraRomaneo>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, taraEditado);
        }

        protected override void Validar(ModificarTaraRomaneo comando, Resultado resultado)
        {
            if (Repositorio.Existe<TaraRomaneo>(e => e.Codigo == comando.Dto.Codigo && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Codigo", Textos.Romaneo_CodigoExistente);
            }
        }
    }
}
