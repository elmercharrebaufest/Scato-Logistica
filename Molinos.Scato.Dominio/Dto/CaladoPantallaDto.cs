using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CaladoPantallaDto
    {
        public Guid WorkflowInstanceId { get; set; }
        public int CicloDeCalado { get; set; }
        public string NumeroOrden { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public IList<CaracteristicaPantallaDto> CaladosPorCaracteristica { get; set; }
        [RegularExpression(@"^\d+$", ErrorMessageResourceType = typeof(Textos), ErrorMessageResourceName = "Error_SoloNumerico")]
        public string MuestraConjunto { get; set; }
        public int CentroId { get; set; }
        public int HumedimetroId { get; set; }
        public Modalidad HumedimetroModalidad { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
        public int? MotivoHumedadManualId { get; set; }
        public int MaterialNirsId { get; set; }
        public string NirsCodigoProducto { get; set; }
        public int NirsId { get; set; }
        public bool CamionSeleccionadoAnalisisIntervalo { get; set; }
        public int MaterialId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public int RecorridoId { get; set; }
        public bool EnvioAInase{ get; set; }
    }
}
