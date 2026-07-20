using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class ReconocimientoPorDiaSemanaDto
    {
        public DateTime Fecha { get; set; }
        public int Reconocidos { get; set; }
        public int NoReconocidos { get; set; }
    }
}
