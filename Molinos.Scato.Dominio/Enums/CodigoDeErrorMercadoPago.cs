using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.Scato.Dominio.Enums
{
    public enum CodigoDeErrorMercadoPago
    {
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError401003")]
        ErrorToken = 401003,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError400029")]
        TokenRequerido = 400029,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError2001")]
        ErrorMismoRequest = 2001,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError400061")]
        ErrorMismoUsuario = 400061,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError4037")]
        ErrorDeMonto = 4037,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError400017")]
        MontoRequerido = 400017,

        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError4037")]
        MontoInvalido = 400018,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError4037")]
        ErrorMonto = 400080,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError4037")]
        FormatoMontoInvalido = 400021,

        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError500")]
        ErrorServidorMP = 500900,
        [Display(ResourceType = typeof(Textos), Name = "MpCodigoError500")]
        ErrorServidor = 500901,

        [Display(ResourceType = typeof(Textos), Name = "MpErrorInsufficientMoney")]
        insufficient_money,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcInsufficientAmount")]
        cc_rejected_insufficient_amount,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcHighRisk")]
        cc_rejected_high_risk,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcInvalidCardId")]
        cc_rejected_invalid_card_id,

        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcbadFilledCardNumber")]
        cc_rejected_bad_filled_card_number,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcBadFilledDate")]
        cc_rejected_bad_filled_date,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcBadFilledOther")]
        cc_rejected_bad_filled_other,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcBadFilledSecurityCode")]
        cc_rejected_bad_filled_security_code,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcBlacklist")]
        cc_rejected_blacklist,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcCallForAuthorize")]
        cc_rejected_call_for_authorize,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcCardDisabled")]
        cc_rejected_card_disabled,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcCardError")]
        cc_rejected_card_error,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcDuplicatedPayment")]
        cc_rejected_duplicated_payment,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcInvalidInstallments")]
        cc_rejected_invalid_installments,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcMaxAttempts")]
        cc_rejected_max_attempts,
        [Display(ResourceType = typeof(Textos), Name = "MpErrorCcOtherReason")]
        cc_rejected_other_reason,
    }
}
