$(document).ready(function () {
    $("#Contrato").select();
    $("#btnAceptar").click(function () {
        if ($("#Contrato").valid()) {
            BlockUI($("form.causaBlock").data().mensajeEspera);
            $.get(this.href, {
                InstanciaWorkflow: $("#InstanciaWorkflow").val(),
                Contrato: $("#Contrato").val(),
                WorkflowDefinicionId: $("#WorkflowDefinicionId").val()
            }, cargarDialogoContrato).complete($.unblockUI);
        } else {
            $("#Contrato").closest("div.control-group").addClass('error');
        }
        return false;
    });
    
    $("form,#Contrato").keyup(function (e) {
        if ((e.keyCode || e.which) == 13) { //Enter keycode
            $("#btnAceptar").click();
            return false;
        }
        return true;
    });
    
    $(document).on('click', '.dialogo-contrato-cerrar', function () {
        $("#dialogo-contrato").modal('hide');
        $("#mensajeContrato").html("");
        return false;
    });
});

function cargarDialogoContrato(data) {
    if (data == "ERROR") {
        window.location = window.location;
        return false;
    }
    if (data == "OK") {
        window.location = $("#ListaDeCamionesUrl").val();
        return false;
    }
    $("#mensajeContrato").html(data);
    $('#dialogo-contrato').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-contrato').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });

}

function VolverListaDeCamiones() {
    window.location = $('#ListaDeCamionesUrl').val();
}