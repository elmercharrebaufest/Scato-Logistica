using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class MicroMuestrasPorCasilleroDto
    {
        public int Id { get; set; }
        public int MuestraId { get; set; }
        public int CasilleroId { get; set; }
        public string CasilleroNumero { get; set; }
        public DateTime Fecha { get; set; }
    }
}
