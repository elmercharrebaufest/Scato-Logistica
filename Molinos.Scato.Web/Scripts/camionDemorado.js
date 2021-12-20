$(document).ready(function () {

    $('#EsTransportista').val(true);

    $("#rechazar").click(function () {

        BlockUI("");
        $.post($('#RechazoCamion').val(), { id: $('#Id').val() },
            function (data) {
                if (data == "OK") {
                    window.location = $('#Index').val();
                } else {
                    $.unblockUI();
                    MostrarAlertaError(data);
                }
            });
    });

    if ($("#FechaCP").val() == "31/01/1980") $("#FechaCP").val("");
    if ($("#FechaVto").val() == "31/01/2100") $("#FechaVto").val("");
    if ($("#Cosecha").val() == "99-99") $("#Cosecha").val("");
    if ($("#PesoBrutoOrigen").val() == "99999") $("#PesoBrutoOrigen").val("");
    if ($("#PesoTaraOrigen").val() == "99999") $("#PesoTaraOrigen").val("");
    if ($("#PesoNetoOrigen").val() == "99999") $("#PesoNetoOrigen").val("");
    if ($("#KmRecorrer").val() == "9999") $("#KmRecorrer").val("");
    if ($("#TarifaReferencia").val() == "9999999999,00") $("#TarifaReferencia").val("");
});