jQuery(document).ready(function ($) {
    $(".patente-internacional").mask("?*******", {placeholder: ""});

    DefinirAutocompletar('#Transportista', '#TransportistaId', $('#links').data().urlBuscarTransportistaUnico, $('#links').data().urlBuscarTransportista);
    $("#Transportista").autocomplete("option", "appendTo", "#dialogo-editar");
    
    if ($('#mensajeInvalido').data() != null && $('#mensajeInvalido').data().invalido != null) {
        $.validator.addMethod("valorDebeSerValido", function (value, element) {
            return ($('#Transportista').val().length == 0 || $("#TransportistaId").val() != 0);
        }, $('#mensajeInvalido').data().invalido);
    }
});