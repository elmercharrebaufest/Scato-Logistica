$(document).on('click', '#btnConfirmarpendienteHB4', function () {
    document.getElementById("modalConfirmarPendienteHB4").close();
    BlockUI();
    $.ajax({
        type: 'POST',
        url: $('#ConfirmarPendienteHB4Url').val(),
        dataType: 'json',
        data: {
            workflowInstance: $('#InstanciaWorkflow').val(),
            workflowDefinicionId: $('#WorkflowDefinicionId').val()
        },
        success: function (response) {
            if (response.EsValido == true) {
                MostrarAlertaExitosa("Se proceso correctamente.");
                window.location.href = $('#Redirect').val();
            } else {
                MostrarRespuestaMensajes(response);
            }
        },
        error: function (error) {
        },
        complete: function () {
            $.unblockUI();  
        }
    });
});

function MostrarRespuestaMensajes(response) {
    response.Mensajes.forEach(function (item, index, array) {
        if (item.TipoDeMensaje === 2) {
            MostrarAlertaError(item.Mensaje);
        } else if (item.TipoDeMensaje === 1) {
            MostrarAlertaAdvertencia(item.Mensaje);
        }
    })
}


$(document).on('click', '.rechazar-boton', function () {
    BlockUI();
    $.get(this.href, cargarDialogoRechazar);
    return false;
});


$(document).on('click', '.dialogo-rechazar-cerrar', function () {
    $("#dialogo-rechazar").modal('hide');
    $("#mensajeRechazar").html("");
    return false;
});

function cargarDialogoRechazar(data) {
    $("#mensajeRechazar").html(data);

    $('#dialogo-rechazar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-rechazar').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $.unblockUI();
}

Mousetrap.stopCallback = function (e, element, combo) {
    return false;
};

Mousetrap.bind('alt+1', function () {
    $("#aceptar").click();
});
Mousetrap.bind('alt+2', function () {
    BlockUI();
    window.location.href = $("#botonCancelar").attr('href');
});
Mousetrap.bind('alt+3', function () {
    $(".rechazar-boton").click();
});