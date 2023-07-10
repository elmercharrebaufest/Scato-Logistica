jQuery(document).ready(function ($) {
    $('#PagadorFlete').addClass('PagadorFleteRequerido');
    $('#Transportista').addClass('TransportistaRequerido');
    $('#TarifaTonelada').addClass('TarifaToneladaRequerido');

    $.validator.addMethod("PagadorFleteRequerido", function (value, element) {
        return value.length > 0;
    }, $('#PagadorFlete').data().errorRequerido);

    $.validator.addMethod("TransportistaRequerido", function (value, element) {
        return value.length > 0;
    }, $('#Transportista').data().errorRequerido);

    $.validator.addMethod("TarifaToneladaRequerido", function (value, element) {
        return value.length > 0;
    }, $('#TarifaTonelada').data().errorRequerido);
});