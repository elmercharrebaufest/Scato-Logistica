using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCallePorRecorrido : Comando
    {
        public TipoCalle TipoCalle { get; set; }
        public Guid InstanciaWorkflow { get; set; }
        public int CargaDeCupoId { get; set; }
        public bool TurnoActivo { get; set; }
        public int CentroId { get; set; }
        public int MaterialId { get; set; }
        public bool FlagReasignacionCalle { get; set; }
        public Calle CalleReasignacion { get; set; }
    }
}
