using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class EtiquetaAuditoriaEnCartaDePorteDetalle : Comando
    {
        public ImpEtiquetaAuditoriaDetalleDto Dto { get; set; }
        public string RutaFotoCartaDePorte { get; set; }
    }
}
