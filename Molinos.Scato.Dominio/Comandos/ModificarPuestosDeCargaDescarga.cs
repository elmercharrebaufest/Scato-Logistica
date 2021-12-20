using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarPuestosDeCargaDescarga : Comando
    {
        public PuestosDeCargaDescargaDto Dto { get; set; }
    }
}
