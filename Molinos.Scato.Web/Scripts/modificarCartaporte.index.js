jQuery(document).ready(function ($) {
    mostrarCamposFerroviario();
    if ($('#soloLectura').val() == "True") {
        $('input').attr("disabled", "disabled");
        $('select').attr("disabled", "disabled");
        $('#agregarVagon').attr("style", "visibility: hidden");
        $("#aceptar").attr("style", "visibility: hidden");
        $('textarea').attr("disabled", "disabled");
    } else {
        $('#NroCartaPorte').attr("readonly", "readonly");
        $('#CEE').attr("readonly", "readonly");
        $('#CTG').attr("readonly", "readonly");
        $('#FechaCP').attr("readonly", "readonly");
        $("#FechaCP").datepicker("destroy");
        $('#FechaVto').attr("readonly", "readonly");
        $("#FechaVto").datepicker("destroy");

        $('#tipoVehiculoDropdown').attr("readonly", "readonly");
        $('#tipoVehiculoDropdown').attr("disabled", "disabled");
        $('#Destino').attr("readonly", "readonly");
        $('#Variedad').attr("readonly", "readonly");

        //$('#Procedencia').attr("readonly", "readonly");
        $('#CodEstab').attr("readonly", "readonly");
        $('#OrigenVehiculo').attr("readonly", "readonly");
        $('#OrigenVehiculo').attr("readonly", "readonly");
        $('#Prestador').attr("readonly", "readonly");

        $('#AcuerdoMarco').attr("readonly", "readonly");
        $('#bocaDestino').attr("readonly", "readonly");
        $('#Caratula').attr("readonly", "readonly");

        $('#FechaEmision').attr("readonly", "readonly");
        $('#EsTransportista').val(true);
    }
});

function mostrarCamposFerroviario() {
    var tipoVehiculo = $('#tipoVehiculoDropdown :selected').text();
    var tipoVehiculoModel = $('#TipoVehiculoModel').val() === "1";

    if ((tipoVehiculo == "Tren" || tipoVehiculoModel) && $('#Cpe').is(':checked')) {
        showTextBoxCpe();
        $('.row-ramal-precinto').show();
    } else {
        $('.row-ramal-precinto').hide();
    }
    $('.row-sucursal-ctg').show();
}





