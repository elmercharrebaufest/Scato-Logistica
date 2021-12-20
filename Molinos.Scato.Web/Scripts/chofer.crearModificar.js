$(document).ready(function () {
    $("#Cuil").change(function () {
        $("#NumeroDeDocumento").val($("#Cuil").val().split('-')[1]);
    });
    $("#Cuil").mask("99-99999999-9");
});