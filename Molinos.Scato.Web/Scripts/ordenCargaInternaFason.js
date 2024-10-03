
﻿$(document).ready(function () {
    var patenteCamion = $('#PatenteCamion').val();
    if (patenteCamion) {
        obtenerOrdenDeCargaOperacionesPorPatente();
    }

})


var cachedOrdenDeCargaOperaciones;
var domicilioConcat = "";
var $selectOption;





function obtenerOrdenDeCargaOperacionesPorPatente() {
    const regex1 = /^[A-Z]{3}\d{3}$/;  // Regex para formato ABC123
    const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Regex para formato AB123CD
    let url = window.location.search;
    let urlParams = new URLSearchParams(url);
    let workflowId = urlParams.get("workflow");

    var patente = $("#PatenteCamion").val().toUpperCase();
    $("#NumeroOrdenExterno").empty();
    if (typeof hayError !== 'undefined' && hayError !== "True") {
        limpiarCamposOrdenDeCargaOperaciones();
    }

    if (patente && (!regex1.test(patente) && !regex2.test(patente))) {
        ValidarDerivadoGranario()
        return;
    }

    let tipoComercial = $('#tiposComerciales').find('option').eq(1).val();
    $('#tiposComerciales').find('select').val(tipoComercial)
    $("#TipoComercialId[type='hidden']").val(tipoComercial)

    BlockUI();
    $.ajax({
        url: $('#links').data().urlObtenerOrdenDeCargaOperacionesPorPatente,
        dataType: 'json',
        data: { patente: patente, workflow: workflowId },
        type: "GET",
        success: function (data) {
            manejarRespuestaExitosa(data);
        },
        error: function (xhr, status, error) {
            const regex = /<h2>(.*?)<\/h2>/;
            const err = xhr.responseText.match(regex);
            MostrarAlertaError(err[1]);
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function manejarRespuestaExitosa(data) {
    if (!data || (!data.Data && !Array.isArray(data.Mensajes)) || (!Array.isArray(data.Data) && data.Mensajes.length === 0)) {
        mostrarInfoAlerta();
        return;
    }

    if (data.TieneAdvertencias) {
        MostrarAlertaAdvertencia(data.Mensajes[0].Mensaje);
    }

    if (!data.EsValido) {
        MostrarAlertaError(data.Mensajes[0].Mensaje);
        return;
    }

    cachedOrdenDeCargaOperaciones = data.Data;
    $("#NumeroOrdenExterno").append($("<option></option>").attr("value", "").text("(Ninguno)"));
    var materialesDerivadoGranario = JSON.parse($("#ListaMaterialesDerivadoGranario").val())

    if (Array.isArray(data.Data)) {
        var nombreCliente = "";
        data.Data.forEach(function (value) {
            if (materialesDerivadoGranario.includes(parseInt(value.MaterialId))) {
                nombreCliente = value.RazonSocialDestino
            } else {
                nombreCliente = value.Cliente
            }

            $("#NumeroOrdenExterno").append($("<option></option>").attr("value", value.Id).attr("title", nombreCliente).text(value.Id.toString().padStart(8, '0') + " | " + nombreCliente));
        });

        var selectedValue = $("#NumeroOrdenExterno").data('selected-value');
        if (data.Data.length === 1 && selectedValue !== undefined && selectedValue !== null) {
            $("#NumeroOrdenExterno").val(data.Data[0].Id);
            seleccionarOrdenDeCargaOperaciones();
        } else if (selectedValue) {
            $("#NumeroOrdenExterno").val(selectedValue);
        } else if (data.Data.length > 1) {
            MostrarAlertaAdvertencia(textoVariasOrdenes);
        }
    }

}


function mostrarInfoAlerta() {
    MostrarAlertaAdvertencia(patenteNoEncontrada);
    ValidarDerivadoGranario()
    limpiarCamposOrdenDeCargaOperaciones()
}


function seleccionarOrdenDeCargaOperaciones() {
    var ddlNumeroOrden = $("#NumeroOrdenExterno");
    var selectedElement = obtenerElementoSeleccionado(ddlNumeroOrden.val());

    if (!selectedElement) {
        return;
    }

    if (ddlNumeroOrden.val() !== '0') {
        BlockUI();
        $.ajax({
            url: $('#links').data().urlObtenerOrdenDeCargaOperacionesSeleccionada,
            dataType: 'json',
            data: obtenerDatosAjax(selectedElement),
            type: "GET",
            success: function (data) {
                manejarRespuestaAjaxSeleccion(data, selectedElement);
            },
            error: function (xhr, status, error) {

            },
            complete: function () {
                $.unblockUI();
            }
        });
    }
}

function obtenerElementoSeleccionado(id) {
    return cachedOrdenDeCargaOperaciones.find(x => x.Id == id);
}

function obtenerDatosAjax(selectedElement) {
    return {
        clienteCUIT: selectedElement.CUITCliente,
        transportistaCUIT: selectedElement.CUITTransporte,
        patente: selectedElement.PatenteChasis,
        acoplado: selectedElement.PatenteAcoplado,
        materialSAP: selectedElement.CodigoProducto,
        ordenId: selectedElement.Id,
        DestinoCuit: selectedElement.CUITDestino
    };
}

function manejarRespuestaAjaxSeleccion(data, selectedElement) {
    if (typeof data.errorResponse === "object") manejarErrorAjax(data);
    if (data.TieneAdvertencias) {
        MostrarAlertaAdvertencia(data.Mensajes[0].Mensaje);
    }

    if (!data.EsValido) {
        MostrarAlertaError(data.Mensajes[0].Mensaje);
        rellenarCampos(data, selectedElement);
        return;
    }

    rellenarCampos(data, selectedElement);
}

function manejarErrorAjax(data) {
    if (data.errorResponse.duplicado === true) {
        $("#dialogo-advertir-body").html("<strong>Fason. Existe mas de una entidad sap para el cuit ingresado. No se puede continuar con la carga</strong>");
        $('#dialogo-advertir').css({
            'top': '30%',
            'margin-left': function () {
                return -($(this).width() / 2);
            },
            'left': '50%',
            'margin-top': function () {
                return -($(this).height() / 2.6);
            }
        });
        $("#dialogo-advertir").modal('show');
        $.unblockUI();
        return;
    } else {
        MostrarAlertaError("Error en la petición AJAX: " + data.errorResponse.error)
        $.unblockUI();
        return;
    }
}

function rellenarCampos(data, selectedElement) {
    domicilioConcat = `${selectedElement.DomicilioTipo}-${selectedElement.DomicilioOrden}`;

    let tipoComercial = $('#tiposComerciales').find('option').eq(1).val();
    $("#Cliente").val(null);
    $('#tiposComerciales').find('select').val(tipoComercial)
    $("#PatenteAcoplado").val(selectedElement.PatenteAcoplado);
    $("#ClienteId").val(data.Data.ClienteId);
    $("#Cliente").val(data.Data.ClienteDescripcion);
    $("#ClienteOriginalId").val(data.Data.ClienteId);
    $("#ClienteOriginal").val(data.Data.ClienteDescripcion);

    $("#TransportistaId").val(data.Data.TransportistaId);
    $("#Transportista").val(data.Data.TransportistaDescripcion);
    $("#Chofer_Cuil").val(convertirCuil(selectedElement.CUILChofer));
    $("#MaterialId").val(data.Data.MaterialId);
    $("#TipoVehiculo").val(data.Data.TipoDeVehiculo);
    $("#Destinatario").val(data.Data.Orden.RazonSocialDestinatario);
    $("#IntermediarioFlete").val(data.Data.Orden.RazonSocialIntermediarioFlete);
    $("#LocalidadSeleccionada").val(data.Data.Orden.LocalidadId);
    $("#LocalidadDestinoId").val(data.Data.Orden.LocalidadId);
    $("#DestinoGranario").val(data.Data.DestinoDescripcion);
    $("#DestinoGranarioId").val(data.Data.DestinoId);
    $("#Corredor").val(data.Data.Orden.Corredor);
    $("#Observaciones").val(data.Data.Orden.Observacion);
    $("#Remitente").val(data.Data.Orden.RemitenteComercial);
    $("#PagadorFlete").val(data.Data.Orden.PagadorFlete);
    $("#LocalidadDescripcion").val(data.Data.Orden.LocalidadDescripcion);

    //rellenar campos ocultos
    $("#Chofer_Cuil[type='hidden']").val(convertirCuil(selectedElement.CUILChofer));
    $("#PatenteAcoplado[type='hidden']").val(selectedElement.PatenteAcoplado);
    $("#MaterialId[type='hidden']").val(data.Data.MaterialId);
    $("#Chofer_NumeroDeDocumento[type='hidden']").val(selectedElement.CUILChofer.slice(2, -1));
    $("#KmARecorrer[type='hidden']").val(data.Data.Orden.KmARecorrer);
    $("#PlantaSeleccionada[type='hidden']").val(data.Data.Orden.PlantaCodigo);
    $("#OrdenDomicilioDestino[type='hidden']").val(data.Data.Orden.DomicilioOrden);
    $("#TipoDomicilioDestino[type='hidden']").val(data.Data.Orden.DomicilioTipo);
    $("#DestinoMercaderia[type='hidden']").val(data.Data.Orden.DestinoMercaderia);
    $("#TipoComercialId[type='hidden']").val(tipoComercial)
    $("#RemitenteId[type='hidden']").val(data.Data.Orden.RemitenteComercialId)

    if (data.Data.TieneErrorCNRT) {

        $('#TipoVehiculo').css({ 'pointer-events': '', 'background-color': '', 'color': '' });
    }
    else {
        $('#TipoVehiculo').css({ 'pointer-events': 'none', 'background-color': '#f0f0f0', 'color': '#666' });

    }


    ValidarDerivadoGranario();

    let elementos = ["#Chofer_Cuil", "#Cliente", "#Destinatario", "#Remitente", "#Transportista", "#IntermediarioFlete", "#PagadorFlete"];

    elementos.forEach(function (selector) {
        let $element = $(selector);

        // Solo disparar eventos si el campo no está vacío
        if ($element.val().trim() !== "") {
            $element.trigger('keydown').trigger('focusout');
            setTimeout(() => $element.blur(), 100);
        }
    });


    setTimeout(function () {
        var existeDomicilio = $("#TipoYOrdenDestino option[value='" + domicilioConcat + "']").end();
        $('#KmARecorrer').val(data.Data.Orden.KmARecorrer); 
        $('#PlantaDGDestino').val(data.Data.Orden.PlantaCodigo);

        if (existeDomicilio.length > 0) {
            $("#TipoYOrdenDestino").val(domicilioConcat);
        } else {
            MostrarAlertaAdvertencia('El domicilio recibido no coincide con los datos de Scato, verifique ó elija uno correcto.');
        }
    }, 2000);
}

function ValidarDerivadoGranario() {
    let materialId = $("#MaterialId").val();
    let materialesDerivadoGranario = JSON.parse($("#ListaMaterialesDerivadoGranario").val())
    if (materialesDerivadoGranario.includes(parseInt(materialId))) {
        $('#DerivadoGranarioHabilitado').val('true')
        $('.derivadoGranario').removeClass('hidden');
        $("label[for='Cliente']").text('Destino');
        $("#Cliente").val($("#DestinoGranario").val());
        $("#ClienteId").val($("#DestinoGranarioId").val());

        CargarPlantas();
        CargarDomicilios();
    } else {
        $("#Cliente").val($("#ClienteOriginal").val());
        $("#ClienteId").val($("#ClienteOriginalId").val());
        $('#DerivadoGranarioHabilitado').val('false')
        $('.derivadoGranario').addClass('hidden');
        $("label[for='Cliente']").text('Cliente');
        $('#PlantaDGDestino').val('');
        $('#TipoYOrdenDestino').val('');
        $('#PagadorFlete').val('');
        $('#PagadorFleteId').val('');
        $("#PlantaSeleccionada").val('')
    }
}
function limpiarCamposOrdenDeCargaOperaciones() {
    $("#PatenteAcoplado").val(null);
    $("#NumeroOrdenExterno").val(null);
    $("#ClienteId").val(null);
    $("#Cliente").val(null);
    $("#TransportistaId").val(null);
    $("#Transportista").val(null);
    $("#Chofer_Cuil").val(null);
    $("#Chofer_Nombre").val(null);
    $("#Chofer_Apellido").val(null);
    $("#Chofer_NumeroDeDocumento").val(null);
    $("#KmARecorrer").val(null);
    $("#KmARecorrer[type='hidden']").val(null);
    $("#MaterialId").val(null);
    $("#TipoVehiculo").val(null);
    $("#Destinatario").val(null);
    $("#TipoVehiculo").val(null);
    $("#Destinatario").val(null); /*DA*/
    $("#Remitente").val(null);
    $("#TipoComercialId").val(null);
    $("#IntermediarioFlete").val(null);
    $("#LocalidadDestinoId").val(null);
    $("#PlantaDGDestino").val(null);
    $("#TipoYOrdenDestino").val(null);
    $("#PagadorFlete").val(null);
    $("#Corredor").val(null);
    $("#localidadDestinoDropdown").empty();
    $("#Observaciones").val(null);
    $("#RemitenteId").val(null);
    $("#LocalidadDescripcion").val(null);
    $("#DestinoMercaderia").val(null);
}

function convertirCuil(cuil) {
    if (!cuil || cuil.length !== 11) {
        return "";
    }

    var validador1 = cuil.substring(0, 2);
    var documento = cuil.substring(2, 10);
    var validador2 = cuil.substring(10, 11);

    return validador1 + "-" + documento + "-" + validador2;
}

function establecerDestinatario(data) {
    $('#DestinatarioId').val(data.DestinatarioId); // Establecer ID del destinatario
}





