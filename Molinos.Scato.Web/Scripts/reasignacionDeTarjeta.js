jQuery(document).ready(function() {
    $("#NroTarjetaRfidNueva").mask("9999999999");
    $('#NroTarjetaRfidAsignada').attr('readonly', true);
    $('.btn').attr('disabled', 'disabled');
    $('#NumeroDocumentoIngreso').focusout(function () {
        if ($('#NumeroDocumentoIngreso').val() != "") {
            ObtenerTarjetaRfid();
        }
    });
    if ($("#NroTarjetaRfidAsignada").val().length > 0) {
        $("#NumeroDocumentoIngreso").attr('readonly', true);
        $('#TipoDocumentoIngreso').attr('readonly', true);
        $('.btn').removeAttr('disabled');
    }
    $("#btnImprimir").click(ImprimirTarjetaDeAcceso);
});

function ImprimirTarjetaDeAcceso() {
    BlockUI($("#MensajeImprimiendo").val());
    $.getJSON($("#btnImprimir").data().imprimirUrl, { numero: $('#NroTarjetaRfidNueva').val()}, function (data) {
        if (data.error) {
            MostrarAlertaError(data.error);
        }
        else if (data.mensaje) {
            MostrarAlertaExitosa(data.mensaje);
        }
    }).complete(function () {
        $.unblockUI();
    });
}

function ObtenerTarjetaRfid() {
    BlockUI($("#MensajeBuscandoDatos").val());
    $.getJSON($("#NumeroDocumentoIngreso").data().numeroUrl, { tipoDocumentoIngreso: $('#TipoDocumentoIngreso').val(), nroDocumentoIngreso: $('#NumeroDocumentoIngreso').val() }, function (data) {
        if (data.tarjetaRfid == -1) {
            $('.btn').attr('disabled', 'disabled');
            MostrarAlertaError(data.error);
        }
        else {
            $('#NroTarjetaRfidAsignada').val(data.tarjetaRfid);
            $("#NumeroDocumentoIngreso").attr('readonly', true);
            $('#TipoDocumentoIngreso').attr('readonly', true);
            $('.btn').removeAttr('disabled');
        }
    }).complete(function () {
        $.unblockUI();
    });
}









