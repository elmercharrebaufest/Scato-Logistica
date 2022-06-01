$(document).ready(function () {
    if ($("#verReintentar").val() == "True" && $("#postDeManual").val() == "True") {
        $("#Automatica").addClass("hide");
    } else if ($("#verReintentar").val() == "True") {
        $("#Manual").addClass("hide");
    } else {
        $("#Automatica").addClass("hide");
        $("#boton-volver").addClass("hide");
    }

    $("#boton-manual").on('click', function () {
        $("#Manual").removeClass("hide");
        $("#Automatica").addClass("hide");
        $("#CodigoDeBaja").focus();
        return false;
    });
    $("#boton-volver").on('click', function () {
        $("#Manual").addClass("hide");
        $("#Automatica").removeClass("hide");
        return false;
    });

    if ($("#modalidadPantalla").val() == "AltaCTG") {
        $("#CodigoCTG").change(function () {
            ValidarCtgCpe()
        });

        $("#NroOrden").change(function () {
            ValidarCtgCpe()
        });

        $("#Sucursal").change(function () {
            ValidarCtgCpe()
        });
    }
});


function ValidarCtgCpe() {
    var url = $('#links').data().validarCtgCpeUrl;
    var ctg = $("#CodigoCTG").val();
    var cpe = $("#NroOrden").val();
    var sucursal = $("#Sucursal").val();
    $("#dialogo-dardealta").prop('disabled', false);
    if (ctg != "" || (cpe != "" && sucursal != "")) {
        $.getJSON(url, { ctg: ctg, cpe: cpe, sucursal: sucursal }, function (response) {
            if (!response.EsValido) {
                var mensajes = [];
                response.Mensajes.forEach(function (item, index, array) {
                    mensajes.push(item.Mensaje);
                })
                MostrarAlertaError(mensajes.join("<br>"))
                $("#dialogo-dardealta").prop('disabled', true);
            }
        });
    }
}