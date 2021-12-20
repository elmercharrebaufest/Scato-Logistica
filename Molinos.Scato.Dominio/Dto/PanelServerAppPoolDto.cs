using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using Molinos.Scato.Dominio.Recursos;

namespace Molinos.Scato.Dominio.Dto
{
    public class PanelServerAppPoolDto
    {
        [Display(ResourceType = typeof(Textos), Name = "Servidor")]
        public string ServidorNombre { get; set; }
        public int DiscoDisponible { get; set; }
        public int Sesiones { get; set; }
        public int MemoriaDisponible { get; set; }
        public int CargaProcesador { get; set; }
        public int CantidadDeConexiones { get; set; }
        public string Error { get; set; }
        public IEnumerable<AppPoolDto> AppPools { get; set; }
        public IEnumerable<EventLogEntry> ListaEventLogs { get; set; }
        public string DatosServer
        {
            get
            {
                return String.Format("Disco:{0}gb Cpu:{1}% Mem:{2}mb Sesiones:{3}",
                                     DiscoDisponible.ToString(CultureInfo.InvariantCulture),
                                     CargaProcesador.ToString(CultureInfo.InvariantCulture),
                                     MemoriaDisponible.ToString(CultureInfo.InvariantCulture),
                                     Sesiones.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
