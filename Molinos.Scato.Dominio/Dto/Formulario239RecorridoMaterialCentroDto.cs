using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Dto
{
    public sealed class Formulario239RecorridoMaterialCentroDto
    {
        public string CentroDescripcion { get; set; }
        public string CentroDireccion { get; set; }
        public string CentroLocalidadDesc { get; set; }
        public string CentroProvinciaDesc { get; set; }

        public string MaterialDescripcion { get; set; }
        public string MaterialCodigoSAP { get; set; }
        public TipoDocumentoIngreso TipoDocumentoIngreso { get; set; }

        public string CTG { get; set; }
    }
}
