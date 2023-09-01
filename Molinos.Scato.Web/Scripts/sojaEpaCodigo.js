$(document).ready(function () {

    setEPARequired();

    $('#CodigoDeEstablecimiento').focusout(function () {
        changeEPARequired();
    });

    $("#form-establecimiento").on("submit", function () {
        $("#EsSojaEPA").removeAttr('disabled');
    })

})

function changeEPARequired() {

    var min = parseInt($('#rangoMinEPA').val());
    var max = parseInt($('#rangoMaxEPA').val());

    if ($('#CodigoDeEstablecimiento').val() >= min
        && $('#CodigoDeEstablecimiento').val() <= max) {
        $('#EsSojaEPA').prop("checked", true);
        $("#EsSojaEPA").attr('disabled', 'disabled');
    } else {
        $('#EsSojaEPA').prop("checked", false);
        $("#EsSojaEPA").removeAttr('disabled');
    }
}

function setEPARequired() {

    var min = parseInt($('#rangoMinEPA').val());
    var max = parseInt($('#rangoMaxEPA').val());

    if ($('#CodigoDeEstablecimiento').val() >= min
        && $('#CodigoDeEstablecimiento').val() <= max
        && $('#EsSojaEPA').prop("checked")) {
        $("#EsSojaEPA").attr('disabled', 'disabled');
    }
}