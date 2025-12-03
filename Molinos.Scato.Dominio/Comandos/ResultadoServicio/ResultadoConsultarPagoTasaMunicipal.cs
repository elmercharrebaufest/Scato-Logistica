using Molinos.Scato.Dominio.Enums;

namespace Molinos.Scato.Dominio.Comandos.ResultadoServicio
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
       public int IdExcepcion { get; set; }
       public bool TieneConfiguracionDeBloqueoDeIngreso { get; set; }
       public bool TieneDiferenciaDePago { get; set; }  
       public bool TieneExcepcion { get; set; }
       public string MotivoExcepcion { get; set; }
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
