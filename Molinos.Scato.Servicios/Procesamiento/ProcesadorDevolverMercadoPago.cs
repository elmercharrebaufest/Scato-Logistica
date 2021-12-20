using System;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System.Linq;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Dominio.Dto;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorDevolverMercadoPago : ProcesadorComando<DevolverMercadoPago>
    {
        private readonly IServicioMercadoPago servicioMercadoPago;

        public ProcesadorDevolverMercadoPago(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioMercadoPago servicioMercadoPago)
            : base(repositorio, conversor, log)
        {
            this.servicioMercadoPago = servicioMercadoPago;
        }

        public override Resultado Ejecutar(DevolverMercadoPago comando)
        {
            var resultado = new ResultadoPagarMercadoPago();
            try
            {
                Log.Debug($"ProcesadorDevolverMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId}");
                var pago = ObtenerPago(comando);
                Validar(comando, pago, resultado);
                if (resultado.HayErrores)
                {
                    return resultado;
                }

                Log.Debug($"ProcesadorDevolverMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId} Idm: {pago.Idempotencia}");
                var detalleDePago = servicioMercadoPago.Reembolsar(pago.MercadoPagoId, string.Empty, 0);
                Log.Debug($"ProcesadorDevolverMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId} Respuesta: {detalleDePago.ToXml()}");

                ActualizarPago(detalleDePago, pago, resultado);

                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar el cobro con Mercado Pago: {e.Message}");
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private void Validar(DevolverMercadoPago comando, PagoConMercadoPago pago, ResultadoPagarMercadoPago resultado)
        {
            if (pago == null || string.IsNullOrEmpty(pago.MercadoPagoId) || pago.Devuelto)
            {
                Log.Error($"ProcesadorDevolverMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId} => Cobro ya devuelto o no realizado para éste camión");
                resultado.Errores.Add("", string.Format(Textos.Error_Reembolso, comando.dto.NumeroDeTarjeta));
            }
        }

        private PagoConMercadoPago ObtenerPago(DevolverMercadoPago comando)
        {
            return Repositorio.ObtenerMayor<PagoConMercadoPago, int>(x => x.Recorrido.Id == comando.dto.RecorridoId, x => x.Id);
        }

        private void ActualizarPago(DetalleDeDevolucionDto detalleDePago, PagoConMercadoPago pago, ResultadoPagarMercadoPago resultado)
        {
            pago.Estado = detalleDePago.Estado;
            pago.Devuelto = true;
            Repositorio.GuardarCambios();
        }

    }
}
