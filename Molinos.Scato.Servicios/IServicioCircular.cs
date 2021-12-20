using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Servicios
{
    public interface IServicioCircular
    {
        void EnviarNotificacionCamionero(string cartaDePorte, string mensaje);
        ArriboCircularResponseDto InformarArribo(string cartaPorte, string patente, string codigoEspecieMaterial);
        void CamionSalioDePlanta(string cartaPorte, bool salidaConExcepcion);
        void InformarDestinoCamionero(string cartaDePorte, string destino, string fila = "");
        void InformarEstadoCalado(string cartaDePorte, IList<AnalisisPorCaracteristicaDto> resultadosCalado, string estado);
        void InformarPesoCircular(string cartaPorte, TipoPesada tipoPesada, int peso);
    }
}
