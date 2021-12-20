using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearAnalisisDeCalidad : Comando
    {
        public AnalisisPorCaracteristicaDto[] Caracteristicas { get; set; }
        public string NumeroDeOrden { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public int PesoNetoOrigen { get; set; }
        public int CaladoId { get; set; }
    }
}
