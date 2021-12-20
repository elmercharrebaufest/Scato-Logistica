namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarNotificacion : Comando
    {
        public int Id { get; set; }
        public string Grupos { get; set; }
    }
}
