using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class CaladoEnPlantaDto
    {
        public int Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public IList<CaladoEnPlantaPorCaracteristicaDto> CaladosPorCaracteristica { get; set; }
        public string Usuario { get; set; }
        public string Comentario { get; set; }
        public int WorkflowDefinicionId { get; set; }
        public string NirsCodigoProducto { get; set; }
        public int MaterialId { get; set; }
        public int PuestoDeTrabajoId { get; set; }
        public int RecorridoId { get; set; }
        public int CentroId { get; set; }
        public string NumeroDocumentoIngreso { get; set; }
        public int? MotivoHumedadManualId { get; set; }
        public int HumedimetroId { get; set; }
        public Modalidad HumedimetroModalidad { get; set; }
        public int NirsId { get; set; }

    }
}
