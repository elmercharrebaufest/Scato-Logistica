using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class AjusteDeDiferenciasEnRedespachosTransmisionASapDto : TransmisionASapDto
    {
        public string Almacen { get; set; }
        public string Cantidad { get; set; }
        public string Centro { get; set; }
        public string CentroDeCoste { get; set; }
        public string ClaseExpedicion { get; set; }
        public string NroDocumento { get; set; }
        public DateTime FechaContab { get; set; }
        public DateTime FechaDoc { get; set; }
        public string Material { get; set; }
        public string UniMed { get; set; }
    }
}