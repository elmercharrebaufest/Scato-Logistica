using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirConstanciaDeEntregaLaser : ComandoImpresion
    {
        public new ImpConstanciaDeEntregaLaserDto Dto
        {
            get { return (ImpConstanciaDeEntregaLaserDto)base.Dto; }
            set { base.Dto = (IDtoConCentroIdMaterialIdWorkflowId)value; }
        }

        public FirmaDto Firma { get; set; }
    }
}
