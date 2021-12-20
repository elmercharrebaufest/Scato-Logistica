using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearCategoria : Comando
    {
        public CategoriaDto Dto { get; set; }
    }
}