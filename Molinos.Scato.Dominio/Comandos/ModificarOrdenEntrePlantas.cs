using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenEntrePlantas : Comando
    {
        public OrdenEntrePlantasDto Orden { get; set; }
        public string NombreUsuario { get; set; }
    }
}
