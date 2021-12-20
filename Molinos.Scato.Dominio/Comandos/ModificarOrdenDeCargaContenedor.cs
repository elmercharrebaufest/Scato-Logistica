using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenDeCargaContenedor : Comando
    {
        public OrdenDeCargaContenedorDto Orden { get; set; }
        public string NombreWorkflow { get; set; }
        public string NombreUsuario { get; set; }
    }
}
