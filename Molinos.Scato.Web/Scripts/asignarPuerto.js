
$(document).ready(function () {
    
    $('#PuntoDeCargaId').addClass("PuntoDeCargaIdRequerido");
    $.validator.addMethod("PuntoDeCargaIdRequerido", function (value, element) {
        return value.length > 0 && parseInt(value) > 0;
    }, $('#PuntoDeCargaId').data().errorRequerido);

});

function ValidarObjetoPC(formulario, elemento) {
    formulario.validate().element(elemento);
    var controlGroup = elemento.closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}