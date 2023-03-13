function rechazo() {
    $("#modal-rechazo-mover").modal('hide');

    var href = urlVisteoRechazoMobile + "?"
    + "instanciaWorflow=" + $("#InstanciaWorflow").val()
    + "&workflowDefinicionId=" + $("#WorkflowDefinicionId").val();
    $.get(href, cargarDialogoRechazar);
}

function enCondiciones(confirmRechazar) {
    esValido = confirmRechazar;
    confirmacionVisteo();
}

function cargarDialogoRechazar(data) {

    $("#mensajeRechazar").html(data);
    $('#dialogo-rechazar').modal({
        keyboard: false,
        backdrop: 'static'
    });
}

function confirmacionVisteo() {
    $.ajax({
        url: urlVisteoMobile,
        data: {
            instanciaWorflow: $("#InstanciaWorflow").val(),
            workflowDefinicionId: $("#WorkflowDefinicionId").val(),
            esValido: esValido,
            actividad: $("#Actividad")[0] ? $("#Actividad").val() : '',
            actividadXaml: $("#ActividadXaml")[0] ? $("#ActividadXaml").val() : '',
            mensaje: $("#Mensaje")[0] ? $("#Mensaje").val() : '',
            comentario: $("#Comentario")[0] ? $("#Comentario").val() : '',
        },
        type: "POST",
        success: function (data) {
            if (data.EsValido) {
                $("#modal-rechazo-mover").modal('hide');
                MostrarAlertaExitosa();
            }
        },
        error: function () {
            $("#modal-rechazo-mover").modal('hide');
            MostrarAlertaError();
        },
        complete: function () {
            document.getElementById("modalConfirmarVisteo").close();
            $("#dialogo-rechazar").modal('hide');
        }
    });
}

