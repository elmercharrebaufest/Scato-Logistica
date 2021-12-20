$(document).ready(function() {
    /*Deshabilito el boton guardar del modal para no generar requests repetidos */
    if ($("#dialogo-editar-guardar").length > 0)
        $("#dialogo-editar-guardar").attr("disabled", true);

    /*Agrego clase de validación para Bootstrap*/
    $('span.field-validation-valid, span.field-validation-error').each(function() {
        $(this).addClass('help-block');
    });

    /*Agrego clase error si hay errores lado cliente*/
    $('form').submit(function() {
        $(this).find('div.control-group').each(function() {
            if ($(this).find('span.field-validation-error').length == 0) {
                $(this).removeClass('error');
            }
        });
        $(this).find('div.control-group').each(function() {
            if ($(this).find('span.field-validation-error').length > 0) {
                $(this).addClass('error');
            }
        });
    });

    /*Agrego clase error si hay errores lado servidor*/
    $(this).find('div.control-group').each(function() {
        if ($(this).find('span.field-validation-error').length == 0) {
            $(this).removeClass('error');
        }
    });
    $(this).find('div.control-group').each(function() {
        if ($(this).find('span.field-validation-error').length > 0) {
            $(this).addClass('error');
        }
    });

    /*Vierifico si los errores siguen persistiendo*/
    $('input, textarea, select').change(function() {
        var controlGroup = $(this).closest("div.control-group");
        if (controlGroup.find('span.field-validation-error').length == 0)
            controlGroup.removeClass('error');
        else
            controlGroup.addClass('error');
    });

    $('input, textarea').keyup(function () {
        var controlGroup = $(this).closest("div.control-group");
        if (controlGroup.find('span.field-validation-error').length == 0)
            controlGroup.removeClass('error');
        else
            controlGroup.addClass('error');
        //For Required validations
        if (!this.value && $(this).data().valRequired) {
            $(this).valid();
            controlGroup.addClass('error');
        }
    });

    $('input, textarea, select').blur(function () {
        var controlGroup = $(this).closest("div.control-group");
        if (controlGroup.find('span.field-validation-error').length == 0)
            controlGroup.removeClass('error');
        else
            controlGroup.addClass('error');
    });

    $('form').keypress(function (e) {
        if ((e.keyCode == 13) && (e.target.type != "textarea")) {
            e.preventDefault();
            $(this).submit();
        }
    });
    
    $('.campoNumerico, .campoNumericoCalado').keydown(function (event) {
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 || event.keyCode == 13) {
        } else {
            if (event.keyCode == 190 || event.keyCode == 188 || event.keyCode == 110) {
                $(this).val($(this).val() + Globalize.culture().numberFormat["."]);
                event.preventDefault();
            }
            else if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });
    
    $('.campoCantidad').keydown(function (event) {
        return (event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) ||
                    event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 ||
                    event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 ||
                    event.keyCode == 13;
    });
});

function ValidarObjeto(formulario, elemento) {
    formulario.validate().element(elemento);
    var controlGroup = elemento.closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0) {
        controlGroup.removeClass('error');
        return 1;
    }
    else {
        controlGroup.addClass('error');
        return 0;
    }       
}