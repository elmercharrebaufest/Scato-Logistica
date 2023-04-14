$(document).ready(function () {
    $('#CodigoDeEstablecimiento').focusout(function () {
        if ($('#CodigoDeEstablecimiento').val().length > 0) {
            $.getJSON($('#links').data().urlCodigoEstablecimientoEnRango, { codigoEstablecimiento: $('#CodigoDeEstablecimiento').val() }
            ).done(function (data) {
                if (data.flag == true) {
                    $('#EsSojaEPA').prop("checked", true);
                }
                else {
                    $('#EsSojaEPA').prop("checked", false);
                }
            })
        }
    }
    )
})