namespace Molinos.Scato.Dominio.Comandos.Validaciones
{
    public class ValidarVehiculoDetectado : Comando
    {
        public string CodigoDispositivo { get; set; }
        public string Patente { get; set; }
    }
}
