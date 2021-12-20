using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarChofer : ProcesadorModificar<ModificarChofer>
    {
        public ProcesadorModificarChofer(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarChofer comando)
        {
            var chofer = Repositorio.Obtener<Chofer>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, chofer);
            if (chofer.TipoDocumentoIdentidad.Id != comando.Dto.TipoDocumentoIdentidadId)
            {
                chofer.TipoDocumentoIdentidad =
                    Repositorio.Obtener<TipoDocumentoIdentidad>(comando.Dto.TipoDocumentoIdentidadId);
            }
        }

        protected override void Validar(ModificarChofer comando, Resultado resultado)
        {
            if (!Repositorio.Existe<TipoDocumentoIdentidad>(e => e.Id == comando.Dto.TipoDocumentoIdentidadId))
            {
                resultado.Error("TipoDocumentoIdentidadId", string.Format(Textos.Error_Requerido, Textos.Chofer_TipoDocumentoIdentidad));
            }
            if (Repositorio.Existe<Chofer>(
                    x =>
                    x.Id != comando.Dto.Id && x.Cuil == comando.Dto.Cuil))
            {
                resultado.Error("Cuil", Textos.Chofer_CuilExistente);
            }
            else if (Repositorio.Existe<Chofer>(
                x =>
                x.Id != comando.Dto.Id && x.NumeroDeDocumento == comando.Dto.NumeroDeDocumento &&
                x.TipoDocumentoIdentidad.Id == comando.Dto.TipoDocumentoIdentidadId))
                    {
                        resultado.Error("NumeroDeDocumento", Textos.Chofer_DocumentoExistente);
                    }
        }
    }
}
