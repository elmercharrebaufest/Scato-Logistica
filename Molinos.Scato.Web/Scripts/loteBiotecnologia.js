jQuery(document).ready(function ($) {

    if ($('#MaterialId').size() > 0) {
        DefinirAutocompletar('#Material', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    }


    DesbloquearBoton();
});

function DesbloquearBoton() {
    setInterval(function () {
        if ($.cookie('RetornoExportacion') != null) {
            $.unblockUI();
            $.removeCookie('RetornoExportacion', { path: '/' });
        }
    }, 1000);
}
