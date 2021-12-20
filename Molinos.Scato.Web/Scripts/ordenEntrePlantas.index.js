jQuery(document).ready(function ($) {
    //Máscaras
    //if ($("#Chofer_Cuil").val().length == 0)
    //    $("#Chofer_Cuil").mask("99-99999999-9");
    $(".patente-internacional").mask("?*******", {placeholder: ""});
    $('#Fecha').attr("readonly", "readonly");
    $("#Fecha").datepicker("destroy");
    // para que el campo retome el foco al seleccionar una fecha
    $('input.date').datepicker("option", "onSelect", function () {
        $(this).focus();
    });

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletarChofer();
    DefinirAutocompletar('#CentroDestino', '#CentroDestinoId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentroUnico);
    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    });
    if ($('#esIngreso').val() == "True") {
        $.validator.addMethod("anexoRequerido", function(value, element) {
            return value.length > 0;
        }, $('#mensajeAnexo').data().errorRequerido);
        MostrarAnexo();
        $('#MaterialId').change(function() {
            MostrarAnexo();
        });
    }
    $('#PatenteCamion').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado').change(ValidarPatenteCnrt);
    //$('#KmRecorrer').mask('?9999');
});

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function MostrarAnexo(codigoAnexo) {
    if ($('#divanexo').data() != null) {
        var resultado = jQuery.inArray($('#MaterialId').val(), $('#divanexo').data().materialesAnexos);

        if (resultado == -1) {
            $('#RequiereAnexoInase').val(false);
            $('#divanexo').addClass('hide');
            $('#CodigoAnexo').val('');
            $('#CodigoAnexo').removeClass('anexoRequerido');
        } else {
            $('#RequiereAnexoInase').val(true);
            $('#divanexo').removeClass('hide');
            $('#CodigoAnexo').addClass('anexoRequerido');
            if (codigoAnexo != null) $('#CodigoAnexo').val(codigoAnexo);
        }
    }
}

function ValidarPatenteCnrt() {
    var patente = $("#PatenteCamion").val();
    var acoplado = $("#PatenteAcoplado").val();
    if (patente != '' && acoplado != '') {
        ActualizarTipoVehiculo(patente, acoplado, function () { BlockUI(" consulta de tipo de vehiculo por patente"); }, function () { $.unblockUI(); });
    }
}
function ActualizarTipoVehiculo(patente, acoplado, before, callback) {
    if (before != null) before();
    $.getJSON($("#links").data().urlObtenerTipovehiculoPorPatente, { patente: patente, acoplado: acoplado, workflow: $('#WorkflowDescripcion').val() }, function (data) {
        if (data.CodigoDeError == 0) {
            if (data.Categoria != null) {
                if ($('#TipoVehiculo option[value=' + data.Categoria + ']').length == 0) {
                    MostrarAlertaError("La categoría del vehículo " + data.CategoriaDesc + " no esta configurada para el centro actual");
                } else {
                    $('#TipoVehiculo').val(data.Categoria);
                }
            } else {
                MostrarAlertaError("El servicio CNRT no devolvió información sobre la categoría del vehículo, debe ingresarla manualmente.");
            }
        } else if (data.CodigoDeError == 1) {
            $('.btn').removeAttr('disabled');
        } else {
            MostrarAlertaAdvertencia(data.Error);
        }
    }).complete(function () {
        if (callback != null) callback();
    });
}