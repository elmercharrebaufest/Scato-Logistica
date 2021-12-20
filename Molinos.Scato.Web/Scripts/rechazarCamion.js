$(document).ready(function() {
    $(document).on('click', '.rechazar-boton', function () {
        $.get(this.href, cargarDialogoRechazar);
        return false;
    });
    $(document).on('click', '.dialogo-rechazar-cerrar', function () {
        $("#dialogo-rechazar").modal('hide');
        $("#mensajeRechazar").html("");
        return false;
    });
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
}
