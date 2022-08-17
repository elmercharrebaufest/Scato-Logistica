using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class SetearProgresoCargaDeCupo : Comando
    {
        public int Id { get; set; }
        public bool EnProgresoAutomatico { get; set; }        
    }
}
