using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenCargaFas : Comando
    {
        public OrdenCargaFasDto Orden { get; set; }
        public string NombreUsuario { get; set; }
    }
}
