using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpGaritaSalidaDto
    {
        public int Id { get; set; }
        public string Impresora { get; set; }
        public Guid WorkflowId { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Codigo { get; set; }
        public string Centro { get; set; }
        public string PatenteAcoplado { get; set; }
        public string NumeroDeTarjetaAsignada { get; set; }
        public string MaterialDesc { get; set; }
        public string NumeroDocumento { get; set; }
        public bool EsSustentable { get; set; }
        public string Calle { get; set; }
        public List<string> Hidraulicas { get { return hidraulicas == null || hidraulicas.Count == 0 ? new List<string> { Textos.SinAsignar } : hidraulicas; } set { hidraulicas = value; } }
        public string Calidad { get; set; }
        public string Almacen { get; set; }
        public string FechaCalado { get; set; }
        public string Humedad { get; set; }
        public string ProteinaAlta { get; set; }
        public string ProteinaBaja { get; set; }
        public string Patente { get; set; }
        private List<string> hidraulicas;

    }
}
