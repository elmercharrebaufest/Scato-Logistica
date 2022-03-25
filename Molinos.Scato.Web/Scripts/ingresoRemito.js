jQuery(document).ready(function ($) {
    $(".patente-internacional").mask("?*******", {placeholder: ""});
    //$('#KmRecorrer').mask('?9999');
    $("#Remito").mask("9999-99999999");
    $("#Cosecha").mask("99-99");
    $("#Material").val("");
    $("#FechaMovimiento").click(function () {
        $("#FechaCP").mask("99/99/9999");
    });

    if ($("#EsRemitoProveedor").val() == "False") {
        DeshabilitarCampos(true);
        $('#Remito').on("focusout", function() {
            if ($('#Remito').val().length == 13) {
                $('.datoDB').val("");
                ObtenerDatosDB();
            }
        });
    }
    
    if ($('#esIngreso').val() == "True") {
        $.validator.addMethod("anexoRequerido", function (value, element) {
            return value.length > 0;
        }, $('#mensajeAnexo').data().errorRequerido);
        MostrarTextBoxAnexo();
        $('#MaterialId').change(function () { MostrarTextBoxAnexo(); });      
    }

    // para que el campo retome el foco al seleccionar una fecha
    $('input.date').datepicker("option", "onSelect", function () {
        $(this).focus();
    });
    
    $('#PesoBrutoOrigen').change(function () {
        CalcularPesoNetoOrigen();
    });

    $('#PesoTaraOrigen').change(function () {
        CalcularPesoNetoOrigen();
    });
    
    $.validator.addMethod("KmRecorrerNumerico", function (value, element) {
        return $.isNumeric(Globalize.parseFloat(value)) || !value.trim();
    }, $('#KmRecorrer').data().errorNumerico);
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

    $.validator.addMethod("cosechaValidacion", function (value, element) {
        var cosecha = $('#Cosecha').val().split('-');
        if (parseInt(cosecha[0]) > parseInt(cosecha[1]))
            return false;
        return true;
    }, $('#Cosecha').data().error);

    $.validator.addMethod("cosechaValidacionDiferencia", function (value, element) {
        var cosecha = $('#Cosecha').val().split('-');
        if (parseInt(cosecha[1]) - parseInt(cosecha[0]) !== 1)
            return false;
        return true;
    }, $('#Cosecha').data().errordiferencia);

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletarChofer();

    if ($("#EsRemitoProveedor").val() == "True") {
        DefinirAutocompletar('#Origen', '#OrigenId', listarProveedores, obtenerProveedor);
    } else {
        DefinirAutocompletar('#Origen', '#OrigenId', $('#links').data().urlBuscarCentros, $('#links').data().urlBuscarCentroUnico);
    }
    DefinirAutocompletar('#Procedencia', '#ProcedenciaId', $('#links').data().urlBuscarProcedencias, $('#links').data().urlBuscarProcedenciaUnica);
    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    });
    cargarTiposVehiculo();

    $('#PatenteCamion').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado').change(ValidarPatenteCnrt);
});

function MostrarTextBoxAnexo() {
    var resultado = jQuery.inArray($('#MaterialId').val(), $('#divanexo').data().materialesAnexos);

    if (resultado == -1) {
        $('#RequiereAnexoInase').val(false);
        $('#divanexo').addClass('hide');
        $('#CodigoAnexo').val('');
        $('#CodigoAnexo').removeClass('anexoRequerido');
    } else {
        $('#RequiereAnexoInase').val(true);
        $('#divanexo').removeClass('hide');
        //$('#CodigoAnexo').val('');
        $('#CodigoAnexo').addClass('anexoRequerido');
    }
}

//Deshabilito los campos. Por defecto los campos están grisados
function DeshabilitarCampos(valor) {
    $('#FechaOD').attr("readonly", valor);

    $('#Material').attr("readonly", valor);
    $('#Centro').attr("readonly", valor);
    $('#Transportista').attr("readonly", valor);
    $('#PatenteCamion').attr("readonly", valor);
    $('#PatenteAcoplado').attr("readonly", valor);
    //Chofer
    $('#Chofer_TipoDocumentoIdentidadId').attr("readonly", valor);
    //$('#Chofer_NumeroDeDocumento').attr("readonly", valor);
    $('#Chofer_Nombre').attr("readonly", valor);
    $('#Chofer_Apellido').attr("readonly", valor);
    $('#Chofer_Cuil').attr("readonly", valor);
    
    if (!valor) {
        $('#Chofer_Cuil').mask("99-99999999-9");
    }
    
    $('#AcuerdoMarco').attr("readonly", valor);
    $('#CodigoAnexo').attr("readonly", valor);
    $('#PesoBrutoOrigen').attr("readonly", valor);
    $('#PesoTaraOrigen').attr("readonly", valor);
}

//Obtengo los datos del Remito (si es que existe)
function ObtenerDatosDB() {
    if ($('#Remito').val() == '____-________') {
        DeshabilitarCampos(true);
    } else {
        BlockUI($("#MensajeBuscandoDatos").val());
        $.getJSON($("#Remito").data().numeroUrl, { numero: $('#Remito').val() }, function (data) {
            if (data.datosDB == -1 || data.datosDB == -3) {
                if (data.datosDB == -1) {
                    $('.btn').attr('disabled', true);
                    MostrarAlertaError(data.error);
                } else {
                    MostrarAlertaError(data.error);
                    DeshabilitarCampos(false);
                    $('.btn').removeAttr('disabled');
                }
            }
            else {
                LlenarFormulario(data.datosDB);
                $('.btn').removeAttr('disabled');
            }
        }).complete(function () {
            $.unblockUI();
        });
    }
}

//Lleno el Form con los datos obtenidos
function LlenarFormulario(datosDB) {
    DeshabilitarCampos(false);
    $('#Material').val(datosDB.Material);
    $('#MaterialId').val(datosDB.MaterialId);
    $('#Material').addClass('italic');
    $('#Origen').val(datosDB.Origen);
    $('#Origen').addClass('italic');
    $('#OrigenId').val(datosDB.OrigenId);
    $('#CentroCodigoSap').val(datosDB.CentroCodigoSap);
    $('#Transportista').val(datosDB.Transportista);
    $('#Transportista').addClass('italic');
    $('#TransportistaId').val(datosDB.TransportistaId);
    $('#PatenteCamion').val(datosDB.PatenteCamion);
    $('#PatenteAcoplado').val(datosDB.PatenteAcoplado);
    $('#Chofer_TipoDocumentoIdentidadId').val(datosDB.Chofer.TipoDocumentoIdentidadId);
    $('#Chofer_NumeroDeDocumento').val(datosDB.Chofer.NumeroDeDocumento);
    $('#Chofer_NumeroDeDocumento').addClass('italic');
    $('#Chofer_Nombre').val(datosDB.Chofer.Nombre);
    $('#Chofer_Nombre').addClass('italic');
    $('#Chofer_Apellido').val(datosDB.Chofer.Apellido);
    $('#Chofer_Apellido').addClass('italic');
    $('#Chofer_Cuil').val(datosDB.Chofer.Cuil);
    $('#Chofer_Cuil').addClass('italic');
    $('#Chofer_Id').val(datosDB.Chofer.Id);
    $('#DocLegalRemito').val(datosDB.DocLegalRemito);
    $('#EsTransportista').val(true);//Valor forzado
    
    $('#KmRecorrer').val(datosDB.KmRecorrer);
    $('#PesoBrutoOrigen').val(datosDB.PesoBrutoOrigen);
    $('#PesoTaraOrigen').val(datosDB.PesoTaraOrigen);
    $('#PesoNetoOrigen').val(datosDB.PesoNetoOrigen);
    $('#CodEstab').val(datosDB.CodEstab);
    $('#Procedencia').val(datosDB.Procedencia);
    $('#ProcedenciaId').val(datosDB.ProcedenciaId);
    

    
}

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
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