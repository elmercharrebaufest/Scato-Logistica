using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Web.Models
{
    public class PlanillaF515ViewModel
    {
        [Display(ResourceType = typeof(Textos), Name = "Centro")]
        public int CentroId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tipo_Pesada")]
        public int TipoPesadaId { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tipo_Comercial")]
        public int TipoComercialId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Balanza_Bruto")]
        public int BalanzaBrutoId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public int MaterialId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Material")]
        public string MaterialDesc { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Balanza_Tara")]
        public int BalanzaTaraId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Almacen_Origen")]
        public int AlmacenOrigenId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Workflow")]
        public int WorkflowId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Almacen_Destino")]
        public int AlmacenDestinoId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Tipo_Vehiculo")]
        public TipoVehiculo TipoVehiculo { get; set; }
        //public int TipoVehiculoInt
        //{
        //    get { return (int)TipoVehiculo; }
        //}
        public int TipoVehiculoInt { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Usuario")]
        public string Usuario { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public int ProveedorId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Proveedor")]
        public string ProveedorDesc { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Patente")]
        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Entregador")]
        public int EntregadorId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Entregador")]
        public string EntregadorDesc { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Recorridos_Activos")]
        public string RecorridosActivos { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Corredor")]
        public int CorredorId { get; set; }
        [Display(ResourceType = typeof(Textos), Name = "Corredor")]
        public string CorredorDesc { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Nivel_Detalle")]
        public int NivelDeDetalleId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Boca_Destino")]
        public int BocaDestinoId { get; set; }


        [Display(ResourceType = typeof(Textos), Name = "Fecha_Ingreso_Desde")]
        public DateTime? FechaIngresoDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Salida_Desde")]
        public DateTime? FechaSalidaDesde { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Ingreso_Hasta")]
        public DateTime? FechaIngresoHasta { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Fecha_Salida_Hasta")]
        public DateTime? FechaSalidaHasta { get; set; }

    }
}