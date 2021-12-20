using System;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public class ImpEtiquetaPuertoDto
    {
        public int Id { get; set; }
        public string Vapor { get; set; }
        public string Cargador { get; set; }
        public string Mercaderia { get; set; }
        public string Destino { get; set; }
        public string Kg { get; set; }
        public string NumeroLote { get; set; }
        public string Bodega { get; set; }
        public string Control { get; set; }
        public DateTime? Fecha { get; set; }
        public int Usuario_Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public UsuarioDto Usuario { get; set; }
    }
}
