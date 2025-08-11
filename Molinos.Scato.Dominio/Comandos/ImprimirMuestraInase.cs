using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirMuestraInase : ComandoImpresion
    {
        public new ImpEtiquetaMuestraInaseDto Dto
        {
            get { return (ImpEtiquetaMuestraInaseDto)base.Dto; }
            set { base.Dto = (IDtoConCentroIdMaterialIdWorkflowId)value; }
        }

        public FirmaDto Firma { get; set; }
    }
}
