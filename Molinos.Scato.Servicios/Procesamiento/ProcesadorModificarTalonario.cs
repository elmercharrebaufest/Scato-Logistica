using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTalonario: ProcesadorModificar<ModificarTalonario>
    {
        public ProcesadorModificarTalonario(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }


        protected override void ModificarEntidad(ModificarTalonario comando)
        {
            var talonarioEditado = Repositorio.Obtener<Talonario>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, talonarioEditado);
        }

        protected override void Validar(ModificarTalonario comando, Resultado resultado)
        {
            if (Repositorio.Existe<Talonario>(e => e.Descripcion == comando.Dto.Descripcion && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.Talonario_DescripcionExistente);
            }
        }
    }
}
