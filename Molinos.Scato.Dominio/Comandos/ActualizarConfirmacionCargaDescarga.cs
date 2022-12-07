using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarConfirmacionCargaDescarga : Comando
    {
        public ConfirmacionCargaDescargaDto Dto { get; set; }
    }
}
