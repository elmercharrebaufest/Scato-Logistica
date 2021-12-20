$(document).ready(function () {
    $.validator.addMethod("cuilValido", function cuilValidoF(cuil, element) {
        cuil = cuil.toString().replace(/[-_]/g, "");
        if (cuil.length != 11 && cuil.length != 0) {
            return false;
        }
    
        var acumulado   = 0;
        var digitos     = cuil.split("");
        var digito      = digitos.pop();

        var primerDigito = digitos[0];
        var primerDigitoExtranjero = digitos[1];
        var segundodigitoExtranjero = cuil[cuil.length - 1];

        for(var i = 0; i < digitos.length; i++) {
            acumulado += digitos[9 - i] * (2 + (i % 6));
        }
 
        var verif = 11 - (acumulado % 11);
        if(verif == 11) {
            verif = 0;
        } else if(verif == 10) {
            verif = 9;
        }
        return digito == verif || cuil.length == 0 || (primerDigito == 9 && segundodigitoExtranjero == 9 && primerDigitoExtranjero == 9);
    }, $(".cuilValido:first").data().errorCuilInvalido);

});