using System;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class ConfiguracionGeneralDto
    {
        public int Id { get; set; }
        public string Pantalla { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public int? CentroId { get; set; }
        public CentroDto Centro { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
        public string UsuarioUltimaModificacion { get; set; }
    }
}