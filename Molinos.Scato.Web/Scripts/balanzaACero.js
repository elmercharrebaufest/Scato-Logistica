var activo;
$(document).ready(function () {
    $(document).on('click', "#dialogo-BalanzaACero-dialogoCerear", cargarDialogoCereo);
    $(document).on('click', "#dialogo-Avanzar-dialogoAvanzar", cargarDialogoAvanzar);
    $(document).on('click', "#dialogo-cerear-cerear", CerearBalanza);
    $(document).on('click', "#dialogo-avanzar-avanzar", AvanzarBalanza);
    activo = true;
    if ($("#Modalidad").val() == 1) {
        $('#dialogo-BalanzaACero-balanzaPeso').text($('#dialogo-BalanzaACero-balanzaPeso').data().obteniendoPeso);
        TomarPesoRecursivo();
    }
    $(document).on('click', "#dialogo-BalanzaACero-enCero", BalanzaEnCero);

    $('#dialogo-Cerear').on('hide', function () {
        if ($("#Modalidad").val() == 1) {
            $('#dialogo-BalanzaACero-balanzaPeso').text($('#dialogo-BalanzaACero-balanzaPeso').data().obteniendoPeso);
            activo = true;
            TomarPesoRecursivo();
        }
    });

});

function cargarDialogoCereo() {
    activo = false;
    $('#dialogo-Cerear').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-Cerear').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $("#dialogo-Cerear").find('.btn:first').focus();
}
function TomarPesoRecursivo() {
    $.getJSON($("#TomarPesoUrl").val(), { balanzaId: $("#BalanzaId").val() }, function (data) {
        var peso = data;
        if ($.isNumeric(data)) {
            data += " Kg";
        }
        if (activo) {
            $('#dialogo-BalanzaACero-balanzaPeso').text(data);
        }
        if ($.isNumeric(peso) && peso == 0 && activo) {
            BalanzaEnCero();
        }
    }).complete(function () {
        if (activo) {
            setTimeout(TomarPesoRecursivo, 2000);
        }
    });
}
function BalanzaEnCero() {
    BlockUI($("#form-balanzaACero").data().mensajeEspera);
    $.ajax({
        url: $("#BalanzaEnCeroUrl").val(),
        dataType: 'json',
        async: false,
        data: { balanzaId: $("#BalanzaId").val(), instanciaWorkflow: $("#InstanciaWorkflow").val(), workflowDefinicionId: $("#WorkflowDefinicionId").val() },
        complete: function (data) {
            if (data != null && data.responseJSON.data != 'true') {
                MostrarAlertaError(data.responseJSON.data);
                $.unblockUI();
            } else {
                MostrarAlertaExitosa();
                activo = false;
                window.location = $("#ListaDeCamionesUrl").val();
            }
        }
    });
}
function CerearBalanza() {
    BlockUI($("#form-balanzaACero").data().mensajeEspera);
    $.getJSON($("#CerearBalanzaUrl").val(), { balanzaId: $("#BalanzaId").val(), instanciaWorkflow: $("#InstanciaWorkflow").val(), workflowDefinicionId: $("#WorkflowDefinicionId").val() }).complete(function (data) {
        if (data != null && data.responseJSON.data != 'true') {
            $.unblockUI();
            MostrarAlertaError(data.responseJSON.data);
            TomarPesoRecursivo();
            activo = true;
            $('#dialogo-Cerear').modal('toggle');

        } else {
            MostrarAlertaExitosa();
            $('#dialogo-Cerear').modal('toggle');
            window.location = $("#ListaDeCamionesUrl").val();
        }
    });
}

function cargarDialogoAvanzar() {
    activo = false;
    $('#dialogo-Avanzar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-Avanzar').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $("#dialogo-Avanzar").find('.btn:first').focus();
    $('#dialogo-Avanzar').on('hidden.bs.modal', function () {
        $('.modal-body').find('textarea,input').val('');
    });
}

function AvanzarBalanza() {
    BlockUI($("#form-balanzaACero").data().mensajeEspera);
    $.getJSON($("#AvanzarBalanzaUrl").val(), { balanzaId: $("#BalanzaId").val(), comentario: $("#Comentario").val(), instanciaWorkflow: $("#InstanciaWorkflow").val(), workflowDefinicionId: $("#WorkflowDefinicionId").val() }).complete(function (data) {
        if (data != null && data.responseJSON.data != 'true') {
            $.unblockUI();
            MostrarAlertaError(data.responseJSON.data);

        } else {
            MostrarAlertaExitosa();
            $('#dialogo-Avanzar').modal('toggle');
            window.location = $("#ListaDeCamionesUrl").val();
        }
    });
}