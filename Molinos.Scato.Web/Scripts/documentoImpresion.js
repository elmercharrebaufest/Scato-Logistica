$(document).ready(function () {
    $("#AplicaTodosLosCentros").attr("Disabled", true);
    $("#FormatoDeImpresionId").change(function() {
        if ($("#FormatoDeImpresionId").val() == "") {
            $("#AplicaTodosLosCentros").attr("Disabled", true);
        } else {
            $("#AplicaTodosLosCentros").attr("Disabled", false);
        }
    });

});