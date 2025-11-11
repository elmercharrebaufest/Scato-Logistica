namespace Molinos.Scato.Dominio.Dto.WebAPI
{
    public class VisecValidationError
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string details { get; set; }
        public VisecValidationError[] validationErrors { get; set; }
        public string propertyName { get; set; }
    }
}