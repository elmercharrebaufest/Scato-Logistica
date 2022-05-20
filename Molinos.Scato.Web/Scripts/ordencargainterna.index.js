$(document).ready(function () {
    //Foco en primer elemento
    $("#orden-form").find(':input:not([readonly]):enabled:visible:first').focus();
    $(".patente-internacional").mask("?*******", { placeholder: "" });

    DefinirAutocompletarChofer();
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
    $('#FechaEmision').mask(formatoFecha);

    $("#FechaEmision").datepicker();



    $.validator.addMethod("clienteRequerido", function (value, element) {
        return value.length > 0;
    }, $('#errorClienteRequerido').data().errorRequerido);

    if ($('#Destino').length > 0) {
        DefinirAutocompletarConSAP('#Destino', '#DestinoId', '#autocompleteDestino', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarClienteUnico, $('#links').data().urlObtenerClientesSap, function () {  validarClienteNoBloqueado(); cargarMaterial(); }, function () { deshabilitarKmRecorrerYLocalidad(); cargarMaterial(); });
    }

    if ($('#Cliente').length > 0) {
        DefinirAutocompletarConSAP('#Cliente', '#ClienteId', '#autocompleteCliente', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarClienteUnico, $('#links').data().urlObtenerClientesSap, function () { completarKmRecorrerYLocalidad(); cargarMaterial(); }, function () { deshabilitarKmRecorrerYLocalidad(); cargarMaterial(); });
    }

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    });
    completarKmRecorrerYLocalidad();
    $('#localidadDestinoDropdown').change(function () {
        $('#KmARecorrer').val($('#localidadDestinoDropdown :selected').data('kilometros'));
    });
    $('#PatenteCamion').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado').change(ValidarPatenteCnrt);
    $('#MaterialId').change(CargarAlamacenesPorMaterial);

    

});

function cargarMaterial() {
    try {
        if (noCargar == true) {
            return;
        }
    }
    catch (e) {
    }

    var clienteId;
    if ($('#Destino').length > 0) {
        clienteId = $('#DestinoId').val();
    }
    if ($('#Cliente').length > 0) {
        clienteId = $('#ClienteId').val();
    }

    $.getJSON($('#links').data().urlObtenerMateriales, { workflowId: workflowId, centroId: $('#centroId').val(), clienteId: clienteId },
        function (allData) {

            var options = '';
            for (var j = 0; j < allData.length; j++) {
                options += "<option value='" + allData[j].Value + "'>"
                    + allData[j].Text + "</option>";
            }
            $('#MaterialId').html(options);
        }
    );
}

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function deshabilitarKmRecorrerYLocalidad() {

    if ($('#ClienteId').length == 0 || $('#ClienteId').val() == null || $('#ClienteId').val() == '0' || $('#localidadDestinoDropdown option').length == 0) {
        $('#KmARecorrer').val("");
        $('#LocalidadDestinoId').val(0);
        $('#localidadDestinoDropdown').html(null);
        $('#KmARecorrer').attr("disabled", true);
        $('#localidadDestinoDropdown').attr("disabled", true);
    }
}

function completarKmRecorrerYLocalidad() {
    var clienteId;
    if ($('#Destino').length > 0) {
        clienteId = $('#DestinoId').val();
    }
    if ($('#Cliente').length > 0) {
        clienteId = $('#ClienteId').val();
    }
    if (clienteId > 0) {

        $.getJSON($('#links').data().urlBuscarKmporproveedor, { clienteId: clienteId },
            function (response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    options += "<option data-kilometros='" + response[i].kmRecorrer + "' value='" + response[i].localidadDestinoId + "'" + ">"
                        + response[i].localidadDescripcion + "</option>";
                }
                if (response.length > 0) {
                    var optdefault = '';
                    optdefault = "<option value=''> (Localidad) </option>";
                    optdefault += options;
                    $('#localidadDestinoDropdown').html(optdefault);
                    $('#localidadDestinoDropdown').removeAttr("disabled");
                    $('#KmARecorrer').removeAttr("disabled");

                    if ($('#LocalidadDestinoId').val() > 0) {
                        $('#localidadDestinoDropdown').val($('#LocalidadDestinoId').val());
                        $('#KmARecorrer').val($('#localidadDestinoDropdown :selected').data('kilometros'));
                    }
                } else {
                    deshabilitarKmRecorrerYLocalidad();
                    MostrarAlertaAdvertencia("El cliente no tiene km a recorrer asociados");
                }

            });


    } else {
        deshabilitarKmRecorrerYLocalidad();
    }

}

function validarClienteNoBloqueado() {
    var clienteId;
    if ($('#Destino').length > 0) {
        clienteId = $('#DestinoId').val();
    }
    if ($('#Cliente').length > 0) {
        clienteId = $('#ClienteId').val();
    }
    if (clienteId > 0) {

        $.getJSON($('#links').data().urlObtenerEstadoCliente, { clienteId: clienteId })
            .done(function (response) {
                if (response.bloqueado) {
                    MostrarAlertaError("El cliente se encuentra bloqueado");
                } else {
                    completarKmRecorrerYLocalidad();
                }
                $("#btnAceptar").attr("disabled", response.bloqueado);
            });
    }
    else {
        deshabilitarKmRecorrerYLocalidad();
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

function CargarAlamacenesPorMaterial() {
    var material = $("#MaterialId").val();
    $.getJSON($('#links').data().urlObtenerAlmacenesPorMaterial, { materialId: material},
        function (allData) {
            var options = '';
            for (var j = 0; j < allData.length; j++) {
                options += "<option value='" + allData[j].Id + "'>"
                    + allData[j].Descripcion + "</option>";
            }
            $('#Almacen_Id').html(options);
        }
    );
}