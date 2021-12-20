namespace Molinos.Scato.Dominio.Dto
{
    public class ImpTarjetaDeAccesoDto
    {
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Impresora { get; set; }
        public string Codigo { get; set; }
        public int CentroId { get; set; }

        public int PuestoDeTrabajoId { get; set; }
    }
}
