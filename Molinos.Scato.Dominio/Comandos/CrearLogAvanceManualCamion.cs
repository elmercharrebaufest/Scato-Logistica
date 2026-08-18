namespace Molinos.Scato.Dominio.Comandos
{
    public class CrearLogAvanceManualCamion : Comando
    {
        public int   PuestoDeTrabajoId  { get; set; }
        public string NombreUsuario     { get; set; }
        public string MotivoFallo       { get; set; }

        // Campos opcionales, usados cuando el registro proviene de identificación vehicular sin recorrido activo.
        public string PatenteLeida      { get; set; }
        public string PatenteIngresada  { get; set; }
        public string Tarjeta           { get; set; }

        /// <summary>Id del LogIdentificacionVehicular que originó este caso.</summary>
        public int? LogIdentificacionVehicularOrigenId  { get; set; }

        /// <summary>Id del LogIdentificacionVehicular con la patente corregida (null hasta que se resuelva).</summary>
        public int? LogIdentificacionVehicularDestinoId { get; set; }
    }
}

