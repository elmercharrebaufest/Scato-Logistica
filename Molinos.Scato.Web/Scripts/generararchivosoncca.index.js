$(document).ready(function () {
    $.unblockUI();
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    $('#fechaInicio').mask(formatoFecha);
    $("#fechaInicio").datepicker();

    $('#fechaFin').mask(formatoFecha);
    $("#fechaFin").datepicker();

    $(document).on('click', '.centrocheck', function() {
        $("#oncca-form .alert").hide();
    });
});