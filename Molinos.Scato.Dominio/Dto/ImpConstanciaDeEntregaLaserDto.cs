using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpConstanciaDeEntregaLaserDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public string Centro { get; set; }
        public string CentroDireccion { get; set; }
        public string CentroLocalidad { get; set; }
        public string CentroProvincia { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroDeOrden { get; set; }
        public string NumeroCertificacion { get; set; }
        public string Material { get; set; }
        public string Corredor { get; set; }
        public string Vendedor { get; set; }
        public string Entregador { get; set; }
        public string Procedencia { get; set; }
        public string NumeroCartaPorte { get; set; }
        public string PesoBruto { get; set; }
        public string PesoTara { get; set; }
        public string PesoNeto { get; set; }
        public DateTime FechaCartaPorte { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Observaciones { get; set; }
        public string ObservacionesCalado { get; set; }
        public ICollection<CaracteristicasCalidadValorDto> CaracteristicasCalidadValor { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string BalanzaTara { get; set; }
        public string BalanzaBruto { get; set; }
        public string ModeloBalanzaTara { get; set; }
        public string ModeloBalanzaBruto { get; set; }
        public string NroSerieBalanzaTara { get; set; }
        public string NroSerieBalanzaBruto { get; set; }
    }
}
