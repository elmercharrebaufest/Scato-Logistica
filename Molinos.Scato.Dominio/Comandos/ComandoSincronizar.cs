namespace Molinos.Scato.Dominio.Comandos
{
    public abstract class ComandoSincronizar : Comando
    {
        public string Cuit { get; set; }
        public bool CargaMasiva { get; set; }
        public bool RetornarResultado { get; set; }
    }
}