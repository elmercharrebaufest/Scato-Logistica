$(document).ready(function () {

    $("#botonaceptar").on('click', function () {
        $("#botonaceptar").attr("disabled", "disabled");
    });

    DesbloquearBoton();
});

function DesbloquearBoton() {
    setInterval(function () {
        if ($.cookie('RetornoExportacion') != null) {
            $("#botonaceptar").removeAttr("disabled");
            $.removeCookie('RetornoExportacion', { path: '/' });
        }
    }, 1000);
}