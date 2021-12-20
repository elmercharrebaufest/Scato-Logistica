$(document).ready(function () {
    $(document).on('click', '.lote-boton', function () {
        if ($("#NroLote").valid()) {
            $.get(this.href, {
                AlmacenId: $("#AlmacenId").val(),
                Centro: $("#Centro").val(),
                InstanciaWorkflow: $("#InstanciaWorkflow").val(),
                Kgs: $("#Kgs").val(),
                LoteObligatorio: $("#LoteObligatorio").val(),
                Material: $("#Material").val(),
                NroLote: $("#NroLote").val(),
                WorkflowDefinicionId: $("#WorkflowDefinicionId").val()
            }, cargarDialogoLote);
        } else {
            $("#NroLote").closest("div.control-group").addClass('error');
        }
        return false;
    });

    $(document).on('click', '.dialogo-lote-cerrar', function () {
        $("#dialogo-lote").modal('hide');
        $("#mensajeLote").html("");
        return false;
    });
});

function cargarDialogoLote(data) {
    if (data == "ERROR") {
        window.location = window.location;
        return false;
    }
    if (data == "OK") {
        window.location = $("#ListaDeCamionesUrl").val();
        return false;
    }
    $("#mensajeLote").html(data);
    $('#dialogo-lote').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-lote').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });

}