$(document).ready(function () {
    $('#modalidad-editada').change(function() {
        if ($('#modalidad-editada').val() != $('#modalidadInicial').val()) {
            $('#motivo').removeAttr('disabled');
        } else {
            $('#motivo').val("");
            $('#motivo').attr('disabled', 'disabled');
        }
    });
});