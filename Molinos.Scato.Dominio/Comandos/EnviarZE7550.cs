using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarZE7550 : Comando
    {
        public TransmisionASapDto transmisionASap { get; set; }
    }
}
