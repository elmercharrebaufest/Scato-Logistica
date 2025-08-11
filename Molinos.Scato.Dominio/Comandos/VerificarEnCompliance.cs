namespace Molinos.Scato.Dominio.Comandos
{
    public class VerificarEnCompliance : Comando
    {
        public string Cuit { get; set; }
        public string Dni { get; set; }
        public string Patente { get; set; }
        public string Planta { get; set; }
    }
}
