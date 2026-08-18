namespace Molinos.Scato.Dominio.Comandos
{
    public class DesencolarIdentificacionVehicular : Comando
    {
        public int PuestoDeTrabajoId { get; set; }
        public bool Eliminar { get; set; }
    }
}
