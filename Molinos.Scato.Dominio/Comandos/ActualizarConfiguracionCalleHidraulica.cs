using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ActualizarConfiguracionCalleHidraulica : Comando
    {
        public int Id { get; set; }
        public int CalleId { get; set; }
        public string CalleNombre { get; set; }
        public string CodigoCartel { get; set; }
        public string CodigoSensorCamaraALPR { get; set; }
        public string CodigoSensorCirculacion { get; set; }
        public string CodigoCamaraALPR { get; set; }
    }
}
