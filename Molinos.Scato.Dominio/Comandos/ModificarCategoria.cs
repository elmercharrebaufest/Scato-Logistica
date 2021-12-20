using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCategoria : Comando
    {
        public CategoriaDto Dto { get; set; }
    }
}