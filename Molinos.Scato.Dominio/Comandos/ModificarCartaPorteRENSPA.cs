namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarCartaPorteRENSPA : Comando
    {
        public int CartaPorteId { get; set; }
        public string CodigoRENSPA { get; set; }
    }
}
