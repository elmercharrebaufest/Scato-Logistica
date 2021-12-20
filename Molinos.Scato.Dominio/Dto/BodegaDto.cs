using System.ComponentModel.DataAnnotations;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class BodegaDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
    }
}
