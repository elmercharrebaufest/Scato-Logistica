using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarRemitoBodegaVino : Comando
    {
        public RemitoBodegaVinoDto Orden { get; set; }
        public string NombreUsuario  { get; set; }
        public string NombreWorkflow  { get; set; }
    }
}
