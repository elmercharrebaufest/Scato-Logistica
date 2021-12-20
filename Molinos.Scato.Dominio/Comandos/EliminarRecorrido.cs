namespace Molinos.Scato.Dominio.Comandos
{
    public class EliminarRecorrido : Comando
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Motivo { get; set; }
    }
}
