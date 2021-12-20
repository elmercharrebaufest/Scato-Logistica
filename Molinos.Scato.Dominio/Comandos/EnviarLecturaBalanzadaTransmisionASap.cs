using Molinos.Scato.Dominio.Entidades;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EnviarLecturaBalanzadaTransmisionASap : Comando
    {
        public int Id { get; set; }
        public string NumeroBalanza { get; set; }
    }
}
