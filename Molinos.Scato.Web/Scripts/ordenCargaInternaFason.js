var cachedOrdenDeCargaOperaciones;
var domicilioConcat = "";
var $selectOption ;

function obtenerOrdenDeCargaOperacionesPorPatente() {
    const regex1 = /^[A-Z]{3}\d{3}$/;  // Regex para formato ABC123
    const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Regex para formato AB123CD

    var patente = $("#PatenteCamion").val().toUpperCase();
    $("#NumeroOrdenExterno").empty();
    if (hayError !== "True") limpiarCamposOrdenDeCargaOperaciones();

    if (!regex1.test(patente) && !regex2.test(patente)) {
        ValidarDerivadoGranario()
        return; 
    }

    BlockUI();
    $.ajax({
        url: $('#links').data().urlObtenerOrdenDeCargaOperacionesPorPatente,
        dataType: 'json',
        data: { patente: patente },
        type: "GET",
        success: function (data) {
            manejarRespuestaExitosa(data);
        },
        error: function (xhr, status, error) {
            const regex = /<h2>(.*?)<\/h2>/;
            const err = xhr.responseText.match(regex);
            //"Error en la petición AJAX: " + status + " - " +
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
    $("#NumeroOrdenExterno").append($("<option></option>").attr("value", "0").text("(Ninguno)"));

    if (Array.isArray(data.Data)) {
        data.Data.forEach(function (value) {
            $("#NumeroOrdenExterno").append($("<option></option>").attr("value", value.Id).text(value.Id.toString().padStart(8, '0')));
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
                MostrarAlertaError("Error en la petición AJAX: " + status + " - " + error);
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
    let url = window.location.search;
    let urlParams = new URLSearchParams(url);
    let workflowId = urlParams.get("workflow");
    return {
        clienteCUIT: selectedElement.CUITCliente,
        transportistaCUIT: selectedElement.CUITTransporte,
        patente: selectedElement.PatenteChasis,
        acoplado: selectedElement.PatenteAcoplado,
        materialSAP: selectedElement.CodigoProducto,
        ordenId: selectedElement.Id,
        workflow: workflowId
    };
}

function manejarRespuestaAjaxSeleccion(data, selectedElement) {
    if (data.TieneAdvertencias) {
        MostrarAlertaAdvertencia(data.Mensajes[0].Mensaje);
    }

    if (!data.EsValido) {
        MostrarAlertaError(data.Mensajes[0].Mensaje);
        return;
    }

    rellenarCampos(data, selectedElement);
}

function rellenarCampos(data, selectedElement) {
    domicilioConcat = `${selectedElement.DomicilioTipo}-${selectedElement.DomicilioOrden}`;

    $("#PatenteAcoplado").val(selectedElement.PatenteAcoplado);
    $("#ClienteId").val(data.Data.ClienteId);
    $("#Cliente").val(data.Data.ClienteDescripcion);
    $("#TransportistaId").val(data.Data.TransportistaId);
    $("#Transportista").val(data.Data.TransportistaDescripcion);
    $("#Chofer_Cuil").val(convertirCuil(selectedElement.CUILChofer));
    $("#MaterialId").val(data.Data.MaterialId);
    $("#TipoVehiculo").val(data.Data.TipoDeVehiculo);
    $("#KmARecorrer").val(selectedElement.KmARecorrer);
    $("#Destinatario").val(data.Data.Orden.RazonSocialDestinatario); /*DA*/
    $("#DestinatarioId").val(data.Data.Orden.CUITDestinatario);
    $("#IntermediarioFlete").val(data.Data.Orden.RazonSocialIntermediarioFlete);
    $("#IntermediarioFleteId").val(data.Data.Orden.CUITIntermediarioFlete);
    $("#LocalidadSeleccionada").val(data.Data.Orden.LocalidadId);
    $("#LocalidadDestinoId").val(data.Data.Orden.LocalidadId);

    //$("#PlantaDGDestino").val(data.Data.Orden.PlantaDGDestino);
    //$("#PlantaSeleccionada").val(data.Data.Orden.PlantaDGDestino);
    //$("#TipoYOrdenDestino").val(data.Data.Orden.TipoYOrdenDestino);
    $("#Corredor").val(data.Data.Orden.Corredor);

    //rellenar campos ocultos
    $("#Chofer_Cuil[type='hidden']").val(convertirCuil(selectedElement.CUILChofer));
    $("#PatenteAcoplado[type='hidden']").val(selectedElement.PatenteAcoplado);
    $("#MaterialId[type='hidden']").val(data.Data.MaterialId);
    $("#Chofer_NumeroDeDocumento[type='hidden']").val(selectedElement.CUILChofer.slice(2, -1));
    $("#KmARecorrer[type='hidden']").val(selectedElement.KmARecorrer);
   

    const select = document.getElementById('localidadDestinoDropdown');
    const option = document.createElement('option');

    // Asignar valor y texto
    option.value = data.Data.Orden.LocalidadId;
    option.text = data.Data.Orden.LocalidadDescripcion;
    option.selected = true;
    select.appendChild(option);
       
    let $element = $("#Chofer_Cuil");
    $element.trigger('focusout');
    ValidarDerivadoGranario();
    setTimeout(function () {
        var existeDomicilio = $("#TipoYOrdenDestino option[value='" + domicilioConcat + "']").end();
        if (existeDomicilio.length > 0) {
            $("#TipoYOrdenDestino").val(domicilioConcat);
        } else {
            MostrarAlertaAdvertencia('El domicilio recibido no coincide con los datos de Scato, verifique ó elija uno correcto.');
        }
    }, 1000);
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


//$(document).ready(function () {
//    $("#PatenteCamion").autocomplete({
//        source: function (request, response) {

//            response([])

//            const regex1 = /^[A-Z]{3}\d{3}$/;  // Regex para formato ABC123
//            const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Regex para formato AB123CD

//            var patente = request.term;// $("#PatenteCamion").val();
//            if (regex1.test(patente) || regex2.test(patente)) {
//                $.ajax({
//                    url: $('#links').data().urlObtenerOrdenDeCargaOperacionesPorPatente,
//                    dataType: 'json',
//                    data: {
//                        patente: patente,
//                    },
//                    type: "GET",
//                    success: function (data) {

//                        if (data.TieneAdvertencias) {
//                            MostrarAlertaAdvertencia(data.Mensajes[0].Mensaje);
//                        }

//                        if (!data.EsValido) {
//                            MostrarAlertaError(data.Mensajes[0].Mensaje);
//                        } else {
//                            //response($.map(data.Data, function (item) {
//                            //    return {
//                            //        label: item.PatenteChasis + " " + item.Id,
//                            //        value: item.PatenteChasis
//                            //    }
//                            //}))
//                        }
//                    }
//                });
//            }
//        }
//    });
//});

function establecerDestinatario(data) {
    $('#DestinatarioId').val(data.DestinatarioId); // Establecer ID del destinatario
}





