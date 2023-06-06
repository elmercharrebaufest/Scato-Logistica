using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System.ComponentModel.DataAnnotations;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class InfoPatenteDeCalleDto
    {

        public string Patente { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InfoPatenteDeCalle_CartaPorte")]
        public string CartaPorte { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InfoPatenteDeCalle_NombreChofer")]
        public string NombreChofer { get; set; }

        public int? RecorridoId { get; set; }
        public int? CargaDeCupoId { get; set; }

        public int CalleId { get; set; }
        public Guid? InstanciaWorflow { get; set; }
        public bool Rechazado { get; set; }
        public TipoCalle TipoCalle { get; set; }
        public bool PermisoReasignarCallePostCalado { get; set; }
        public TipoCalidad TipoCalidad { get; set; }
        public int MaterialId { get; set; }
        public int? CaladoId { get; set; }
        public TipoCalidad CalidadCamion { get; set; }
        public string Tarjeta { get; set; }
        public int? WorkflowDefinicionId { get; set; }
        public string Etapa { get; set; }
        public bool CalleNoGrano { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InfoPatenteDeCalle_NombreWorkflow")]
        public string NombreWorkflow { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "InfoPatenteDeCalle_FechaIngreso")]
        public DateTime FechaIngreso { get; set; }
        public TipoDocumentoIngreso? TipoDocumento { get; set; }
        public string Cliente { get; set; }
        public string Material { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "Tipo_Vehiculo")]
        public TipoVehiculo? TipoVehiculo { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "DescripcionAlmacen")]
        public string DescripcionAlmacen { get; set; }
        public bool CorrespondeConfirmarCargaDescarga { get; set; }
        public bool EsSojaEPA { get; set; }
        public bool EsSojaIMPO { get; set; }
    }
}
