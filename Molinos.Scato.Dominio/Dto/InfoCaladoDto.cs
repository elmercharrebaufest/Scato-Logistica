using System;

namespace Molinos.Scato.Dominio.Dto
{
    public class InfoCaladoDto
    {
        public Boolean TrigoEspecial { get; set; }

        public string TitularCartaPorte { get; set; }

        public string Entregador { get; set; }

        public string AgenteCompras { get; set; }

        public string RtteComercial { get; set; }

        public string CTG { get; set; }

        public string Cupo { get; set; }

        public bool EnvioDirectoCamara { get; set; }

        public string Procedencia { get; set; }
    }
}
