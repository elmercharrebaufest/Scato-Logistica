using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios
{
    public interface IFirmware
    {
        string Ejecutar(LecturaPuestoDeTrabajoDto lecturaDetarjeta);
    }
}
