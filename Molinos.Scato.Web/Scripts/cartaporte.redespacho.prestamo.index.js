jQuery(document).ready(function ($) {
    $('#Prestador').addClass('PrestadorRequerido');
    $.validator.addMethod("PrestadorRequerido", function (value, element) {
        return value.length > 0;
    }, $('#Prestador').data().error);

    $('#Cpe').change(function () {
        if ($('#Cpe').is(':checked')) {
            $('#NroCartaPorte').prop('required', false);
            $('#NroCartaPorte').rules('remove', 'required');
            $('#Sucursal').prop('required', false);
            $('#Sucursal').rules('remove', 'required');
        } else {
            $('#NroCartaPorte').prop('required', true);
            $('#NroCartaPorte').rules('add', 'required');
            $('#Sucursal').prop('required', true);
            $('#Sucursal').rules('add', 'required');
        }
    });
});