using System;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ValidarAccesoStopBandasHorarias : Comando
    {
        public int Id { get; set; }
        public string CTG { get; set; }
        public string Patente { get; set; }
        public DateTime? Fecha { get; set; }
        public int Reintentos { get; set; }
    }
}
