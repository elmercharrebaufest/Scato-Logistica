jQuery(document).ready(function ($) {
    var notificaLectura = $.connection.notificaLectura;

    notificaLectura.client.informarLectura = function (notificacion) {
        $("#validation-patente-alert").addClass("hide");
        $("#validation-patente-danger").addClass("hide");
        if (!notificacion.TarjetaValida && !notificacion.EsTarjetaSupervisor) {
            $("#validation-patente").html(notificacion.MensajeError);
            $("#validation-patente-alert").removeClass("hide");
            $("#Numero").val('');
            $("#PuestoDeTrabajoId").val(0);
        } else if (notificacion.EsTarjetaSupervisor) {
            $("#validation-danger").html(notificacion.MensajeError);
            $("#validation-patente-danger").removeClass("hide");
            $("#Numero").val('');
            $("#PuestoDeTrabajoId").val(0);
        } else if (notificacion.NumeroDeTarjeta !== null && notificacion.NumeroDeTarjeta !== "") {
            $("#Numero").val(notificacion.NumeroDeTarjeta);
        }
    };

    
    // Start the connection
    try {
        var puestosDeTrabajo = [];

        if ($("#puestosDeTrabajo").val() !== "") {
            puestosDeTrabajo = $.map(JSON.parse($("#puestosDeTrabajo").val()), function (item) { return item; });
        }

        $("#labelConectado").addClass('hidden');
        $("#labelDesconectado").addClass('hidden');
        $("#Numero").removeAttr('readonly');
        $("#Numero").attr("placeholder", "");

        if (puestosDeTrabajo.length != 0 && puestosDeTrabajo[0].Automatico) {
            BlockUI($("#cargando").val());
            $(".btn-primary").focus();
            window.hubReady.done(function () {
                $.each(puestosDeTrabajo, function (index, value) {
                    if (value.Automatico) {
                        notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), value.Id);
                    }
                });
                $.unblockUI();
            }).fail(function (error) {
                window.location.href = window.location.href;
            });
        }
    }
    catch (err) {
        window.location.href = window.location.href;
    }

});