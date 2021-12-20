using Molinos.Scato.Dominio.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Comandos
{
    public class AutomatizacionEtapas : Comando
    {
        public DateTime Fecha { get; set; }
        public List<InstanciaWorkflowDto> ListaWorflows { get; set; }
    }
}
