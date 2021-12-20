using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Dominio.Comandos
{
    public class GuardarAutorizacionComando : Comando
    {
       
        public string Patente { get; set; }
        public int ChoferId { get; set; }
        public string Mensaje { get; set; }
        public string NombreUsuario { get; set; }
        public int Centro { get; set; }
    }
}
