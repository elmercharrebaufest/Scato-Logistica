$(document).ready(function () {
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');

    $("#FechaDesde").datepicker({
        onSelect: function (selected) {
            cambiarFechaDesde($("#FechaDesde").val());
        }
    });
    $("#FechaHasta").datepicker({
        onSelect: function (selected) {
            cambiarFechaHasta($("#FechaHasta").val());
        }
    });

    $("#FechaHasta").change(function () {
        cambiarFechaHasta($("#FechaHasta").val());
    });

    $("#FechaDesde").change(function () {
        cambiarFechaDesde($("#FechaDesde").val());
    });

});//onSelect

function cambiarFechaDesde(selected) {
    
    $("#FechaHasta").datepicker("option", { "minDate": selected, /*"maxDate": max */ });

    var controlGroup = $("#FechaDesde").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length === 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}

function cambiarFechaHasta(valor) {
    
    $("#FechaDesde").datepicker("option", { /*"minDate": min, */"maxDate": valor });

    var controlGroup = $("#FechaHasta").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length === 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}


function AgregarDias(fecha, dias) {
    var theDate = new Date(fecha.split('/')[2], fecha.split('/')[1] - 1, fecha.split('/')[0]);
    theDate.setDate(theDate.getDate() + dias);
    var fechaStr = theDate.toLocaleDateString();
    return AgregarCeroDelante(fechaStr.split('/')[1]) + '/' + AgregarCeroDelante(fechaStr.split('/')[0]) + '/' + fechaStr.split('/')[2];
}

function AgregarCeroDelante(numero) {
    if (numero.toString().length === 3) {
        console.log("0" + numero);
        return "0" + numero.toString();
    } else {
        return numero;
    }
}