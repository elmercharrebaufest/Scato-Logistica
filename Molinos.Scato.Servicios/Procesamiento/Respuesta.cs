using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios.Procesamiento
{
    //TODO: Control de errores back-front, para replicar en el resto de los servicios
    public class Respuesta
    {
        public int status { get; set; }
        public string message { get; set; }

        public Respuesta(int status, string message)
        {
            this.status = status;
            this.message = message;
        }
    }
}
