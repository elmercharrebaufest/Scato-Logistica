$(document).ready(function() {
    $('.nunca').prop('checked', false);

    if ($('#cantidadCaracteristicas').val() == "0") {
        $('#enviar').attr('disabled', 'disabled');
    }
    
    $('form').off('keypress');

    $('form').keypress(function (event) {
        if (event.which == 13) {
            $("#enviar").click();
            return false;
        }
        return true;
    });
});