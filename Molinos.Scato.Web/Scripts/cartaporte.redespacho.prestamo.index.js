jQuery(document).ready(function ($) {
    $('#Prestador').addClass('PrestadorRequerido');
    $('#PagadorFlete').addClass('PagadorFleteRequerido');
    $('#Transportista').addClass('TransportistaRequerido');
    $('#TarifaTonelada').addClass('TarifaToneladaRequerido');

    


    $.validator.addMethod("PrestadorRequerido", function (value, element) {
        return value.length > 0;
    }, $('#Prestador').data().error);

    $.validator.addMethod("PagadorFleteRequerido", function (value, element) {
        return value.length > 0;
    }, $('#PagadorFlete').data().errorRequerido);

    $.validator.addMethod("TransportistaRequerido", function (value, element) {
        return value.length > 0;
    }, $('#Transportista').data().errorRequerido);

    $.validator.addMethod("TarifaToneladaRequerido", function (value, element) {
        return value.length > 0;
    }, $('#TarifaTonelada').data().errorRequerido);


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