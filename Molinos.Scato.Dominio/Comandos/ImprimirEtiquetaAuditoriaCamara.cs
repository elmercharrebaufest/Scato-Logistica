using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ImprimirEtiquetaAuditoriaCamara : Comando
    {
        public ImpEtiquetaAuditoriaCamaraDto Dto { get; set; }
        public int CantidadCopias { get; set; }
    }
}
