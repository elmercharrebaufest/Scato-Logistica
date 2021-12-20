$(document).ready(function () {
    $(".formatoHora").mask("99:99");
    $(".transmitir").click(function () {
        var btn = $(this);
        if ($("#transmisiones").val().length != 0) {
            BlockUI($("#transmisiones-form").data().mensajeEspera);
            //Elimino primer y último pipe que sobran
            var t = $("#transmisiones").val().substring(1, $("#transmisiones").val().length - 1);
            $.post(btn.data().url, { transmisiones: t }, function (data) {
                $("#transmisiones-form").submit();
                if (data == "OK") {
                    MostrarAlertaExitosa();
                }else if (data[0] == 'W') {
                    MostrarAlertaAdvertencia(data.split("-")[1]);
                }else if (data[0] == 'E') {
                    MostrarAlertaError(data.split("-")[1]);
                }
            }).complete(function() {
                $.unblockUI();
                $("#transmisiones").val("|");
                $("#cantidad").text("0");
            });
        }
    });
    //Inicializo las transmisiones
    $("#transmisiones").val("|");
    $("#cantidad").text("0");
    $("#btnBuscar").click(function() {
        $("#transmisiones").val("|");
        $("#cantidad").text("0");
    });

    $.validator.addMethod("date", function (value, element) {
        return Globalize.parseDate($('.FechaDesde input:first').val()) <= Globalize.parseDate(value) || $('.FechaHasta input:first').val() == "" || $('.FechaDesde input:first').val() == "";
    }, $('.FechaHasta').data().secondDateValidation);

    var select = document.getElementById('EstadoTransmisionCTG');
    if (!select) {
        select = document.getElementById('EstadoTransmisionASap');
    }
    for (var i = 0, length = select.options.length; i < length; i++) {
        if (select.options[i] && (select.options[i].value === 'Cancelada' || select.options[i].value === 'Pendiente')) {
            select.options[i] = null;
        }
    }
});