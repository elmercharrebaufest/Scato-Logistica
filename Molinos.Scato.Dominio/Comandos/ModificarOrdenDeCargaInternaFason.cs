using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenCargaInternaFason : Comando
    {
        public OrdenCargaInternaFasonDto Orden { get; set; }
        public string NombreWorkflow { get; set; }
        public string NombreUsuario { get; set; }
    }
}
