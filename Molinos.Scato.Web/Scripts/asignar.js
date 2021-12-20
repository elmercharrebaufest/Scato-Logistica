$(document).ready(function () {
    if ($('#balanzasObligatorias').val() == "True") {
        $.validator.addMethod("balanzaTara",
            function (value, element) {
            return value.length > 0;
        }, $('#BalanzaTaraId').data().errorRequerido);
        $.validator.addMethod("balanzaBruto", function(value, element) {
            return value.length > 0;
        }, $('#BalanzaBrutoId').data().errorRequerido);
        $('#BalanzaTaraId').attr("disabled", false);
        $('#BalanzaBrutoId').attr("disabled", false);
        
    } else {
        $('#BalanzaTaraId').attr("disabled", true);
        $('#BalanzaBrutoId').attr("disabled", true);
    }

    $('.validarcheck').on('change', function () {
        var controlGroup = $('.validarcheck').closest("div.control-group");
        if ($(".validarcheck:checked").length == 0 && controlGroup.find('span.field-validation-valid').length == 1) {
            controlGroup.addClass('error');
            var validation2 = controlGroup.find('span.field-validation-valid');
            validation2.removeClass("field-validation-valid");
            validation2.addClass("field-validation-error");
            validation2.html(controlGroup.data().requerido);
        } else if ($(".validarcheck:checked").length != 0 && controlGroup.find('span.field-validation-error').length == 1) {
            controlGroup.removeClass('error');
            var validation = controlGroup.find('span.field-validation-error');
            validation.removeClass("field-validation-error");
            validation.addClass("field-validation-valid");
            validation.html("");
        }
    });

});

function ValidarObjetoPC(formulario, elemento) {
    formulario.validate().element(elemento);
    var controlGroup = elemento.closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}