$(document).ready(function () {

    setEPARequired();

    $('#CodigoDeEstablecimiento').focusout(function () {
        setEPARequired();
    });

    $("#form-establecimiento").on("submit", function () {
        $("#EsSojaEPA").removeAttr('disabled');
    })

    function setEPARequired() {
        if ($('#CodigoDeEstablecimiento').val() >= $('#rangoMinEPA').val()
            && $('#CodigoDeEstablecimiento').val() <= $('#rangoMaxEPA').val()) {
            $('#EsSojaEPA').prop("checked", true);
            $("#EsSojaEPA").attr('disabled', 'disabled');
        } else {
            $('#EsSojaEPA').prop("checked", false);
            $("#EsSojaEPA").removeAttr('disabled');
        }
    }
})
