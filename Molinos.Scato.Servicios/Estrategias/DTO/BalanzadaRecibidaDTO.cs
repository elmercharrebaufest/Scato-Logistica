using System;

namespace Molinos.Scato.Servicios.Estrategias
{
    public class BalanzadaRecibidaDTO
    {
        //BALANZADA
        public int Id { get; set; }
        public int PesoBruto { get; set; }
        public int PesoTara { get; set; }
        public int PesoNeto { get; set; }
        public string Capacidad { get; set; }

        //INICIO
        public string Commodity { get; set; }
        public string Bodega { get; set; }
        public string Exportador { get; set; }
        public string Destino { get; set; }
        public string Vapor { get; set; }
        public int PesoProgramado { get; set; }

        //FIN

        public DateTime? FechaInicio { get; set; }

        //COMPARTIDO
        public int ToneladasAW { get; set; }
        public string NumeroBalanza { get; set; }
        public string TipoBalanzada { get; set; }
        public DateTime Fecha { get; set; }

        //BALANZA
        public int IdOffset { get; set; }
        public string CodigoDispositivo { get; set; }
        public int OffsetBalanza { get; set; }
        public int UltimaValidacion { get; set; }
        public int IntentosValidacion { get; set; }
    }
}