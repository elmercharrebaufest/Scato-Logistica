using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class NotificacionPesadaAutomaticaDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string Patente { get; set; }
        public string CartaPorte { get; set; }
        public string Material { get; set; }
        public string Entregador { get; set; }
        public string TipoPeso { get; set; }
        public string Peso { get; set; }
        public string Diferencia { get; set; }
        public string Error { get; set; }
        public string Actividad { get; set; }
        public string Tarjeta { get; set; }
        public bool NoRedirecciona { get; set; }
        public string DifPeso { get; set; }
        public string TipoVehiculo { get; set; }
        public string DifNeto { get; set; }
        public bool FotoAlMarcarTarjeta { get; set; }
        public string Calle { get; set; }
        public string TipoPesoOrigen { get; set; }
        public string PesoBrutoOrigen { get; set; }
        public string PesoNetoOrigen { get; set; }
        public string DocumentoIngreso { get; set; }
        public string TipoComercial { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public int? PesoBruto { get; set; }
        public int? PesoTara { get; set; }
        public string PatentePrevia { get; set; }
    }
}
