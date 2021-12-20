$(document).ready(function () {
    $("#PesoBruto, #PesoTara").change(ActualizarPesoNeto);
    
    $("#remitoNro").mask("9999-99999999");
    

    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    $.validator.addMethod("date", function (value, element) { return true; });
    $('.FechaFabricacion input').mask(formatoFecha);
    $(".FechaFabricacion input").datepicker({ maxDate: '0' });
    

    IniciarBalanza();

    $("#BalanzaId").change(ReiniciarBalanza);
    $("#tomarPeso").on('click', TomarPeso);
    
    $.unblockUI();
});

function ActualizarPesoNeto() {
    var a = $.isNumeric(Globalize.parseFloat($("#PesoBruto").val())) ? Globalize.parseFloat($("#PesoBruto").val()) : 0;
    var b = $.isNumeric(Globalize.parseFloat($("#PesoTara").val())) ? Globalize.parseFloat($("#PesoTara").val()) : 0;
    
    $("#PesoNeto").val(formatWithComma(a - b));
    if (a > 0 && b > 0) {
        ValidarObjeto($("#descargar-form"), $("#PesoNeto"));
    }
}

function IniciarBalanza() {
    if ($("#BalanzaId").val() > 0) {
        $("#tomarPeso").attr("disabled", true);
        $.getJSON($("#BalanzaId").data().balanzaUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            $("#BalanzaId").data().balanzaModalidad = data.Modalidad;
        }).complete(function () { $("#tomarPeso").attr("disabled", false); });
    }
}

function ReiniciarBalanza() {
    if ($("#BalanzaId").val() > 0) {
        $("#PesoBruto").val("");
        $('#PesoBruto').attr('readonly', 'readonly');
        ActualizarPesoNeto();
        var label = $("#tomarPeso").html();
        $("#tomarPeso").attr("disabled", true);
        $.getJSON($("#BalanzaId").data().balanzaUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            $("#BalanzaId").data().balanzaModalidad = data.Modalidad;
        }).complete(function () { $("#tomarPeso").html(label); $("#tomarPeso").attr("disabled", false); });
    }
}

function TomarPeso() {
    if ($("#BalanzaId").data().balanzaModalidad == 0) {
        $('#PesoBruto').removeAttr('readonly');
        $('#PesoBruto').focus();
    } else {
        $('#PesoBruto').attr('readonly', 'readonly');
        //Toma el peso desde el orquestador
        var label = $("#tomarPeso").html();
        $("#tomarPeso").html($("#tomarPeso").data().mensajeEsperar);
        $("#tomarPeso").attr("disabled", true);
        $.getJSON($("#PesoBruto").data().pesoUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            if ($.isNumeric(data)) {
                $("#PesoBruto").val(data);
                ActualizarPesos();
            } else { //Devolvió error
                MostrarAlertaError(data);
            }
        }).complete(function () { $("#tomarPeso").html(label); $("#tomarPeso").attr("disabled", false); });
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