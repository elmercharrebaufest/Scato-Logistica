using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarDocumentoDeImpresionPorCentro : ProcesadorModificar<ModificarDocumentoDeImpresionPorCentro>
    {
        public ProcesadorModificarDocumentoDeImpresionPorCentro(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarDocumentoDeImpresionPorCentro comando)
        {
                var impresiones = Repositorio.Obtener<DocumentoDeImpresionPorCentro>(comando.Dto.Id);
                Conversor.Convertir(comando.Dto, impresiones);

                if (impresiones.DocumentoDeImpresion.Id != comando.Dto.DocumentoDeImpresionId && comando.Dto.DocumentoDeImpresionId != 0)
                {
                    impresiones.DocumentoDeImpresion =
                        Repositorio.Obtener<DocumentoDeImpresion>(comando.Dto.DocumentoDeImpresionId);
                }
                if (impresiones.FormatoDeImpresion == null ||
                    impresiones.FormatoDeImpresion.Id != comando.Dto.FormatoDeImpresionId)
                {
                    impresiones.FormatoDeImpresion =
                        Repositorio.Obtener<FormatoDeImpresion>(comando.Dto.FormatoDeImpresionId);
                }
                if (impresiones.Impresora.Id != comando.Dto.ImpresoraId)
                {
                    impresiones.Impresora = Repositorio.Obtener<Impresora>(comando.Dto.ImpresoraId);
                }
                if (impresiones.PuestoDeTrabajo == null || impresiones.PuestoDeTrabajo.Id != comando.Dto.PuestoDeTrabajoId)
                {
                    impresiones.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);
                }
        }

        protected override void Validar(ModificarDocumentoDeImpresionPorCentro comando, Resultado resultado)
        {
            if (Repositorio.Existe<DocumentoDeImpresionPorCentro>(x => x.Id != comando.Dto.Id && x.DocumentoDeImpresion.Id == comando.Dto.DocumentoDeImpresionId && x.Centro.Id == comando.Dto.CentroId && ((x.PuestoDeTrabajo == null && comando.Dto.PuestoDeTrabajoId == null) || x.PuestoDeTrabajo.Id == comando.Dto.PuestoDeTrabajoId)))
            {
                resultado.Error("", string.Format(Textos.Error_Existente, Textos.DocumentoDeImpresion));
            }
        }
    }
}
