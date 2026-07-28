namespace Molinos.Scato.Dominio.Dto
{
    public class PromedioTaraVehiculoDto
    {
        public PromedioTaraVehiculoDto()
        {
            PatenteCamion = string.Empty;
            PatenteAcoplado = string.Empty;
            PatenteAcoplado2 = string.Empty;
            ChoferApellido = string.Empty;
            ChoferNombre = string.Empty;
            ChoferNumeroDocumento = string.Empty;
            PromedioTara = 0;
        }

        public string PatenteCamion { get; set; }
        public string PatenteAcoplado { get; set; }
        public string PatenteAcoplado2 { get; set; }
        public int PromedioTara { get; set; }
        public string ChoferApellido { get; set; }
        public string ChoferNombre { get; set; }
        public string ChoferNumeroDocumento { get; set; }
    }
}
