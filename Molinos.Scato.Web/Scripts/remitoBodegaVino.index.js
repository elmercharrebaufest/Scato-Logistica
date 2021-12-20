$(document).ready(function () {
    //Foco en primer elemento
    $("#orden-form").find(':input:not([readonly]):enabled:visible:first').focus();

    $(".patente-internacional").mask("?*******", {placeholder: ""});
    $("#NroRemito").mask("9999-99999999");
    $("#OrdenDeCompra").mask("9999999999");
    $("#ModeloCamion").mask("9999");

    DefinirAutocompletarChofer();

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    });


    $('#TipoVehiculoBodegaId').val('option: first').val();
    //inhabilitar campos hasta busqueda en SAP
    InhabilitarCampos();

    $('#mensajeError').hide();

    $('#aplicar').click(function () {
        buscarEnSAP();
    });

    $('#OrdenDeCompra').on('change', function() {
        $('#OrdenDeCompra').html("");
        $('#OrdenDeCompra').addClass('field-validation-valid');
        $('#OrdenDeCompra').removeClass('field-validation-error');
    });

    $('#Patente').on('change', function() {
        $('#Patente').html("");
        $('#Patente').addClass('field-validation-valid');
        $('#Patente').removeClass('field-validation-error');
    });

    $('#NroRemito').on('change', function() {
        $('#NroRemito').html("");
        $('#NroRemito').addClass('field-validation-valid');
        $('#NroRemito').removeClass('field-validation-error');
    });

    $('#SolicitudDatosSap').keypress(function(event) {
        if (event.keyCode == 13) {
            buscarEnSAP();
            return false;
        }
    });
    
    $('#PesoBrutoOrigen').change(function () {
        CalcularPesoNetoOrigen();
    });

    $('#PesoTaraOrigen').change(function () {
        CalcularPesoNetoOrigen();
    });

});

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function FiltrarMaterialesPorOrden() {
    if ($('#OrdenDeCompra').val() > "") {
        $.getJSON($('#links').data().urlFiltrarMateriales, { workflow: $('#workflow').val(), centroId: $('#centroId').val(), materialesCodigoSap: $('#materialesCodigoSap').val() },
            function (allData) {

                var options = '';
                for (var j = 0; j < allData.length; j++) {
                    options += "<option value='" + allData[j].Value + "'>"
                            + allData[j].Text + "</option>";
                }
                $('#MaterialIdYPosicion').html(options);
            }
        );
    }
}

function CalcularPesoNetoOrigen() {
    var pesoBrutoOrigen = 0;
    if ($.isNumeric(parseInt($('#PesoBrutoOrigen').val()))) {
        pesoBrutoOrigen = $('#PesoBrutoOrigen').val();
    }
    var pesoTaraOrigen = 0;
    if ($.isNumeric(parseInt($('#PesoTaraOrigen').val()))) {
        pesoTaraOrigen = $('#PesoTaraOrigen').val();
    }
    $('#PesoNetoOrigen').val(pesoBrutoOrigen - pesoTaraOrigen);
}

function InhabilitarCampos() {
    $('select').attr("readonly", "readonly");
    $('input').attr("readonly", "readonly");
    $('.agregar').attr("disabled", "disabled");
    $('.aceptar').attr("disabled", "disabled");
    $('#tiposComerciales').attr("disabled", "disabled");
    $('#tiposDeVehiculo').attr("disabled", "disabled");
    $('#listaMateriales').attr("disabled", "disabled");
    $('#tiposDeDocumento').attr("disabled", "disabled");
    
    $('#Patente').removeAttr("readonly");
    $('#NroRemito').removeAttr("readonly");
    $('#OrdenDeCompra').removeAttr("readonly");
}

function HabilitarCampos() {
    $('select').removeAttr("readonly");
    $('input').removeAttr("readonly");
    $('.agregar').removeAttr("disabled");
    $('.aceptar').removeAttr("disabled");

    $('#PesoNetoOrigen').attr("readonly", "readonly");

    $('#listaMateriales').removeAttr("disabled");
    $('#tiposComerciales').removeAttr("disabled");
    $('#tiposDeVehiculo').removeAttr("disabled");
    $('#tiposDeDocumento').removeAttr("disabled");
}

function buscarEnSAP() {

    if ($('#OrdenDeCompra').val().length > 0 && $('#Patente').val().length > 0 && $('#NroRemito').val().length > 0)  {

        $('.OrdenDeCompramensaje').html("");
        $('.OrdenDeCompramensaje').addClass('field-validation-valid');
        $('.OrdenDeCompramensaje').removeClass('field-validation-error');

        $('.Patentemensaje').html("");
        $('.Patentemensaje').addClass('field-validation-valid');
        $('.Patentemensaje').removeClass('field-validation-error');

        $('.NroRemitoMensaje').html("");
        $('.NroRemitoMensaje').addClass('field-validation-valid');
        $('.NroRemitoMensaje').removeClass('field-validation-error');


        BlockUI();
        $.getJSON($('#links').data().urlBuscarSap, { ordenDeCompra: $('#OrdenDeCompra').val() },
            function(data) {
                if (data.mensajeError > "") {
                    MostrarAlertaError(data.mensajeError);
                } else {
                    $('#ProveedorId').val(data.proveedorId);
                    $('#Proveedor').val(data.proveedorDesc);
                    $('#Proveedor').attr("disabled", "disabled");
                    $('#materialesCodigoSap').val(data.materialesCodigoSAP);

                    HabilitarCampos();
                    FiltrarMaterialesPorOrden();
                }
            }).always(function() {
                $.unblockUI();
            });
    }
    else{
        if (!$('#Patente').val().length > 0) {
            $('.PatenteMensaje').html($('#Patente').data().mensajerequerido);
            $('.PatenteMensaje').addClass('field-validation-error');
            $('.PatenteMensaje').removeClass('field-validation-valid');
            
        }
        if (!$('#NroRemito').val().length > 0) {
            $('.NroRemitoMensaje').html($('#NroRemito').data().mensajerequerido);
            $('.NroRemitoMensaje').addClass('field-validation-error');
            $('.NroRemitoMensaje').removeClass('field-validation-valid');
        }
        if (!$('#OrdenDeCompra').val().length > 0) {
                $('.OrdenDeCompraMensaje').html($('#OrdenDeCompra').data().mensajerequerido);
                $('.OrdenDeCompraMensaje').addClass('field-validation-error');
                $('.OrdenDeCompraMensaje').removeClass('field-validation-valid');
        }
   }
   
}