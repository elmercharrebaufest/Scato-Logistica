using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarDocumentoDeImpresion : ProcesadorModificar<ModificarDocumentoDeImpresion>
    {
        public ProcesadorModificarDocumentoDeImpresion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarDocumentoDeImpresion comando)
        {
            var impresiones = Repositorio.Obtener<DocumentoDeImpresion>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, impresiones);
        }

        protected override void Validar(ModificarDocumentoDeImpresion comando, Resultado resultado)
        {
            if ( Repositorio.Existe<DocumentoDeImpresion>(x => x.Id != comando.Dto.Id && x.Codigo == comando.Dto.Codigo))
            {
                resultado.Error("Codigo", string.Format(Textos.Error_Existente,Textos.Codigo));
            }
        }
    }
}
