var notificaLectura = null;
var procesando = false;

jQuery(document).ready(function ($) {
    notificaLectura = $.connection.notificaLectura;

    notificaLectura.client.informarLecturaPagoTasaMunicipal = function (notificacion) {
        $("#validation-patente-alert").addClass("hide");
        $("#validation-patente-danger").addClass("hide");
        if (!notificacion.TarjetaValida && !notificacion.EsTarjetaSupervisor) {
            $("#validation-patente").html(notificacion.MensajeError);
            $("#validation-patente-alert").removeClass("hide");
            $(".btn").prop('disabled', true);
        } else if (notificacion.EsTarjetaSupervisor) {
            $("#validation-danger").html(notificacion.MensajeError);
            $("#validation-patente-danger").removeClass("hide");
            $(".btn").prop('disabled', true);
        } else if (notificacion.NumeroDeTarjeta !== null && notificacion.NumeroDeTarjeta !== "") {
            limpiarVentana();
            $("#numTarjeta").val(notificacion.NumeroDeTarjeta);
            traerDatosCP(notificacion.NumeroDeTarjeta, notificacion.PuestoDeTrabajoId);
            var alerta = notificacion.TipoAlerta === 0 ? 'alert alert-success' : 'alert alert-danger';
            mostrarAlerta(notificacion.MensajeAlerta, alerta);
            if (notificacion.TipoAlerta === 2 && !notificacion.Rechazado) {
                $('#pagoManual').modal('show');
            }
        }
    };

    $(document).on('keydown', function (e) {
        if ($('#pagoManual').is(':visible') && e.key === 'Enter') {
            e.preventDefault();
            if ($('#btnCancelarPagoMunicipal').is(':focus') || $('.close').is(':focus'))
                $('#btnCancelarPagoMunicipal').click();
            else
                $('#btnAceptarPagoMunicipal').click();
        }
    });

    $.connection.hub.disconnected(function () {
        console.log("Conexión perdida, intentando reconectar...");
        setTimeout(function () {
            iniciarConexionSignalR();
        }, 5000);
    });

    iniciarConexionSignalR();

    $("#btnCancelar").click(function () {
        limpiarVentana();
    });

    $("#guardar").click(function () {
        avanzarDeEtapa();
    });
});

function cerrarModal() {
    $('#pagoManual').modal('hide');
}

function mostrarModal() {
    $('#pagoManual').modal('show');
}

function limpiarVentana() {
    $("#numTarjeta").val("esperando...");
    $("#patente").val("esperando...");
    $("#numCartaPorte").val("esperando...");
    $("#transportista").val("esperando...");
    $("#montoCobrado").val(null);
    $("#RecorridoId").val(null);
    $("#tokenDePago").val(null);
    $('#MensajeEstadoDePago').text(null);
    $(".loader").css('visibility', 'hidden');
    $("#spinnerEsperandoLecturaQr").css('visibility', 'hidden');
    $("#alertaError").hide();
    $("btn").prop('disabled', true);
    $('#pagoManual').modal('hide');
    ocultarAlerta();
}

function mostrarAlerta(mensaje, clase) {
    $('#alertaPago').attr('class', clase);
    $("#alertaPago").text(mensaje).show();
}

function ocultarAlerta() {
    $("#alertaPago").hide();
}

function traerDatosCP(numTarjeta, puesto) {
    $.ajax({
        url: $("#links").data().urlDatos,
        dataType: 'json',
        data: { numeroDeTarjeta: numTarjeta, puestodetrabajoId: puesto },
        type: "GET",
        success: function (data) {
            if (data.HayErrores === true) {
                limpiarVentana();
                mostrarAlertaPorPantalla(2, data.Error.Value);
            } else {
                $("#patente").val(data.Patente);
                $("#numCartaPorte").val(data.NumeroDocumentoIngreso);
                $("#transportista").val(data.NombreTransportista);
                $("#medioDePago").val(data.MedioDePago);
                $("#montoCobrado").val(data.Monto.replace(".", ","));
                $("#RecorridoId").val(data.RecorridoId);
            }
        }
    }).fail(function () {
        mostrarAlertaPorPantalla(3, "Error de conexión");
    });
}

function avanzarDeEtapa() {
    $.ajax({
        url: $("#links").data().urlPagoconfectivo,
        dataType: 'json',
        data: {
            NumeroDeTarjeta: $("#numTarjeta").val(),
            PuestoDeTrabajoId: $("#puestoDeTrabajoEf").val()
        },
        type: "POST",
        success: function (data) {
            if (data.HayErrores) {
                var todosLosErrores = Object.keys(data.Errores).map(function (key) { return data.Errores[key]; });
                let mensajeDeError = "";
                todosLosErrores.forEach(function (e) { mensajeDeError = mensajeDeError + " \n " + e });
                mostrarAlertaPorPantalla(2, "\n Error al Imprimir Ticket de pago: " + mensajeDeError);
            } else {
                mostrarAlertaPorPantalla(3, "Cobro realizado en efectivo, se va a imprimir el recibo municipal.");
            }
            limpiarVentana();
        }
    }).fail(function () {
        mostrarAlertaPorPantalla(3, "Error de conexión");
    });
}

function iniciarConexionSignalR() {
    BlockUI($("#cargando").val());
    $.connection.hub.start()
        .done(function () {
            var puestoDeTrabajo = $("#puestoDeTrabajo").val();
            var puestoDeTrabajoEf = $("#puestoDeTrabajoEf").val();
            if (puestoDeTrabajo) {
                notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), puestoDeTrabajo);
            }
            if (puestoDeTrabajoEf) {
                notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), puestoDeTrabajoEf);
            }
            $.unblockUI();
        })
        .fail(function () {
            setTimeout(iniciarConexionSignalR, 5000);
        });
}