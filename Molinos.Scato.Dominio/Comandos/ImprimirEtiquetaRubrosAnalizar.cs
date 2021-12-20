using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirEtiquetaRubrosAnalizar : Comando
    {
        public ImpEtiquetaRubrosAnalizarDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}