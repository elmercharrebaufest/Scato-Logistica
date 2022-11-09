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
            console.log(error)
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