using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTaraContenedor : ProcesadorModificar<ModificarTaraContenedor>
    {
        public ProcesadorModificarTaraContenedor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTaraContenedor comando)
        {
            var tipoDocIdEntidad = Repositorio.Obtener<TaraContenedor>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto,tipoDocIdEntidad);
        }

        protected override void Validar(ModificarTaraContenedor comando, Resultado resultado)
        {
            if (Repositorio.Existe<TaraContenedor>(e => e.Descripcion == comando.Dto.Descripcion && (!comando.Dto.Id.HasValue || e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", string.Format(Textos.Error_Existente, Textos.Descripcion));
            }
        }
    }
}
