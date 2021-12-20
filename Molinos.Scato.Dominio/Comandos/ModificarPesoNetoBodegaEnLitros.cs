using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRecorridoPesoNetoBodegaEnLitros : Comando
    {
        public int BalanzaId { get; set; }
        public bool EstaEnCero { get; set; }
        public Guid WorkflowId { get; set; }
        public decimal PesoNetoBodega { get; set; }
        public int MaterialId { get; set; }
    }
}
