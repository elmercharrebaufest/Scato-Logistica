using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarLecturaDeTarjetaPatente : Comando
    {
        public int PuestoDeTrabajoId { get; set; }


        public string Patente { get; set; }
        public string PatenteLeida { get; set; }
        public string Lectura { get; set; }
    }
}
