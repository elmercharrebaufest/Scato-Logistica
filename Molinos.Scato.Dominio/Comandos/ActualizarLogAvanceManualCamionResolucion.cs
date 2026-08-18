namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarLogAvanceManualCamionResolucion : Comando
    {
        public int    LogId                                { get; set; }
        public string PatenteIngresada                    { get; set; }
        public int?   LogIdentificacionVehicularDestinoId { get; set; }
    }
}
