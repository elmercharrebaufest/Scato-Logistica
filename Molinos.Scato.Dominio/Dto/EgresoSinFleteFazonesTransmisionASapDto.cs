using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class EgresoSinFleteFazonesTransmisionASapDto : TransmisionASapDto
    {
        public Guid WorkflowId { get; set; }
        public string WorkflowCodigo { get; set; }

        public string Almacen { get; set; }
        public string CuitTransportista { get; set; }
        public string CuitClienteDestinatario { get; set; }
        public string Cantidad { get; set; }
        public string CentroId { get; set; }

        public string DocLegal { get; set; }
        public string FechaCon { get; set; }
        public string FechaDoc { get; set; }
        public string CodigoMaterial { get; set; }
        public string NombreChofer { get; set; }
        public string NombreTransportista { get; set; }
        public string NroDocumentoChofer { get; set; }
        public string PatenteCamion { get; set; }
        public string PatenteAcoplado { get; set; }
        public string TipoDocumentoChofer { get; set; }
    }
}