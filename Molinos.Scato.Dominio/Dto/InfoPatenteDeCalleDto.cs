using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Dominio.Dto
{
    public class InfoPatenteDeCalleDto
    {

        public string Patente { get; set; }

        public string CartaPorte { get; set; }

        public string NombreChofer { get; set; }

        public int RecorridoId { get; set; }

        public int CalleId { get; set; }
        public Guid InstanciaWorflow { get; set; }
        public bool Rechazado { get; set; }
        public TipoCalle TipoCalle { get; set; }
        public bool PermisoReasignarCallePostCalado { get; set; }
        public TipoCalidad TipoCalidad { get; set; }
        public int MaterialId { get; set; }
        public int CaladoId { get; set; }
        public TipoCalidad CalidadCamion { get; set; }
        public string Tarjeta { get; set; }
        public Guid InstanceId { get; set; }
        public string Etapa { get; set; }
    }
}