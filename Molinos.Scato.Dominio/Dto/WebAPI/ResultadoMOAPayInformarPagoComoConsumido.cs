using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResultadoMOAPayInformarPagoComoConsumido
    {
        public bool Resultado { get; set; }

        public string Mensaje { get; set; }

        public int Registros { get; set; }

        public List<string> Datos { get; set; }
    }
}