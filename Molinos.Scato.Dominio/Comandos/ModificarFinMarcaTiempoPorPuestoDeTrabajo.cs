namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarFinMarcaTiempoPorPuestoDeTrabajo : Comando
    {
        public string CodigoDispositivo { get; set; }
        public bool EstadoSensor { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
