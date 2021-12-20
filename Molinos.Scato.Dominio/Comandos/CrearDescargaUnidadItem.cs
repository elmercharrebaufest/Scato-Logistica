using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearDescargaUnidadItem : Comando
    {
        public DescargaUnidadItemDto Dto { get; set; }
    }
}
