$(document).ready(function () {
    $("#PesoTara").change(ActualizarPesos);
    $("#remitoNro").mask("9999-99999999");
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    $.validator.addMethod("date", function (value, element) { return true; });
    $('.FechaFabricacion input').mask(formatoFecha);
    $(".FechaFabricacion input").datepicker({ maxDate: '0' });
    $.unblockUI();
    ObtenerPeso();
    $("#MaterialId").on('change', ObtenerPeso);
    $("#Unidades").on('change', ActualizarPesos);




});

function ActualizarPesos() {
    var neto = $.isNumeric(Globalize.parseFloat($("#PesoMaterial").val())) && $.isNumeric(Globalize.parseFloat($("#Unidades").val())) ? Globalize.parseFloat($("#PesoMaterial").val()) * Globalize.parseFloat($("#Unidades").val()) : 0;
    var tara = $.isNumeric(Globalize.parseFloat($("#PesoTara").val())) ? Globalize.parseFloat($("#PesoTara").val()) : 0;
    $("#PesoNeto").val(formatWithComma(neto));
    $("#PesoBruto").val(formatWithComma(neto + tara));
    if (neto > 0 && tara > 0) {
        ValidarObjeto($("#descargar-form"), $("#PesoBruto"));
    }
}

// Formatting Functions
function formatWithComma(x, precision) {
    if (!$.isNumeric(x))
        return "";
    var options = {
        precision: precision || 2,
        seperator: Globalize.cultures[Globalize.cultureSelector].numberFormat["."]
    };
    var formatted = parseFloat(x, 10).toFixed(options.precision);
    var regex = new RegExp(
            '^([^\\w]?\\d+)[^\\d](\\d{' + options.precision + '})$');
    formatted = formatted.replace(
        regex, '$1' + options.seperator + '$2');
    return formatted;
};


function ObtenerPeso() {
    $.getJSON($("#MaterialId").data().pesoUrl, { materialId: $("#MaterialId").val() }, function (data) {
        if ($.isNumeric(data)) {
            $("#PesoMaterial").val(data);
            ActualizarPesos();
        } else { //Devolvió error
            MostrarAlertaError(data);
        }
    });
}