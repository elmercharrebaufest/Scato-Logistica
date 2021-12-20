jQuery(document).ready(function ($) {
    $("input:text,form").attr("autocomplete", "off");

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletar('#EntregadorDesc', '#EntregadorId', $('#links').data().urlBuscarEntregadores, $('#links').data().urlBuscarEntregador, null, null, null, 6);
    DefinirAutocompletarConSAP('#ProveedorDesc', '#ProveedorId', '#autocompleteCorr', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, true, false, false);
    DefinirAutocompletarConSAP('#CorredorDesc', '#CorredorId', '#autocompleteCorr', listarProveedores, obtenerProveedor, obtenerProveedorSap, null, null, false, true, false);
    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');

    //$('.FechaDesdeEgreso input').mask(formatoFecha);
    //$('.FechaHastaEgreso input').mask(formatoFecha);

    $('#NivelDeDetalleId').click(function () {
        if ($('#NivelDeDetalleId').val() == '4') {
            $('#BocaDestinoId').removeAttr('disabled');
        } 
        else {
            $('#BocaDestinoId').val("0");
            $('#BocaDestinoId').attr("disabled", true);
        }
        
    });
    $('.FechaDesde input').datetimepicker({
        format: 'dd/mm/yyyy hh:ii',
        autoclose: true,
        pickerPosition: "bottom-left"
    });

    $('.FechaDesdeEgreso input').datetimepicker({
        format: 'dd/mm/yyyy hh:ii',
        autoclose: true,
        pickerPosition: "bottom-left"
    });

    //$(".FechaDesdeEgreso input").datepicker({
    //    onSelect: function (selected) {
    //        cambiarFechaDesde(selected);
    //    }
    //});

    $('.FechaHastaEgreso input').datetimepicker({
        format: 'dd/mm/yyyy hh:ii',
        autoclose: true,
        pickerPosition: "bottom-left"
    });

    $('.FechaHasta input').datetimepicker({
        format: 'dd/mm/yyyy hh:ii',
        autoclose: true,
        pickerPosition: "bottom-left"
    });

    //$(".FechaHastaEgreso input").datepicker({
    //    onSelect: function (selected) {
    //        cambiarFechaHasta(selected);
    //    }
    //});

    $(".FechaHastaEgreso input").change(function (selected) {
        cambiarFechaHasta($(".FechaHastaEgreso input").val());
    });

    $(".FechaDesdeEgreso input").change(function (selected) {
        cambiarFechaDesde($(".FechaDesdeEgreso input").val());
    });

    if ($('#mensajeInvalido').data() != null && $('#mensajeInvalido').data().invalido != null) {
        $.validator.addMethod("valorDebeSerValido", function (value, element) {
            return ($("#MaterialId").val() != 0);
        }, $('#mensajeInvalido').data().invalido);
    }
    
    $("#CentroId").change(function () {
        var centroId = $("#CentroId").val();

        $.getJSON($('#links').data().urlObtenerFiltros, { centroId: centroId },
            function (response) {
                
                var options = '';
                for (var i = 0; i < response.tipoComercial.length; i++) {
                    options += "<option value='" + response.tipoComercial[i].Value + "'" + ">"
                        + response.tipoComercial[i].Text + "</option>";
                }
                $('#TipoComercialId').html(options);

                options = '';
                for (i = 0; i < response.almacenOrigen.length; i++) {
                    options += "<option value='" + response.almacenOrigen[i].Value + "'" + ">"
                        + response.almacenOrigen[i].Text + "</option>";
                }
                $('#AlmacenOrigenId').html(options);

                options = '';
                for (i = 0; i < response.almacenOrigen.length; i++) {
                    options += "<option value='" + response.almacenOrigen[i].Value + "'" + ">"
                        + response.almacenOrigen[i].Text + "</option>";
                }
                $('#AlmacenDestinoId').html(options);

                options = '';
                for (i = 0; i < response.workflows.length; i++) {
                    options += "<option value='" + response.workflows[i].Value + "'" + ">"
                        + response.workflows[i].Text + "</option>";
                }
                $('#WorkflowId').html(options);

                options = '';
                for (i = 0; i < response.balanzas.length; i++) {
                    options += "<option value='" + response.balanzas[i].Value + "'" + ">"
                        + response.balanzas[i].Text + "</option>";
                }
                $('#BalanzaBrutoId').html(options);

                options = '';
                for (i = 0; i < response.balanzas.length; i++) {
                    options += "<option value='" + response.balanzas[i].Value + "'" + ">"
                        + response.balanzas[i].Text + "</option>";
                }
                $('#BalanzaTaraId').html(options);

                options = '';
                for (i = 0; i < response.materiales.length; i++) {
                    options += "<option value='" + response.materiales[i].Value + "'" + ">"
                        + response.materiales[i].Text + "</option>";
                }
                $('#MaterialId').html(options);
            });

    })
});

function cambiarFechaDesde(selected) {
    $(".FechaHastaEgreso input").datepicker("option", "minDate", selected);
    $(".FechaDesdeEgreso input").valid();
    var controlGroup = $(".FechaDesdeEgreso input").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}

function cambiarFechaHasta(valor) {
    $(".FechaDesdeEgreso input").datepicker("option", "maxDate", valor);
    $(".FechaHastaEgreso input").valid();
    var controlGroup = $(".FechaHastaEgreso input").closest("div.control-group");
    if (controlGroup.find('span.field-validation-error').length == 0)
        controlGroup.removeClass('error');
    else
        controlGroup.addClass('error');
}

