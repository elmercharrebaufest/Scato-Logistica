namespace Molinos.Scato.Dominio.Comandos
{
    public class LiberarLogAvanceManualCamionConMotivo : Comando
    {
        public int    LogId  { get; set; }
        public string Motivo { get; set; }
    }
}
