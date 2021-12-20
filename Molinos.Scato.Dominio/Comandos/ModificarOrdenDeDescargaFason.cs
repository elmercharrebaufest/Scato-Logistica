using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarOrdenDeDescargaFason : Comando
    {
        public OrdenDeDescargaFasonDto Orden { get; set; }
    }
}
