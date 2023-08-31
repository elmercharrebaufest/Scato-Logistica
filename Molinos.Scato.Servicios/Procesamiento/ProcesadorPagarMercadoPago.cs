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
    public class ProcesadorPagarMercadoPago : ProcesadorComando<PagarMercadoPago>
    {
        private readonly IServicioMercadoPago servicioMercadoPago;

        public ProcesadorPagarMercadoPago(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioMercadoPago servicioMercadoPago)
            : base(repositorio, conversor, log)
        {
            this.servicioMercadoPago = servicioMercadoPago;
        }

        public override Resultado Ejecutar(PagarMercadoPago comando)
        {
            var resultado = new ResultadoPagarMercadoPago();
            try
            {
                Log.Debug($"ProcesadorPagarMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId}");

                var pago = ObtenerPago(comando);
                Validar(comando, pago, resultado);
                if (resultado.HayErrores)
                {
                    return resultado;
                }
                
                Log.Debug($"ProcesadorPagarMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId} Idm: {pago.Idempotencia}");
                var detalleDePago = servicioMercadoPago.Pagar(decimal.ToDouble(comando.dto.Monto), pago.Token, pago.Idempotencia, comando.dto.NombreGarita, comando.dto.PuestoDeTrabajoId);
                Log.Debug($"ProcesadorPagarMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId} Respuesta: {detalleDePago.ToXml()}");

                resultado.DetalleDePago = detalleDePago;
                ActualizarPago(detalleDePago, pago, resultado);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error al procesar el cobro con Mercado Pago: {e.Message}");
                resultado.Error("", e.Message);
            }
            return resultado;
        }

        private void Validar(PagarMercadoPago comando, PagoConMercadoPago pago, ResultadoPagarMercadoPago resultado)
        {
            if (pago != null && (pago.Estado != "Error" && pago.Estado != "Pendiente"))
            {
                Log.Error($"ProcesadorPagarMercadoPago: Tarjeta: {comando.dto.NumeroDeTarjeta} Puesto: {comando.dto.PuestoDeTrabajoId} R: {comando.dto.RecorridoId} => Cobro ya efectuado para éste camión");
                resultado.Errores.Add("", Textos.Error_CobroYaEfectuado);
            }
        }

        private PagoConMercadoPago ObtenerPago(PagarMercadoPago comando)
        {
            var pago = Repositorio.ObtenerMayor<PagoConMercadoPago, int>(x => x.Recorrido.Id == comando.dto.RecorridoId && !x.Devuelto, x => x.Id);
            if (pago == null)
            {
                pago = Repositorio.Agregar(new PagoConMercadoPago
                {
                    Estado = "Pendiente",
                    Fecha = DateTime.Now,
                    Recorrido = Repositorio.Obtener<Recorrido>(x => x.Id == comando.dto.RecorridoId),
                    Token = comando.dto.Token
                });
                Repositorio.GuardarCambios();
            }
            if (pago.Estado == "Error")
            {
                //Si hubo un error en el Qr, genero una nueva idempotencia para procesar un nuevo cobro
                //Si hubo una excepción en el cobro, es decir pago "Pendiente", mantengo la idempotencia porque capaz se proceso ok en MP
                pago.Fecha = DateTime.Now != pago.Fecha ? DateTime.Now : pago.Fecha.AddSeconds(1);
                pago.Estado = "Pendiente";
                pago.Token = comando.dto.Token;
                Repositorio.GuardarCambios();
            }
            return pago;
        }

        private void ActualizarPago(EstadoPagoDto detalleDePago, PagoConMercadoPago pago, ResultadoPagarMercadoPago resultado)
        {
            if (detalleDePago != null && string.IsNullOrEmpty(detalleDePago.Error))
            {
                pago.MercadoPagoId = detalleDePago.Id;
                pago.MontoCobrado = Convert.ToDecimal(detalleDePago.MontoCobrado);
                pago.Estado = detalleDePago.Estado;
                pago.DetalleDelEstado = detalleDePago.DetalleDeEstado;
            }
            else
            {
                pago.Estado = "Error";
                pago.DetalleDelEstado = detalleDePago?.Error;
                resultado.Error("", detalleDePago.Error);
            }
            Repositorio.GuardarCambios();
        }

    }
}
