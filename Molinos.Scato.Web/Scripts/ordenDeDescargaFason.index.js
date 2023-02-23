jQuery(document).ready(function ($) {
    //Máscaras
    $(".numeroRemito").mask("9999-99999999");

    $(".patente-internacional").mask("?*******", { placeholder: "" });
    $("#FechaOD").click(function () {
        $("#FechaCP").mask("99/99/9999");
    });

    $('.numero-ctg-cpe').attr("hidden", true);
    $('#marco-cp').attr("hidden", true);
    $('.procedenciaDG').attr("hidden", true);


    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;
    DefinirAutocompletarChofer();
    DefinirAutocompletar('#Procedencia', '#ProcedenciaId', $('#links').data().urlBuscarProcedencias, $('#links').data().urlBuscarProcedenciaUnica);
    DefinirAutocompletarConSAP('#Cliente', '#ClienteId', '#autocompleteCliente', $('#links').data().urlBuscarClientes, $('#links').data().urlBuscarClienteUnico, $('#links').data().urlObtenerClientesSap, null, null);

    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);

    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
        if (!$('#Transportista').hasClass('transportistaRequerido')) ValidarObjeto($("#orden-form"), $("#Transportista"));
    });

    // para que el campo retome el foco al seleccionar una fecha
    $('input.date').datepicker("option", "onSelect", function () {
        $(this).focus();
    });

    $('.peso').change(function () {
        var pesoBruto = $('#pesoBruto').val();
        var pesoTara = $('#pesoTara').val();

        if (pesoBruto < 0)
            pesoBruto = 0;
        if (pesoTara < 0)
            pesoTara = 0;

        $('#pesoNeto').val(pesoBruto - pesoTara);
    });

    $.validator.addMethod("clienteRequerido", function (value, element) {
        return $('#ClienteId').val() > 0;
    }, $('#Cliente').data().errorRequerido);
    $(".pesoNetoMaximoEjecutar").change(
        function () {
            $('.pesoNetoMaximo').valid();
        }
    );

    $.validator.addMethod("pesoNetoMaximo", function (value, element) {
        var pesoNeto = $('#pesoNeto').val();
        if (pesoNeto > 0 && $('#pesoBruto').val() > 0 && $('#pesoTara').val() > 0) {
            var netoMax = $('#tipoVehiculoDropdown :selected').data('netomaximo');
            return netoMax == null || pesoNeto <= netoMax;
        } else {
            return true;
        }
    }, $('#pesoNeto').data().errorPesonetomaximo);
    cargarTiposVehiculo();

    $('#PatenteCamion').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado').change(ValidarPatenteCnrt);
    $('#NumeroCTG').change(ValidarCTG);
    $('#MaterialId').change(function () {
        var materialId = $("#MaterialId").val();
        var datos = $("#Material").val();

        $('#EsDerivadoGranario').val(JSON.parse(datos).find(f => f.Id === materialId).EsDerivadoGranario);

        if (JSON.parse(datos).find(f => f.Id === materialId).EsDerivadoGranario) {
            $('.numero-ctg-cpe').removeAttr('hidden');
            $('#marco-cp').removeAttr('hidden');
            $('#divImagenPdf').attr("style", "width: 30%")
            $('.procedencia').attr("hidden", true);
            $('.procedencia').prop('required',false)
            $('.procedenciaDG').removeAttr('hidden');
        }

        else {
            $('.numero-ctg-cpe').attr("hidden", true);
            $('#marco-cp').attr("hidden", true);
            $('#Sucursal').val('');
            $('#NroOrden').val('');
            $('#NumeroCTG').val('');
            $("#imagen-cp").attr("src", "");
            $.removeData($('#imagen-cp'), 'elevateZoom');//borra la data zoom de img
            $('.zoomContainer').remove();// borra el contenedor de zoom en el DOM
            $('#divImagenPdf').removeAttr("style")
            $('.procedencia').removeAttr('hidden');
            $('.procedencia').prop('required',true);
            $('.procedenciaDG').attr("hidden", true);

        }
    });

    $('#imagen-cp').mouseover(function () {
        console.log("Antes")

        $('#imagen-cp').elevateZoom({
            zoomType: "inner",
            cursor: "crosshair",
            scrollZoom: true,
        });
    })
        .mouseout(function () {
        });

    if ($("#NumeroCTG").val() != '') {
        $('.numero-ctg-cpe').removeAttr("hidden");
        $('.procedencia').attr("hidden", true);
        $('.procedenciaDG').removeAttr("hidden");
    }
});

function cargarMaterial() {
    var clienteId = $('#ClienteId').val();
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

function ValidarCTG() {
    var nroCartaPorte = $('#NumeroCTG').val();
    var codigoPadre = null;
    var codigoDerivadoGranario = null;

    if (nroCartaPorte != '') {
        BuscarNumeroCPE(nroCartaPorte, function () { BlockUI(" consulta de numero de cpe"); }, function () { $.unblockUI(); }, codigoPadre, codigoDerivadoGranario);
    }
}

function BuscarNumeroCPE(ctg, before, callback, codigoPadre, codigoDerivadoGranario) {
    if (before != null) before();
    $.getJSON($("#links").data().urlObtenerCartaPorteCtg, { numeroCtg: ctg }, function (data) {
        if (data.status == 500) {
            MostrarAlertaAdvertencia(data.satus);
        }
    }).complete(function (data) {
        var cadenaSucursal = "000000000" + data.responseJSON.Sucursal;
        var cadenaNumeroOrden = "000000000" + data.responseJSON.NroOrden;
        var formatSucursal = cadenaSucursal.substr(cadenaSucursal.length - 5);
        var formatNroOrden = cadenaNumeroOrden.substr(cadenaNumeroOrden.length - 8);
        var cuitOrigen = data.responseJSON.CuitOrigen;
        var plantaDG = data.responseJSON.PlantaDG;
        codigoPadre = data.responseJSON.CodigoPadre;
        codigoDerivadoGranario = data.responseJSON.CodigoGranario;

        $('#Sucursal').val(formatSucursal);
        $('#NroOrden').val(formatNroOrden);
        
        $('#Transportista').val(data.responseJSON.CuitTransportista);
        $('#Transportista').focusout();
    
        $('#PatenteCamion').val(data.responseJSON.PatenteCamion);
        $('#PatenteAcoplado').val(data.responseJSON.PatenteAcoplado);

        $('#Chofer_Cuil').val(data.responseJSON.CuitChofer);
        $('#Chofer_Cuil').focusout();
        
        $('#pesoBruto').val(data.responseJSON.PesoBruto);
        $('#pesoTara').val(data.responseJSON.PesoTara);
        $("#imagen-cp").attr("src", "data:application/pdf;base64," + data.responseJSON.PdfBase);

        BuscarCliente(cuitOrigen);
        BuscarProcedenciaDG(plantaDG);
        BuscarMaterialDG(codigoPadre, codigoDerivadoGranario);


        if (callback != null) callback();
    });
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
                if ($('#tipoVehiculoDropdown option[value=' + data.Categoria + ']').length == 0) {
                    MostrarAlertaError("La categoría del vehículo " + data.CategoriaDesc + " no esta configurada para el centro actual");
                } else {
                    $('#tipoVehiculoDropdown').val(data.Categoria);
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


function BuscarMaterialDG(codigoPadre, codigoDerivadoGranario){
    $.getJSON($("#links").data().urlObtenerMaterialUnico, { codigoPadre: codigoPadre, codigoDerivadoGranario: codigoDerivadoGranario }, function (data) {
        if (data.status == 500) {
            MostrarAlertaAdvertencia(data.satus);
        }
    }).complete(function (data) {
        if(data.responseJSON != null && data.responseJSON.Id != null){
            var materialId = data.responseJSON.Id;
            $("#MaterialId").val(materialId);        
        }
    });
}

function BuscarCliente(cuitOrigen){
    $.getJSON($("#links").data().urlBuscarClienteUnico, { term: cuitOrigen }, function (data) {
        if (data.status == 500) {
            MostrarAlertaAdvertencia(data.satus);
        }
    }).complete(function (data) {
        if(data.responseJSON != null && data.responseJSON.Id != null){
            var descripcion = data.responseJSON.Descripcion;
            $("#Cliente").val(descripcion);  
            $('#Cliente').focusout();      
        }
    });
}

function BuscarProcedenciaDG(plantaDG){
    $.getJSON($("#links").data().urlObtenerProcedenciaDg, { plantaDG: plantaDG }, function (data) {
        if (data.status == 500) {
            MostrarAlertaAdvertencia(data.satus);
        }
    }).complete(function (data) {
        if(data.responseJSON != null && data.responseJSON.Id != null){
            var descripcion = data.responseJSON.Descripcion;
            var domicilioId = data.responseJSON.Id;
            $("#Domicilio").val(descripcion);  
            $("#DomicilioId").val(domicilioId);  
            $("#Procedencia").val(" ");
        }
    });
}