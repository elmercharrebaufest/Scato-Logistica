using System;
using System.Collections.Generic;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpAsignacionDeRutaDto
    {
        public int Id { get; set; }
        private string balanzaBruto;
        private string calle;
        private List<string> hidraulicas;
        private string almacen;
        private string balanzaTara;

        public string Impresora { get; set; }
        public string NumeroDeOrden { get; set; }
        public DateTime FechaImpresion { get; set; }
        public string Patente { get; set; }
        public string PatenteAcoplado { get; set; }
        public string Material { get; set; }
        public string MaterialCodigoSap { get; set; }
        public string BalanzaBruto { get { return String.IsNullOrEmpty(balanzaBruto) ? Textos.SinAsignar : balanzaBruto; } set { balanzaBruto = value; } }
        public string Calle { get { return String.IsNullOrEmpty(calle) ? Textos.SinAsignar : calle; } set { calle = value; } }
        public List<string> Hidraulicas { get { return hidraulicas == null || hidraulicas.Count == 0 ? new List<string>{ Textos.SinAsignar} : hidraulicas; } set { hidraulicas = value; } }
        public string Almacen { get { return String.IsNullOrEmpty(almacen) ? Textos.SinAsignar : almacen; } set { almacen = value; } }
        public string BalanzaTara { get { return String.IsNullOrEmpty(balanzaTara) ? Textos.SinAsignar : balanzaTara; } set { balanzaTara = value; } }
        public string Humedad { get; set; }
        public string Calidad { get; set; }
        public DateTime? FechaCalado { get; set; }
        public Guid WorkflowId { get; set; }
        public string Codigo { get; set; }
        public string Observacion { get; set; }
        public string TipoVehiculo { get; set; }

    }
}
