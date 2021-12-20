using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios
{
    public interface IServicioMercadoPago
    {
        EstadoPagoDto Pagar(double montoACobrar, string tokenDePago, string idempotencia, string garita, int puestoDeTrabajoId);
        DetalleDeDevolucionDto Reembolsar(string mercadopagoId, string garita, int puestoDeTrabajoId);
        string SolicitarCredencialesVendedor(string codigoObtenido);
    }
}
