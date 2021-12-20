jQuery(document).ready(function($) {
    $('#FechaCarga').attr("readonly", "readonly");
    $('#PesoBrutoOrigen').val('');
    $('#PesoTaraOrigen').val('');
    $("#PesoBrutoOrigen, #PesoTaraOrigen").change(ActualizarPesoNeto);
    
    $.validator.addMethod("FechaEmisionValidacion", function (value, element) {
        if (Globalize.parseDate($('#FechaEmision').val()) > Globalize.parseDate($('#FechaCarga').val()))
            return false;
        return true;
    }, $('#FechaEmision').data().error);

    $.validator.addMethod("FechaVtoValidacion", function (value, element) {
        if (Globalize.parseDate($('#FechaCarga').val()) > Globalize.parseDate($('#FechaVencimiento').val()))
            return false;
        return true;
    }, $('#FechaVencimiento').data().error);

    $.validator.addMethod("FechaVtoValidacionMaxima", function (value, element) {
        var fechaActual = Globalize.parseDate($('#FechaCarga').val());
        fechaActual.setDate(fechaActual.getDate() + 61);
        if (fechaActual < Globalize.parseDate($('#FechaVencimiento').val()))
            return false;
        return true;
    }, $('#FechaVencimiento').data().errorMaxima);
    
    $.validator.addMethod("pesoBrutoNumerico", function (value, element) {
        return $.isNumeric(value);
    }, $('#PesoBrutoOrigen').data().errorNumerico);
    $.validator.addMethod("pesoTaraNumerico", function (value, element) {
        return $.isNumeric(value);
    }, $('#PesoTaraOrigen').data().errorNumerico);

    $.validator.addMethod("pesoNetoPositivo", function (value, element) {
        return $('#PesoNetoOrigen').val() >= 0;
    }, $('#PesoNetoOrigen').data().errorPositivo);
    $.validator.addMethod("pesoBrutoPositivo", function (value, element) {
        return $('#PesoBrutoOrigen').val() >= 0;;
    }, $('#PesoBrutoOrigen').data().errorPositivo);
    $.validator.addMethod("pesoTaraPositivo", function (value, element) {
        return $('#PesoTaraOrigen').val() >= 0;;
    }, $('#PesoTaraOrigen').data().errorPositivo);
    $(".pesoNetoMaximoEjecutar").change(
            function () {
                $('.pesoNetoMaximo').valid();
            }
        );

    $.validator.addMethod("pesoNetoMaximo", function (value, element) {
        var pesoNeto = $('#PesoNetoOrigen').val();
        if (pesoNeto > 0 && $('#PesoBrutoOrigen').val() > 0 && $('#PesoTaraOrigen').val() > 0) {
            var netoMax = $('#tipoVehiculoDropdown :selected').data('netomaximo');
            return netoMax == null || pesoNeto <= netoMax;
        } else {
            return true;
        }
    }, $('#PesoNetoOrigen').data().errorPesonetomaximo);

    if ($('#esIngreso').val() == "True") {
        $('#CentroDestino').attr("readonly", "readonly");
        $('#CentroDestino').autocomplete({ disabled: true });
    }
    
    $(".patente-internacional").mask("?*******", {placeholder: ""});
    $("#NroHojaDeRutaYerbatera").mask("9999-99999999");
    DefinirFechas();

    // para que el campo retome el foco al seleccionar una fecha
    $('input.date').datepicker("option", "onSelect", function () {
        $(this).focus();
    });
    

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;
    
    DefinirAutocompletarChofer();
    DefinirAutocompletarConSAP('#Proveedor', '#ProveedorId', '#autocompleteProv', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletar('#Procedencia', '#ProcedenciaId', $('#links').data().urlBuscarProcedencias, $('#links').data().urlBuscarProcedenciaUnica);
    DefinirAutocompletarConSAP('#Destinatario', '#DestinatarioId', '#autocompleteDest', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletar('#CentroDestino', '#CentroDestinoId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentroUnico);

    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    });
    
    $('#TipoComercialId').focus();
    cargarTiposVehiculo();
});

function DefinirFechas() {
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    $('.FechaEmision input').mask(formatoFecha);
    $('.FechaVencimiento input').mask(formatoFecha);
    $(".FechaEmision input").datepicker("option", "maxDate", $(".FechaCarga input").val());
    $(".FechaVencimiento input").datepicker("option", "minDate", $(".FechaCarga input").val());
    var fechaActual = Globalize.parseDate($('#FechaCarga').val());
    if (fechaActual != null) {
        fechaActual.setDate(fechaActual.getDate() + 61);
        $(".FechaVencimiento input").datepicker("option", "maxDate", fechaActual);
    }
}

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function ActualizarPesoNeto() {
    var a = $.isNumeric(Globalize.parseInt($("#PesoBrutoOrigen").val())) ? Globalize.parseInt($("#PesoBrutoOrigen").val()) : 0;
    var b = $.isNumeric(Globalize.parseInt($("#PesoTaraOrigen").val())) ? Globalize.parseInt($("#PesoTaraOrigen").val()) : 0;

    $("#PesoNetoOrigen").val(a - b);
    if (a > 0 && b > 0) {
        ValidarObjeto($("#orden-form"), $("#PesoNetoOrigen"));
    }
}

function cargarTiposVehiculo(bool) {
    $.getJSON($('#links').data().urlObtenertiposvehiculo, { conTren: bool },
            function (response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    options += "<option data-netoMaximo='" + response[i].netoMaximo + "'  data-brutoMaximo='" + response[i].brutoMaximoEgreso + "' value='" + response[i].tipoVehiculoValue + "'" + ">"
                        + response[i].tipoVehiculoText + "</option>";
                }
                $('#tipoVehiculoDropdown').html(options);
                $('#tipoVehiculoDropdown').val($('#TipoVehiculoInt').val());
            });
}