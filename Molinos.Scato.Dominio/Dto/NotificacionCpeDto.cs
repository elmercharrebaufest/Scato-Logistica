using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class LecturaCpeDto
    {
        public int Id { get; set; }
        public int PuestoId { get; set; }
        public int CentroId { get; set; }
        public long NroCtg { get; set; }
    }
}
