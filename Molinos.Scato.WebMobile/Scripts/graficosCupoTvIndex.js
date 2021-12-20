$(document).ready(function () {
    $(".navbar").remove();
    $("#panel").remove();

    //Actualizo los graficos cada un minuto.
    setInterval(actualizarGraficos, 30000);
});

function actualizarGraficos() {
    $.get(urlEstadoDePlanta, function (data) {
        $('#estadodePlanta').html(data);
    });
}