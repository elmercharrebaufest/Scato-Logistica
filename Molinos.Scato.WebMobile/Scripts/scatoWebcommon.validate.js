$(document).ready(function() {
    $(".navbar-toggle").on('click', function()
    {
        if ($(this).hasClass("collapsed")) {
            $(this).find('i').addClass('glyphicon-chevron-up');
            $(this).find('i').removeClass('glyphicon-chevron-right');
        } else {
            $(this).find('i').addClass('glyphicon-chevron-right');
            $(this).find('i').removeClass('glyphicon-chevron-up');
        }
        return true;
    });
});

function ValidarObjeto(formulario, elemento) {
    formulario.validate().element(elemento);
    var controlGroup = elemento.closest("div.form-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}

function BloquearPantalla() {
    /*$.blockUI({
        blockMsgClass: 'blocuiBox',
        message: '<h5>' + cargandoGif() + 'Cargando</h5>'
    });*/
}

function cargandoGif() {
    return "<img src=\"" + loadingGift + "\"/>";
}


jQuery.validator.methods["date"] = function (value, element) { return true; }

function checkDate(selector) {
    var elemento = $(selector);
    var re = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
    if (elemento.val() === "" || !elemento.val().match(re)) {
        elemento.focus();
        return false;
    }
    return true;
}