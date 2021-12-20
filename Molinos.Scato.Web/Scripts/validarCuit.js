$(document).ready(function () {
    $(function () {
        $("#Cuit").mask("99-99999999-9");
    });
    
    $.validator.addMethod("cuitValido", function cuitValidoF(cuit, element) {
        cuit = cuit.toString().replace(/[-_]/g, "");
        if (cuit.length != 11 && cuit.length != 0) {
            return false;
        }
    
        var acumulado   = 0;
        var digitos     = cuit.split("");
        var digito      = digitos.pop();
 
        for(var i = 0; i < digitos.length; i++) {
            acumulado += digitos[9 - i] * (2 + (i % 6));
        }
 
        var verif = 11 - (acumulado % 11);
        if(verif == 11) {
            verif = 0;
        } else if(verif == 10) {
            verif = 9;
        }
 
        return digito == verif || cuit.length == 0;
    }, $(".cuitValido:first").data().errorCuitInvalido);

});