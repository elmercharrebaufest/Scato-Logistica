using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDocumentoDeImpresion : ProcesadorCrear<CrearDocumentoDeImpresion, DocumentoDeImpresion>
    {
        public ProcesadorCrearDocumentoDeImpresion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override DocumentoDeImpresion CrearEntidad(CrearDocumentoDeImpresion comando)
        {
            return Conversor.Convertir<DocumentoDeImpresionDto, DocumentoDeImpresion>(comando.Dto);
        }

        protected override void Validar(CrearDocumentoDeImpresion comando, Resultado resultado)
        {
            if (Repositorio.Existe<DocumentoDeImpresion>(x => x.Codigo == comando.Dto.Codigo))
            {
                resultado.Error("Codigo", string.Format(Textos.Error_Existente,Textos.Codigo));
            }
        }
    }
}
