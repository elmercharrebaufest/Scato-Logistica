using System;
using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BalanzadaDto
    {
        public int Id { get; set; }
        public string NumeroBalanza { get; set; }
        public int PesoBruto { get; set; }
        public int PesoTara { get; set; }
        public int PesoNeto { get; set; }
        public string Capacidad { get; set; }
        public DateTime? Fecha { get; set; }
        public bool EnviadoASap { get; set; }
        public CargaDto CargaInicial { get; set; }
        public int CargaInicial_Id { get; set; }
        public string CargaInicial_NumeroBalanza { get; set; }

        public  ModuloDeCargaBalanzasDto ModuloDeCargaBalanzas_Id { get; set; }
    }
}
