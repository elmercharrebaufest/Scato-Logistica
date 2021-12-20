using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public sealed class CupoMobile
    {
        [Key]
        public int Id { get; set; }
        public int Otorgados { get; set; }

        public int ArribadosDia { get; set; }
        public int ArribadosVencidos { get; set; }
        public int ArribadosFuturos { get; set; }

        public int Descargados { get; set; }
        public int Pendiente { get; set; }
        public int SinRecorrido { get; set; }
        public int Excedente { get; set; }
        public string Material { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public int Orden { get; set; }
        public int SinCupo { get; set; }
        public int DescargadosTodos { get; set; }

        public int CentroId { get; set; }
    }
}