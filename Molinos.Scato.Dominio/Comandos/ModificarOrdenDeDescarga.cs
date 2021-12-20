using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenDeDescarga : Comando
    {
        public OrdenDeDescargaDto Orden { get; set; }
        public string NombreUsuario { get; set; }
    }
}
