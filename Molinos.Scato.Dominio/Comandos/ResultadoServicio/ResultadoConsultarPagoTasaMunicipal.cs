using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos
{
    public class ResultadoConsultarPagoTasaMunicipal : Resultado
    {
       public TipoAlerta TipoAlerta { get;  set; }
       public string MensajeAlerta { get;  set; } 
       public bool SeLevantaBarrera { get;  set; } 
       public int? IdPago { get;  set; } 
       public int? IdDiferenciaDePago { get;  set; } 
       public bool EsPagoAbonado { get;  set; }
       public bool EjecutaWorkFlow { get;  set; }
        public int IdPay { get; set; }
        public int IdPayComplemento { get; set; }
        public ResultadoConsultarPagoTasaMunicipal()
        {
            TipoAlerta = TipoAlerta.Error;
            MensajeAlerta = string.Empty;
            SeLevantaBarrera = true;
            IdPago = null;
            IdDiferenciaDePago = null;
            EsPagoAbonado = false;
            EjecutaWorkFlow = true;
        }
    }
}
