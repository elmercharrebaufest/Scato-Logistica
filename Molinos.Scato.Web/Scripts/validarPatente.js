$(document).ready(function () {
    $(".control[data-toggle='tooltip']").each(function () {
        $(this).tooltip({ trigger: 'manual' });
    });

    $('.patente-argentina').change(function (e) {
        var valor = $(this).val();
        if (valor.length > 0 && !patenteArgentinaValida(valor)) {
            $(this).closest('.controls').addClass('active');
            $(this).tooltip('show');
            $(this).closest('.control-group').addClass('warning');
        } else {
            $(this).closest('.control-group').removeClass("warning");
            $(this).tooltip('hide');
        }
        return true;
    });

});
function isDigit(sChar) {
    var sCod = sChar.charCodeAt(0);
    return ((sCod > 47) && (sCod < 58));
}

function isAlpha(sChar) {
    var sCod = sChar.charCodeAt(0);
    var sRes = ((sCod > 64) && (sCod < 91));
    var sRes = sRes || ((sCod > 96) && (sCod < 123));
    return sRes;
}

function patenteArgentinaValida(valor) {
    var bRes = true;
    if (valor.length == 6) {
        bRes = bRes && isAlpha(valor.substr(0, 1));
        bRes = bRes && isAlpha(valor.substr(1, 1));
        bRes = bRes && isAlpha(valor.substr(2, 1));
        bRes = bRes && isDigit(valor.substr(3, 1));
        bRes = bRes && isDigit(valor.substr(4, 1));
        bRes = bRes && isDigit(valor.substr(5, 1));
        if (!bRes)
            return false;
    } else return false;
    return true;
}