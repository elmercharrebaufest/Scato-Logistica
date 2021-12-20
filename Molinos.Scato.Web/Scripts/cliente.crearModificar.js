$(document).ready(function () {
    $("#Cuit").change(function () {
        $("#NumeroDeDocumento").val($("#Cuit").val().split('-')[1]);
    });
    $("#Cuit").mask("99-99999999-9");
});