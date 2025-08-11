using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirAsigRecorrCtrolCalid : ComandoImpresion
    {
        public new ImpAsigRecorrCtrolCalidDto Dto
        {
            get { return (ImpAsigRecorrCtrolCalidDto)base.Dto; }
            set { base.Dto = (IDtoConCentroIdMaterialIdWorkflowId)value; }
        }

        public FirmaDto Firma { get; set; }
    }
}
