using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearSuplencia : ProcesadorCrear<CrearSuplencia, Suplencia>
    {
        public ProcesadorCrearSuplencia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Suplencia CrearEntidad(CrearSuplencia comando)
        {
            var suplenciaEditada = Conversor.Convertir<SuplenciaDto, Suplencia>(comando.Dto);
            suplenciaEditada.UsuarioASuplantar = Repositorio.Obtener<Usuario>(comando.Dto.UsuarioASuplantarId);
            suplenciaEditada.UsuarioSuplente = Repositorio.Obtener<Usuario>(comando.Dto.UsuarioSuplenteId);
            return suplenciaEditada;
        }

        protected override void Validar(CrearSuplencia comando, Resultado resultado)
        {
            if (comando.Dto.UsuarioASuplantarId == comando.Dto.UsuarioSuplenteId)
            {
                resultado.Error("UsuarioSuplenteId", Textos.Suplencia_ASuplantarIgualSuplente);
            }
        }
    }
}
