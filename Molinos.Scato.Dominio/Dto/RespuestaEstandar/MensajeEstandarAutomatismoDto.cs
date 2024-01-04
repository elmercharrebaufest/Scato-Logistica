namespace Molinos.Scato.Dominio.Dto
{
    public class MensajeEstandarAutomatismoDto
    {
        public MensajeEstandarAutomatismoDto()
        {
        }

        public MensajeEstandarAutomatismoDto(string message, bool exitoso)
        {
            Mensaje = message;
            Success = exitoso;
        }

        public string Mensaje { get; set; }

        public bool Success { get; set; }

        public override string ToString() => Mensaje;
    }
}