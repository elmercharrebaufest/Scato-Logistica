using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarPuestoComandoCaladoEnPlanta : Comando
    {
        public AsignacionDto Dto { get; set; }
        public bool BalanzasObligatorias { get; set; }
    }
}
