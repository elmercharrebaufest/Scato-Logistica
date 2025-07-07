namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class ResultadoMOAPayCrearModificarCPE
    {
        public bool Resultado { get; set; }

        public string Mensaje { get; set; }

        public int Registros { get; set; }

        public MOAPayConsultarPagosDatos Datos { get; set; }
    }

    public class MOAPayCrearModificarCPEDatos
    {
        public int Id { get; set; }

        public string NumeroDocumento { get; set; }

        public string EnlacePago { get; set; }
    }
}