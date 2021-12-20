$(document).ready(function () {
    $(".formatoHora").mask("99:99");
    $("#btnTransmitir").click(function () {
        if ($("#transmisiones").val().length != 0) {
            BlockUI($("#transmisiones-form").data().mensajeEspera);
            //Elimino primer y último pipe que sobran
            var t = $("#transmisiones").val().substring(1, $("#transmisiones").val().length - 1);
            $.post($("#btnTransmitir").data().url, { transmisiones: t }, function (data) {
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
            });
        }
    });
    $("#btnNoTransmitir").click(function () {
        if ($("#transmisiones").val().length != 0) {
            //Elimino primer y último pipe que sobran
            var t = $("#transmisiones").val().substring(1, $("#transmisiones").val().length - 1);
            $.post($("#btnNoTransmitir").data().url, { transmisiones: t }, function (data) {
                $("#transmisiones-form").submit();
            });
        }
    });
    $("#btnRetransmitirTodas").click(function () {
            BlockUI($("#btnRetransmitirTodas").data().mensaje);
            $.removeCookie('Retrasmitidas', { path: '/' });
            var intervalo = ActualizarBloqueo();
            var t = $("#ids").val();            
            $.post($("#btnRetransmitirTodas").data().url, { listaIds: t }, function (data) {
                $("#transmisiones-form").submit();
            }).complete(function () {
                $.unblockUI();
                clearInterval(intervalo);             
            });
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

});

function ActualizarBloqueo() {
    return setInterval(function () {
        if ($.cookie('Retrasmitidas') != null) {
            $.unblockUI();
            BlockUI($("#btnRetransmitirTodas").data().mensaje +" "+ $.cookie('Retrasmitidas'));
            $.removeCookie('Retrasmitidas', { path: '/' });
        }
    }, 10);
}