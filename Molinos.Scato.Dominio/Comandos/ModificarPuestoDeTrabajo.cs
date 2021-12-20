using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarPuestoDeTrabajo : Comando
    {
        public PuestoDeTrabajoDto Dto { get; set; }
    }
}
