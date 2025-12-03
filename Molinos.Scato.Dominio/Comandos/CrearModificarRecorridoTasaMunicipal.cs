namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearModificarRecorridoTasaMunicipal : Comando
    {
        public int Id { get; set; }
        public bool TieneExcepcion { get; set; }
        public string MotivoExcepcion { get; set; }
    }
}