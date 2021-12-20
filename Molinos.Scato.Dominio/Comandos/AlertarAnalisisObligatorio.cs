using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AlertarAnalisisObligatorio : Comando
    {
        public List<int> ListaPuestoDeTrabajoId { get; set; }
        public int CentroId { get; set; }
    }
}
