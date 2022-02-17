//import { notificarLectura } from "./inputmask/global/window";

jQuery(document).ready(function ($) {
    var notificaLectura = $.connection.notificaLectura;

    notificaLectura.client.informarLectura = function (notificacion) {
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
        }
    };
    //tenemos que poner en pantalla un punto verde si hay conexion y un punto rojo sino, 
    //informa la conexion al lector
    notificaLectura.client.informarEstadoConexion = function (notificacion) {
        if (notificacion.Estado) {
            $("#labelConectado").addClass('hidden');
            $("#labelDesconectado").removeClass('hidden');
        } else {
            $("#labelConectado").removeClass('hidden');
            $("#labelDesconectado").addClass('hidden');
        }
    };

    BlockUI($("#cargando").val());
    // Start the connection
    try {
        var puestoDeTrabajo = null;
        var puestoDeTrabajoEf = null;
        if ($("#puestoDeTrabajo").val() !== "") {
            puestoDeTrabajo = $("#puestoDeTrabajo").val();
        }
        if ($("#puestoDeTrabajoEf").val() !== "") {
            puestoDeTrabajoEf = $("#puestoDeTrabajoEf").val();
        }
        console.log(puestoDeTrabajo);
        console.log(puestoDeTrabajoEf);

        window.hubReady.done(function () {
            if (puestoDeTrabajo != null) {
                notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), puestoDeTrabajo);
                notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), puestoDeTrabajoEf);
            }
            $.unblockUI();
        }).fail(function (error) {
            window.location.href = window.location.href;
        });
    }
    catch (err) {
        window.location.href = window.location.href;
    }
    //////////////

    $(".btn").prop('disabled', true);
    //$("#btnCobrarEnEfectivo").click(function () {
    //    avanzarDeEtapa();
    //});

    $("#btnCancelar").click(function () {
        limpiarVentana();
    });

    escucharCambiosTextBoxTokenQr();
    ProcesarPago();
    $("#spinnerEsperandoLecturaQr").css('visibility', 'hidden');
});

function traerDatosCP(numTarjeta, puesto) {

    $.ajax({
        url: $("#links").data().urlDatos,
        dataType: 'json',
        data: { numeroDeTarjeta: numTarjeta, puestodetrabajoId : puesto },
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

                if (data.MedioDePago == 0) {
                    avanzarDeEtapa();
                    return;
                }

                cambiarMensajeDeEstado("esperandoQr");
            }
        }
    }).fail(function () {
        mostrarAlertaPorPantalla(3, "Error de conexión");
    });
}

procesando = false;
qrCompleto = null;
function escucharCambiosTextBoxTokenQr() {
    
    $('#tokenDePago').on('propertychange input', function (e) {
        if ($("#tokenDePago").val().length < 36 || procesando) {
            return;
        }
        qrCompleto = Date.now();
    });
}

function ProcesarPago() {
    setInterval(function ()
    {
        if (procesando) {
            return;
        }
        var fechaActual = Date.now() - 1000;
        
        if (qrCompleto == null || qrCompleto > fechaActual) {
            return;
        } 
        qrCompleto = null;

        if ($("#numTarjeta").val() == "" || $("#numTarjeta").val() == "esperando...") {

            mostrarAlertaPorPantalla(3, "No hay tarjeta leída");
            limpiarVentana();
        } else {

            cambiarMensajeDeEstado("procesandoPago");
            procesando = true;
            $('#tokenDePago').prop("disabled", true);
            $.ajax({
                url: $("#links").data().urlPago,
                dataType: 'json',
                data: {
                    Token: $("#tokenDePago").val(),
                    NumeroDeTarjeta: $("#numTarjeta").val(),
                    Monto: $("#montoCobrado").val(),
                    RecorridoId: $("#RecorridoId").val(),
                    PuestoDeTrabajoId: $("#puestoDeTrabajo").val(),
                    NombreGarita: $("#garita").val()
                },
                type: "POST",
                success: function (data) {

                    if (data.HayErrores) {

                        cambiarMensajeDeEstado("errorDePago");

                        var todosLosErrores = Object.keys(data.Errores).map(function (key) { return data.Errores[key]; });

                        let mensajeDeError = "";
                        todosLosErrores.forEach(function (e) { mensajeDeError = mensajeDeError + " \n " + e });

                        var obj = $('#MensajeEstadoDePago').text("Error al Procesar el cobro con MercadoPago: " + mensajeDeError);
                        obj.html(obj.html().replace(/\n/g, '<br/>'));
                        setTimeout(function () { cambiarMensajeDeEstado("esperandoQr"); }, 10000);

                    } else {
                        let idMercadoPago = data.DetalleDePago.Id;
                        mostrarAlertaPorPantalla(0, "Cobro efectuado Exitosamente. " + "\n \n N° Patente: " + $("#patente").val() + "\n \n  -  Id de Pago: " + idMercadoPago);

                        limpiarVentana();
                    }
                },
                complete: function (data) {
                    procesando = false;
                    $('#tokenDePago').prop("disabled", false);
                    $('#tokenDePago').focus();
                }
            }).fail(function () {
                mostrarAlertaPorPantalla(2, "Error de conexión");
            });
        }
    }, 500);
}

function avanzarDeEtapa() {

    $.ajax({
        url: $("#links").data().urlPagoconfectivo,
        dataType: 'json',
        data: {
            Token: $("#tokenDePago").val(),
            NumeroDeTarjeta: $("#numTarjeta").val(),
            Monto: $("#montoCobrado").val(),
            RecorridoId: $("#RecorridoId").val(),
            PuestoDeTrabajoId: $("#puestoDeTrabajo").val()
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


function cambiarMensajeDeEstado(estadoDelPago) {

    switch (estadoDelPago) {
        case "esperandoQr":

            $('#MensajeEstadoDePago').text("Por favor realice la lectura del código QR");
            $(".loader").css({ "border-top": "5px solid green", "animation": "spin 1.5s ease-in-out infinite" });
            $(".loader").hide();
            $(".loader").show();
            $("#MensajeEstadoDePago").css('color', 'green');
            $(".loader").css('visibility', 'visible');
            $("#spinnerEsperandoLecturaQr").css('visibility', 'visible');
            $(".btn").prop('disabled', false);
            $("#tokenDePago").focus();
            break;

        case "procesandoPago":

            $('#MensajeEstadoDePago').text("Procesando el cobro...");
            $(".loader").css({ "border-top": "5px solid blue", "animation": "spin 1s ease infinite" });
            $(".loader").hide();
            $(".loader").show();
            $("#MensajeEstadoDePago").css('color', 'blue');
            $(".loader").css('visibility', 'visible');
            $("#spinnerEsperandoLecturaQr").css('visibility', 'visible');
            $(".btn").prop('disabled', true);
            break;

        case "errorDePago":

            $(".loader").css({ "border-top": "5px solid red", "animation": "spin 2s linear infinite" });
            $(".loader").hide();
            $(".loader").show();
            $("#MensajeEstadoDePago").css('color', 'red');
            
            $(".loader").css('visibility', 'hidden');
            $("#spinnerEsperandoLecturaQr").css('visibility', 'visible');
            $(".btn").prop('disabled', false);
            $("#tokenDePago").val(null);
            $("#tokenDePago").focus();
            break;
    }
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
    $(".btn").prop('disabled', true);
}

