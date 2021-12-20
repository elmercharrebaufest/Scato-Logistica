$(document).ready(function () {

    $(".FechaDesde input").datetimepicker({
        format: "dd/mm/yyyy hh:ii",
        autoclose: true,
        pickerPosition: "bottom-left",
    });
    $(".FechaHasta input").datetimepicker({
        format: "dd/mm/yyyy hh:ii",
        autoclose: true,
        pickerPosition: "bottom-left",
    });

    $(".FechaDesde input").change(function (selected) {
        cambiarFechaDesde($(".FechaDesde input").val());
    });

    $(".FechaHasta input").change(function (selected) {
        cambiarFechaHasta($(".FechaHasta input").val());
    });

    $(function () { $('#tipoCalada').bootstrapToggle() });

    $("#tipoCalada").change(function () {
        aceptaMotivo = false;

        if (cambioUsuario) {
            $("#MotivoModal").modal('show');
        }

        cambioUsuario = true;
    });

    $('#MotivoModal').on('hidden', function () {
        var tipoPinchazo = ObtenerTipoPinchazo();
        var motivo = $("#motivo").val();
        $("#motivo").val("");

        if (aceptaMotivo) {
            CambiarPinchazo(tipoPinchazo, motivo);
        } else {
            cambioUsuario = false;
            $('#tipoCalada').bootstrapToggle('toggle')
        }
    })


    $("#motivoAceptar").click(function () {
        var motivo = $("#motivo").val();
        if (motivo.length < 10 || motivo.length > 200) {
            alert("Por favor ingrese un motivo con longitud entre 10 y 200 caracteres");
        } else {
            aceptaMotivo = true;
            $("#MotivoModal").modal('hide');
        }
    });
});

function ObtenerTipoPinchazo() {
    return $('#tipoCalada').is(':checked') ? DosYTresPinchazos : UnoYDosPinchazos;
}

function CambiarPinchazo(tipoPinchazo, motivo) {
    $('#pinchazosPorCalada').block({
        message: '<div>Cargando...</div>',
        css: {
            'width': '150px',
            'height': '25px',
            'font-size': '100%',
            'font- family': 'Arial, Helvetica, sans- serif',
            'color': '#00000',
            'font-weight': 'bolder'
        },
        overlayCSS: { backgroundColor: 'transparent' }
    }); 

    $.ajax({
        url: urlCambiarPinchazos,
        type: 'GET',
        cache: false,
        data: { tipoPinchazo: tipoPinchazo, motivo: motivo }
    }).done(function (result) {
        $("#tipoCalada").prop('disabled', false);
        $("#pinchazosPorCalada").html(result);
    });
}

function cambiarFechaDesde(selected) {
    $(".FechaHasta input").datepicker("option", "minDate", selected);
    $(".FechaDesde input").valid();
    var controlGroup = $(".FechaDesde input").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}

function cambiarFechaHasta(valor) {
    $(".FechaDesde input").datepicker("option", "maxDate", valor);
    $(".FechaHasta input").valid();
    var controlGroup = $(".FechaHasta input").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}