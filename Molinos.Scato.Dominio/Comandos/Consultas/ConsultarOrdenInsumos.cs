using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Dominio.Comandos.Consultas
{
    public class ConsultarOrdenInsumos : Comando
    {
        public string Patente { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public string NumeroOrden { get; set; }
    }

    public class ResultadoConsultarOrdenInsumos : Resultado
    {
        public OrdenCargaInternaDto Dto { get; set; }
    }
}
