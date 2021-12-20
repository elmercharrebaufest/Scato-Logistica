$(document).ready(function() {
    $(".numeroCasillero").mask("9999-999999");
    $("#Hasta").mask("9999-999999");
    $(".hasta").hide();

    if ($('.crearMasivo').is(':checked')) {
        $(".hasta").show();
    }

    $('.crearMasivo').click(function () {
        if ($('.crearMasivo').is(':checked')) {
            $(".hasta").show();
        } else {
            $(".hasta").hide();
        }
    });
    
    $.validator.addMethod("hastaVacio", function (value, element) {
        if ($('#Hasta').val().length == 0 && $('.crearMasivo').is(':checked')) {
            return false;
        }
        return true;
    }, $('#Hasta').data().errorVacio);
});