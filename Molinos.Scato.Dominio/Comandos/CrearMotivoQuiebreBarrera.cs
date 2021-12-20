namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearMotivoQuiebreBarrera : Comando
    {
        public string CodigoDispositivo { get; set; }
        public bool Apertura { get; set; }
    }
}
