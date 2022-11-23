using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearConfirmacionCargaDescarga : Comando
    {
        public ConfirmacionCargaDescargaDto Dto { get; set; }
    }
}
