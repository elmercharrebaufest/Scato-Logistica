using Molinos.Scato.Dominio.Comandos.ResultadoServicio;
using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio
{
    public class PagoTasaMunicipalBuilder : IPagoTasaMunicipalBuilder
    {
        public ResultadoConsultarPagoTasaMunicipal Resultado { get; private set; }
        private TipoValidacionPagoTasaMunicipal CondicionPago { get; set; }

        public PagoTasaMunicipalBuilder()
        {
            Resultado = new ResultadoConsultarPagoTasaMunicipal();
        }

        public void ExcepcionDePago()
        {
            CondicionPago = TipoValidacionPagoTasaMunicipal.Abonado24Hrs;
        }

        public void AsignarError(string error, string errorDescripcion)
        {
            Resultado.Errores.Add(error, errorDescripcion);
        }

        public void AsignarValidacionDePago(TipoValidacionPagoTasaMunicipal condicionPago)
        {
            CondicionPago = condicionPago;
        }

        public void AsignarIdDePago(int idPago)
        {
            Resultado.IdPago = idPago;
        }

        public void AsignarIdDiferenciaDePago(int idDiferenciaPago)
        {
            Resultado.IdDiferenciaDePago = idDiferenciaPago;
        }

        public void AsignarIdPay(int idPay)
        {
            Resultado.IdPay = idPay;
        }

        public void AsignarIdPayComplemento(int idPayComplemento)
        {
            Resultado.IdPayComplemento = idPayComplemento;
        }

        public TipoValidacionPagoTasaMunicipal ObtenerCondicionDePago()
        {
            return CondicionPago;
        }

        public ResultadoConsultarPagoTasaMunicipal ConstruirResultado()
        {
            // Use local variables to avoid repeated property access
            var resultado = Resultado;
            switch (CondicionPago)
            {
                case TipoValidacionPagoTasaMunicipal.Abonado:
                    resultado.TipoAlerta = TipoAlerta.Exito;
                    resultado.MensajeAlerta = "TASA ABONADA";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = true;
                    resultado.EjecutaWorkFlow = true;
                    break;

                case TipoValidacionPagoTasaMunicipal.Adeudado:
                    resultado.TipoAlerta = TipoAlerta.Error;
                    resultado.MensajeAlerta = "TASA ADEUDADA";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = false;
                    resultado.EjecutaWorkFlow = true;
                    break;

                case TipoValidacionPagoTasaMunicipal.DiferenciaDePago:
                    resultado.TipoAlerta = TipoAlerta.Error;
                    resultado.MensajeAlerta = "EXISTEN DIFERENCIAS EN EL PAGO";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = false;
                    resultado.EjecutaWorkFlow = false;
                    break;

                case TipoValidacionPagoTasaMunicipal.Abonado24Hrs:
                    resultado.TipoAlerta = TipoAlerta.Exito;
                    resultado.MensajeAlerta = "TASA ABONADA 24 HRS";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = true;
                    resultado.EjecutaWorkFlow = true;
                    break;

                default:
                    resultado.TipoAlerta = TipoAlerta.Error;
                    resultado.MensajeAlerta = "TASA ADEUDADA";
                    resultado.SeLevantaBarrera = true;
                    resultado.EsPagoAbonado = false;

                    break;
            }
            return resultado;
        }
    }
}