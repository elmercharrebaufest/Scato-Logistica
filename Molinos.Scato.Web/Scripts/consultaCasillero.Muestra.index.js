$(document).ready(function () {
    DefinirAutocompletar('#Centro', '#CentroId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentroUnico);
    $("#NumeroDocumento").mask("9999-99999999");
    $("#Patente").mask("aaa999");

    $("#Patente").keyup(function() {
        $(this).text().toUpperCase();

    });
});