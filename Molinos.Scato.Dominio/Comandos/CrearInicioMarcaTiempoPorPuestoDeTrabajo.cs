using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearInicioMarcaTiempoPorPuestoDeTrabajo : Comando
    {
        public int PuestoDeTrabajoId { get; set; }
        public string NumeroDeTarjeta { get; set; }
        public string Patente { get; set; }
        public int CentroId { get; set; }
        public TipoIngresoPorPuesto TipoIngreso { get; set; }
    }
}
