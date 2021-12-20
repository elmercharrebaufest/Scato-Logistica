jQuery(document).ready(function ($) {
    //$('#esPdf').val(false);
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, borrarDescripcionCorta, borrarDescripcionCorta);
    if ($('#MaterialDescripcionCortaId').length != 0) {
        DefinirAutocompletar('#MaterialDescripcionCorta', '#MaterialDescripcionCortaId', $('#links').data().urlBuscarMaterialesdesc, $('#links').data().urlBuscarMaterialdesc, borrarMaterial, borrarMaterial);
    }

    $(window).keydown(function (event) {
        if (event.keyCode == 13) {
            event.target.click();
            event.preventDefault();
            return false;
        }
    });
    
    $.validator.addMethod("requerido", function (value, element) {
        return (($('#MaterialDescripcionCortaId').length == 0 || $('#MaterialDescripcionCortaId').val() == '0') && $('#MaterialId').val() != '0') || ($('#MaterialDescripcionCortaId').length != 0 && $('#MaterialDescripcionCortaId').val() != '0' && $('#MaterialId').val() == '0');
    }, "");
    
    DesbloquearBoton();
});

function borrarDescripcionCorta() {
    $('#MaterialDescripcionCorta').val('').trigger('focusout');
    $('#MaterialDescripcionCortaId').val('0');
    if ($('#MaterialDescripcionCortaId').length != 0) {
        ValidarObjeto($("form"), $("#MaterialDescripcionCorta"));
    }
    ValidarObjeto($("form"), $("#MaterialDesc"));
}

function borrarMaterial() {
    $('#MaterialDesc').val('').trigger('focusout');
    $('#MaterialId').val('0');
    if ($('#MaterialDescripcionCortaId').length != 0) {
        ValidarObjeto($("form"), $("#MaterialDescripcionCorta"));
    }
    ValidarObjeto($("form"), $("#MaterialDesc"));
}

function DesbloquearBoton() {
    setInterval(function () {
        if ($.cookie('RetornoExportacion') != null) {
            $.unblockUI();
            $.removeCookie('RetornoExportacion', { path: '/' });
            $('.validation-summary-errors').html('');
        }
    }, 1000);
}