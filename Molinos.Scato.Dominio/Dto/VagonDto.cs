using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class VagonDto
    {
        public int Id { get; set; }
        public string NumeroPatente { get; set; }
        public Guid Recorrido { get; set; }
    }
}
