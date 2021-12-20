
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirEtiquetaPuerto : Comando
    {
        public int ImpresoraId { get; set; }
        public string Impresora { get; set; }
        public int UsuarioId { get; set; }
        public int Id { get; set; }
        public int CentroId { get; set; }
        public ImpEtiquetaPuertoDto Dto { get; set; }
    }
}
