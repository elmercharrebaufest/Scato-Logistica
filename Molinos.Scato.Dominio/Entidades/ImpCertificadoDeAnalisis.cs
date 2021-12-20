using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.Scato.Dominio.Entidades
{
    [Table("ImpCertificadoDeAnalisis")]
    public class ImpCertificadoDeAnalisis : Impresion
    {

        public virtual string Centro { get; set; }
        public virtual string NumeroDocumento { get; set; }
        public virtual string NumeroIngreso { get; set; }
        public virtual string PatenteAcoplado { get; set; }
        public virtual string Entregador { get; set; }
        public virtual string Corredor { get; set; }
        public virtual string Vendedor { get; set; }
        [InverseProperty("ImpCertificadoDeAnalisiss")]
        public virtual ICollection<AnalisisPorCaracteristica> AnalisisPorCaracteristicas { get; set; }
        [InverseProperty("ImpCertificadoDeAnalisiss")]
        public virtual ICollection<CaladoPorCaracteristica> CaladoPorCaracteristicas { get; set; }
        public virtual string Observaciones { get; set; }
        public virtual string HumedadDescripcion { get; set; }

        public virtual string NumeroDeTarjetaAsignada { get; set; }
        public virtual string FechaYhoraDeIngreso { get; set; }
        public virtual string MaterialDesc { get; set; }
        public virtual bool EsSustentable { get; set; }
        public virtual string CTG { get; set; }
        public virtual string TitularDeCartaDePorteCuit { get; set; }
        public virtual string TitularDeCartaDePorteRazon { get; set; }
        public virtual string IntermediarioCuit { get; set; }
        public virtual string IntermediarioRazon { get; set; }
        public virtual string RemitenteComercialCuit { get; set; }
        public virtual string RemitenteComercialRazon { get; set; }
        public virtual string CorredorCuit { get; set; }
        public virtual string VendedorCuit { get; set; }
        public virtual string EntregadorCuit { get; set; }
        public virtual string AgenteDeComprasCuit { get; set; }
        public virtual string AgenteDeComprasRazon { get; set; }
        public virtual string DestinatarioCuit { get; set; }
        public virtual string DestinatarioRazon { get; set; }
        public virtual string DestinoCuit { get; set; }
        public virtual string DestinoRazon { get; set; }
        public virtual string TransportistaCuit { get; set; }
        public virtual string TransportistaRazon { get; set; }
        public virtual string ChoferCuit { get; set; }
        public virtual string ChoferNombre { get; set; }
        public virtual string ProcedenciaDeLaMercanderiaCodAfip { get; set; }
        public virtual string ProcedenciaDeLaMercanderiaDesc { get; set; }
        public virtual string Cupo { get; set; }
    }
}
