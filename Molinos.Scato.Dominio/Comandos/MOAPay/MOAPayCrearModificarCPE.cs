namespace Molinos.Scato.Dominio.Comandos
{
    public class MOAPayCrearModificarCPE : Comando
    {
        public string NumeroDocumento { get; set; }
        public string Dominio { get; set; }
        public string TipoDeVehiculo { get; set; }
        public string CuitInterviniente { get; set; }
    }
}