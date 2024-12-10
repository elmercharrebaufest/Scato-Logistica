namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarConfirmacionCargaDescarga : Comando
    {
        public bool Confirmar { get; set; }
        public bool DeshabilitarConfirmacion { get; set; }
        public int RecorridoId { get; set; }
    }
}
