using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarSuplencia : ProcesadorModificar<ModificarSuplencia>
    {
        public ProcesadorModificarSuplencia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarSuplencia comando)
        {
            var suplenciaEditada = Repositorio.Obtener<Suplencia>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, suplenciaEditada);
            suplenciaEditada.UsuarioASuplantar = Repositorio.Obtener<Usuario>(comando.Dto.UsuarioASuplantarId);
            suplenciaEditada.UsuarioSuplente = Repositorio.Obtener<Usuario>(comando.Dto.UsuarioSuplenteId);
        }

        protected override void Validar(ModificarSuplencia comando, Resultado resultado)
        {
            if (comando.Dto.UsuarioASuplantarId == comando.Dto.UsuarioSuplenteId)
            {
                resultado.Error("UsuarioSuplenteId", Textos.Suplencia_ASuplantarIgualSuplente);
            }
        }
    }
}
