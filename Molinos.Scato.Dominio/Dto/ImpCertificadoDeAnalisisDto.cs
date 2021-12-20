using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpCertificadoDeAnalisisDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public IList<AnalisisPorCaracteristicaDto> AnalisisPorCaracteristicas { get; set; }
        public IList<CaladoPorCaracteristicaDto> CaladoPorCaracteristicas { get; set; }
        public string Observaciones { get; set; }
        public string HumedadDescripcion { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }

        public string NumeroDeTarjetaAsignada { get; set; }
        public string FechaYhoraDeIngreso { get; set; }
        public string MaterialDesc { get; set; }
        public bool EsSustentable { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroIngreso { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string CTG { get; set; }
        public string TitularDeCartaDePorteCuit { get; set; }
        public string TitularDeCartaDePorteRazon { get; set; }
        public string IntermediarioCuit { get; set; }
        public string IntermediarioRazon { get; set; }
        public string RemitenteComercialCuit { get; set; }
        public string RemitenteComercialRazon { get; set; }
        public string CorredorCuit { get; set; }
        public string Corredor { get; set; }
        public string VendedorCuit { get; set; }
        public string Vendedor { get; set; }
        public string EntregadorCuit { get; set; }
        public string Entregador { get; set; }
        public string AgenteDeComprasCuit { get; set; }
        public string AgenteDeComprasRazon { get; set; }
        public string DestinatarioCuit { get; set; }
        public string DestinatarioRazon { get; set; }
        public string DestinoCuit { get; set; }
        public string DestinoRazon { get; set; }
        public string TransportistaCuit { get; set; }
        public string TransportistaRazon { get; set; }
        public string ChoferCuit { get; set; }
        public string ChoferNombre { get; set; }
        public string ProcedenciaDeLaMercanderiaCodAfip { get; set; }
        public string ProcedenciaDeLaMercanderiaDesc { get; set; }
        public string Cupo { get; set; }
    }
}
