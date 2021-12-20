$(document).ready(function () {
    $("textarea").resizable({
        handles: "se"
    });
    
    $("#btnGenerarTransmision").click(function (e) {
        e.preventDefault();
        var datos;
        datos = $("#DocumentoPatente").val().replace(/(\r\n|\n|\r)/gm, ";");
        BlockUI();
        $.getJSON($("#btnGenerarTransmision").attr('href') + "/" + "ProcesarTransmision", { datos: datos, tipo: $("#funcionSeleccionada").val() }, function (data) {
            MostrarAlertaAdvertencia(data.Mensaje.join("</br>"));
        }).complete(function () {
            $.unblockUI();
        });
    });


});
