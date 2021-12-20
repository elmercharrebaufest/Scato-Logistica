using Molinos.Scato.Dominio.Dto;
using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarStock : Comando
    {
        public DateTime Fecha { get; set; }
        public int? ProveedorId { get; set; }
        public int? MaterialId { get; set; }
    }
}
