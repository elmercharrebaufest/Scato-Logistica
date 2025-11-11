using Molinos.Scato.Dominio.Comandos;

namespace Molinos.Scato.Servicios
{
    public interface IHangfireQueue
    {
        Resultado EncolarImportarCartaPorteVisec(int id);
    }
}