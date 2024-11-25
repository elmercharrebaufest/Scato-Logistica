$(document).ready(function () {
    $('#CodigoDeEstablecimiento').focusout(function () {
        changeEPARequired();
    });
})

function changeEPARequired() {

    var min = parseInt($('#rangoMinEPA').val());
    var max = parseInt($('#rangoMaxEPA').val());

    if ($('#CodigoDeEstablecimiento').val() >= min
        && $('#CodigoDeEstablecimiento').val() <= max) {
        $('#EsSojaEPA').prop("checked", true);
    } else {
        $('#EsSojaEPA').prop("checked", false);
    }
}
