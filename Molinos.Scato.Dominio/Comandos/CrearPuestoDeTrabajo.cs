using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearPuestoDeTrabajo : Comando
    {
        public PuestoDeTrabajoDto Dto { get; set; }
    }
}
