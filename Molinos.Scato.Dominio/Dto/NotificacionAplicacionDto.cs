using Molinos.Scato.Dominio.Enums;
using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class NotificacionAplicacionDto
    {
        public int Id { get; set; }
        public string TipoAccion { get; set; }
        public DateTime FechaAccion { get; set; }
        public string Usuario { get; set; }
        public string Titulo { get; set; }
        public string Detalle { get; set; }
        public string FechaString
        {
            get
            {
                return this.FechaAccion.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
