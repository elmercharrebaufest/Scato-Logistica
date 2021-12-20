$(document).ready(function () {

    if (idMensaje > 0) {
        HabilitarCampos($("#activarMensaje").val("on"));
    }else {
        HabilitarCampos($("#activarMensaje").val("off"));
    }
    

    $(function () { $('#activarMensaje').bootstrapToggle() });

    $("#activarMensaje").change(function () {
        if (activarMensaje == false) {
            //$("#MensajeModal").modal('show');
            activarMensaje = true;
            HabilitarCampos($("#activarMensaje").val("on"));
            
        } else if (estadoActual != ' ' && estadoActual != 'Baja') {
            activarMensaje = false;
            try {
                $("#mensajeAceptar").click();
                //MostrarAlertaExitosa();
                LimpiarCampos();
                HabilitarCampos($("#activarMensaje").val("off"));
                idMensaje = 0;
            } catch (error) {
                MostrarAlertaError();
            }
        } else {
            if ($("#activarMensaje").val() == "on") {
                HabilitarCampos($("#activarMensaje").val("off"));
            } else {
                HabilitarCampos($("#activarMensaje").val("on"));
            }
        }
        cambioUsuario = true;
    });

    $("#mensajeAceptar").click(function () {
        var detalle = $("#notificacionDetalle").val();
        var titulo = $("#notificacionTitulo").val();
        if (activarMensaje && (detalle.length < 10 || detalle.length >1000)) {
            alert("Por favor ingrese un motivo con longitud mayor a 10 y menor a 1000 caracteres");
            return;
        } 

        try {
            aceptaMotivo = true;
            var tipoMensaje = ObtenerMensaje();
            CambiarMensaje(tipoMensaje, idMensaje, titulo, detalle);
        } catch (error) {
            MostrarAlertaError();
        }
    });
});

function ObtenerMensaje() {  
    if (activarMensaje) {
        return "Alta";
    }else {
        return "Baja";
    }
}

function HabilitarCampos(actMsj) {
    if (actMsj.val() == "on") {
        $("#notificacionFecha").attr("disabled", false);
        $("#notificacionTitulo").attr("disabled", false);
        $("#notificacionDetalle").attr("disabled", false);
        $("#mensajeAceptar").attr("disabled", false);
        activarMensaje = true;
    } else {
        $("#notificacionFecha").attr("disabled", true);
        $("#notificacionTitulo").attr("disabled", true);
        $("#notificacionDetalle").attr("disabled", true);
        $("#mensajeAceptar").attr("disabled", true);
        activarMensaje = false;
    }
}

function CambiarMensaje(tipoMensaje,idMensaje, titulo, detalle) {
    $('#notificacionActiva').block({
        message: '<div>Cargando...</div>',
        css: {
            'width': '150px',
            'height': '25px',
            'font-size': '100%',
            'font- family': 'Arial, Helvetica, sans- serif',
            'color': '#00000',
            'font-weight': 'bolder'
        },
        overlayCSS: { backgroundColor: 'transparent' }
    }); 

    $.ajax({
        url: urlActivarMensaje,
        type: 'GET',
        cache: false,
        data: { tipoMensaje: tipoMensaje,idMensaje: idMensaje,titulo: titulo , detalle: detalle }
    }).done(function (result) {
        $("#tipoMensaje").prop('disabled', false);
        $("#notificaiconActiva").html(result);
        MostrarAlertaExitosa();
        estadoActual = tipoMensaje;
        //console.log(result);
        //idMensaje = result.idMensaje;
    });
}

function LimpiarCampos() {
    //$("#notificacionFecha").val();
    $("#notificacionTitulo").val("");
    $("#notificacionDetalle").val("");
}
