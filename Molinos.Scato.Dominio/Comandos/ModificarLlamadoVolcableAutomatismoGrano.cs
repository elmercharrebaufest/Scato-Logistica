namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarLlamadoVolcableAutomatismoGrano : Comando
    {
        public int Id { get; set; }
        public bool EsLLamadoVolcable { get; set; }
    }
}