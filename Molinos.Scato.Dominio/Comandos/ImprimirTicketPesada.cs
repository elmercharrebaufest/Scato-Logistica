using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirTicketPesada : ComandoImpresion
    {
        public new ImpTicketPesadaDto Dto 
        {
            get { return (ImpTicketPesadaDto)base.Dto; }
            set { base.Dto = (IDtoConCentroIdMaterialIdWorkflowId)value; }
        }

        public FirmaDto Firma { get; set; }
    }
}
