using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearDocumentoDeImpresionPorCentro : ProcesadorCrear<CrearDocumentoDeImpresionPorCentro, DocumentoDeImpresionPorCentro>
    {
        public ProcesadorCrearDocumentoDeImpresionPorCentro(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override DocumentoDeImpresionPorCentro CrearEntidad(CrearDocumentoDeImpresionPorCentro comando)
        {
            var entidad = Conversor.Convertir<DocumentoDeImpresionPorCentroDto, DocumentoDeImpresionPorCentro>(comando.Dto);

            entidad.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            entidad.DocumentoDeImpresion = Repositorio.Obtener<DocumentoDeImpresion>(comando.Dto.DocumentoDeImpresionId);
            entidad.FormatoDeImpresion = Repositorio.Obtener<FormatoDeImpresion>(comando.Dto.FormatoDeImpresionId);
            entidad.Impresora = Repositorio.Obtener<Impresora>(comando.Dto.ImpresoraId);
            entidad.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);

            return entidad;
        }

        protected override void Validar(CrearDocumentoDeImpresionPorCentro comando, Resultado resultado)
        {
            if (Repositorio.Existe<DocumentoDeImpresionPorCentro>(x => x.DocumentoDeImpresion.Id == comando.Dto.DocumentoDeImpresionId && x.Centro.Id == comando.Dto.CentroId && ((x.PuestoDeTrabajo == null && comando.Dto.PuestoDeTrabajoId == null) || x.PuestoDeTrabajo.Id == comando.Dto.PuestoDeTrabajoId)))
            {
                resultado.Error(string.Empty, Textos.Error_DocumentoDeImpresionPorCentro_ConfiguracionExistente);
            }
        }
    }
}
