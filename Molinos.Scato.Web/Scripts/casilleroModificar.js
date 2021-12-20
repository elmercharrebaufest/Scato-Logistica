$(document).ready(function () {
    $(".numeroCasillero").mask("9999-999999");
    $("#Hasta").mask("9999-999999");

    if ($('#Masivo').val() == "true") {
        $(".hasta").show();
    } else {
        $(".hasta").hide();
    }
    
    $.validator.addMethod("hastaVacio", function (value, element) {
        if ($('#Hasta').val().length == 0 && $('#Masivo').val() == "true") {
            return false;
        }
        return true;
    }, $('#Hasta').data().errorVacio);
});