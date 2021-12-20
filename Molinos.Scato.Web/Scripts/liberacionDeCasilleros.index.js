$(document).ready(function () {
    DefinirAutocompletar('#Centro', '#CentroId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentroUnico);
    $("#Desde").mask("9999-999999");
    $("#Hasta").mask("9999-999999");
    $("#DiasDeAntiguedad").mask("9?99");
    $("#muestras").val("|");
    $("#ImprimirMuestrasAEliminarDiv").hide();
    $(".patente-internacional").mask("?*******", { placeholder: "" });
    
    $("#btnLiberarCasilleros").click(function () {
        $("#dialogo-confirmar-body").text($(this).data().mensaje);
        $("#dialogo-confirmar").modal('show');
    });
    
    $('#dialogo-confirmar-confirmar').click(function () {
        if ($("#muestras").val().length != 0) {
            var t = $("#muestras").val().substring(1, $("#muestras").val().length - 1);
            $.post($("#dialogo-confirmar-confirmar").data().url, { muestras: t }, function (data) {
                $("#liberacionDeCasilleros-form").submit();
                $("#muestras").val("|");
            });
        }
        $("#dialogo-confirmar").modal('hide');
    });
    
});