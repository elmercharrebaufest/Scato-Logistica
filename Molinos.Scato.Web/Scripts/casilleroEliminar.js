$(document).ready(function () {
    $(".numeroCasillero").mask("9999-999999");
    $("#Hasta").mask("9999-999999");

    $('#dialogo-editar-guardar').val($('.numeroCasillero').data().textoEliminar);

    $.validator.addMethod("hastaVacio", function (value, element) {
        if ($('#Hasta').val().length == 0) {
            return false;
        }
        return true;
    }, $('#Hasta').data().errorVacio);
});