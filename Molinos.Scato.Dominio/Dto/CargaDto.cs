using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Consultas;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class CargaDto
    {
        public int Id { get; set; }

        public string NumeroBalanza { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Buque")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Vapor { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Commodity")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Material { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Bodega")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Bodega { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Exportador")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Exportador { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Destino")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public string Destino { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_PesoProgramado")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int PesoProgramado { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_ToneladasAW")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public int ToneladasAW { get; set; }

        [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_Fecha")]
        [Required(ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_Requerido")]
        public DateTime? Fecha { get; set; }

        // [Display(ResourceType = typeof(Textos), Name = "OperacionesPuerto_FechaInicio")]
        public DateTime? FechaInicio { get; set; }

        public int? IdFin { get; set; }

        public int? Error { get; set; }

        public string ErrorMensaje { get; set; }

        public bool EnviadoASap { get; set; }

        // campos para el autocompletar
        public int VaporId { get; set; }
        public int MaterialId { get; set; }
        public int BodegaId { get; set; }
        public int ExportadorId { get; set; }
        public int DestinoId { get; set; }
        public int? CargaOpuesta_Id { get; set; }
        public string CargaOpuesta_NumeroBalanza { get; set; }

        public string Tipo { get; set; }
        public int PorcentajeDeCarga { get; set; }
        public bool Pediente { get; set; }
    }
}
