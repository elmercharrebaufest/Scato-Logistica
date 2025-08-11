using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirAsignacionDeRuta : ComandoImpresion
    {
        public new ImpAsignacionDeRutaDto Dto
        {
            get { return (ImpAsignacionDeRutaDto)base.Dto; }
            set { base.Dto = (IDtoConCentroIdMaterialIdWorkflowId)value; }
        }
    }
}
