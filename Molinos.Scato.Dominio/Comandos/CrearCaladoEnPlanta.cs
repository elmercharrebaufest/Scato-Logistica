using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCaladoEnPlanta : Comando
    {
        public CaladoEnPlantaDto CaladoEnPlanta { get; set; }
    }
}
