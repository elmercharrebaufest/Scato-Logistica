namespace Molinos.Scato.Dominio.Comandos
{
    /// <summary>
    /// Comando liviano para el job batch de cacheo de CPE: consulta AFIP y persiste
    /// la respuesta en CartaPorteElectronica, sin el enriquecimiento interactivo
    /// (Proveedores/Localidad/Categoria/Chofer/Transportista ni render de PDF) que
    /// realiza ConsultarCPDigital.
    /// </summary>
    public class CachearCPEAfip : Comando
    {
        public int CentroId { get; set; }
        public long NroCTG { get; set; }
        public int TipoVehiculo { get; set; }
    }
}
