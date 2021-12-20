using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenCargaInterna : Comando
    {
        public OrdenCargaInternaDto Orden { get; set; }
        public string NombreWorkflow { get; set; }
        public string NombreUsuario { get; set; }
    }
}
