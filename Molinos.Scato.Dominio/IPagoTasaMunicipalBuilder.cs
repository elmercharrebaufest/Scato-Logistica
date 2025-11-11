using Molinos.Scato.Dominio.Comandos.ResultadoServicio;

namespace Molinos.Scato.Dominio
{
    public interface IPagoTasaMunicipalBuilder
    {
        ResultadoConsultarPagoTasaMunicipal ConstruirResultado();
    }
}