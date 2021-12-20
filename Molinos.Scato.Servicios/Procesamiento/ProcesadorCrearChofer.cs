using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearChofer : ProcesadorCrear<CrearChofer, Chofer>
    {
        public ProcesadorCrearChofer(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override Chofer CrearEntidad(CrearChofer comando)
        {
            return new Chofer
            {
                Apellido = comando.Dto.Apellido,
                Nombre = comando.Dto.Nombre,
                Cuil = comando.Dto.Cuil,
                NumeroDeDocumento = comando.Dto.NumeroDeDocumento,
                TipoDocumentoIdentidad = Repositorio.Obtener<TipoDocumentoIdentidad>(comando.Dto.TipoDocumentoIdentidadId)
            };
        }

        protected override void Validar(CrearChofer comando, Resultado resultado)
        {
            if (
                Repositorio.Existe<Chofer>(
                    x =>
                    x.Id != comando.Dto.Id && (x.NumeroDeDocumento == comando.Dto.NumeroDeDocumento &&
                    x.TipoDocumentoIdentidad.Id == comando.Dto.TipoDocumentoIdentidadId)))
            {
                resultado.Error("NumeroDeDocumento", Textos.Chofer_DocumentoExistente);
            }
            if (
                Repositorio.Existe<Chofer>(
                    x =>
                    x.Id != comando.Dto.Id && x.Cuil == comando.Dto.Cuil))
            {
                resultado.Error("Cuil", Textos.Chofer_CuilExistente);
            }
            if (!Repositorio.Existe<TipoDocumentoIdentidad>(e => e.Id == comando.Dto.TipoDocumentoIdentidadId))
            {
                resultado.Error("TipoDocumentoIdentidadId", string.Format(Textos.Error_Requerido, Textos.Chofer_TipoDocumentoIdentidad));
            }
        }
    }
}