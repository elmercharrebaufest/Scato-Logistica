
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

var validarPatenteSingleton = true;
function validarPatente() {
    if (!$(".patente").is('[readonly="readonly"]') && validarPatenteSingleton) {
        if ($(".patente").val().toLowerCase().replace('_', '') != $(".patenteOriginal").val().toLowerCase()) {
            InhabilitarPantalla();
            if ($(".patente").val() != null && $(".patente").val() != '') {
                MostrarAlertaError($('.patente').data().invalida);
            }
        } else {
            validarPatenteSingleton = false;
            HabilitarPantalla();
        }
    }
}

$(document).ready(function () {
    
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

    $(document).on('click', '#botonCancelar', function () {
        BlockUI();
        return true;
    });
});
