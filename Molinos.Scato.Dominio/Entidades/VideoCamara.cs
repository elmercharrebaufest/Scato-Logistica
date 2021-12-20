using System.ComponentModel.DataAnnotations;

namespace Molinos.Scato.Dominio.Entidades
{
    public class VideoCamara
    {
        [Key]
        public int Id { get; set; }
        public string Codigo  { get; set; }
        public string Directorio  { get; set; }
        public string Descripcion { get; set; }
        public PuestoDeTrabajo PuestoDeTrabajo { get; set; }
        public ActividadPorDispositivo ActividadPorDispositivo { get; set; }
    }
}
