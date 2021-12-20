namespace Molinos.Scato.Dominio.Comandos
{
    public class ModificarModalidadPuestoDeTrabajo : Comando
    {
        public int IdPuesto { get; set; }
        public bool Automatico { get; set; }
        public string Motivo { get; set; }
        public int CentroId { get; set; }
    }
}
