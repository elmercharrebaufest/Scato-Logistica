using Molinos.Scato.Dominio.Consultas;

namespace Molinos.Scato.Dominio.Comandos.Consultas
{
    public class ConsultarExcepcionPagoTicketMunicipal : Comando
    {
        public string Patente { get; set; }

        public Paginacion Paginacion { get; set; }
    }
}

