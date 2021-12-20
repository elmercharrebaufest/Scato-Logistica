using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CorrespondeAnalisisObligatorio : Comando
    {
        public int PuestoDeTrabajoId { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public int RecorridoId { get; set; }

        public bool Cancelar { get; set; }
    }
}
