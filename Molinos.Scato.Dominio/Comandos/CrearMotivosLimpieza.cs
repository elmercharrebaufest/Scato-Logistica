using System;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearMotivosLimpieza : Comando
    {
        public MotivosLimpiezaDto Dto { get; set; }
    }
}