using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarDocumentoExterno : ProcesadorModificar<ModificarDocumentoExterno>
    {
        public ProcesadorModificarDocumentoExterno(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarDocumentoExterno comando)
        {
            var docExterno = Repositorio.Obtener<DocumentoExterno>(comando.Dto.Id);
            docExterno.NumeroDeDocumento = comando.Dto.NumeroDeDocumento;
        }

        protected override void Validar(ModificarDocumentoExterno comando, Resultado resultado)
        {
            if (Repositorio.Existe<DocumentoExterno>(e => e.NumeroDeDocumento == comando.Dto.NumeroDeDocumento && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("NumeroDeDocumento", string.Format(Textos.Error_Existente, Textos.Chofer_NumeroDocumentoIdentidad));
            }
        }
    }
}
