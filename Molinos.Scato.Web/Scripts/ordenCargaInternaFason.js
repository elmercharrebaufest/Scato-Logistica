var cachedOrdenDeCargaOperaciones;

function obtenerOrdenDeCargaOperacionesPorPatente() {
    const regex1 = /^[A-Z]{3}\d{3}$/;  // Regex para formato ABC123
    const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Regex para formato AB123CD

    var patente = $("#PatenteCamion").val();
    $("#NumeroOrden").empty();
    limpiarCamposOrdenDeCargaOperaciones();

    if (regex1.test(patente) || regex2.test(patente)) {
        BlockUI();
        $.ajax({
            url: $('#links').data().urlObtenerOrdenDeCargaOperacionesPorPatente,
            dataType: 'json',
            data: {
                patente: patente,
            },
            type: "GET",
            success: function (data) {

                if (data.TieneAdvertencias) {
                    MostrarAlertaAdvertencia(data.Mensajes[0].Mensaje);
                }

                if (!data.EsValido) {
                    MostrarAlertaError(data.Mensajes[0].Mensaje);
                } else {
                    cachedOrdenDeCargaOperaciones = data.Data;
/*                    $("#NumeroOrden").append($("<option></option>").attr("value", null).text("Seleccionar"));*/
                    $.each(data.Data, function (key, value) {
                        $("#NumeroOrden").append($("<option></option>").attr("value", value.Id).text(value.Id.toString().padStart(8, '0')));
                    });


                    if (data.Data.length == 1) {
                        $("#NumeroOrden").val(data.Data[0].Id);
                        seleccionarOrdenDeCargaOperaciones();
                    } else {
                        MostrarAlertaAdvertencia("Se encontró más de una orden de carga asociada a la patente, por favor seleccione una en Nº de Orden de Carga.");
                    }
                }
            },
            complete: function () {
                $.unblockUI();
            }
        });

    }
};

function seleccionarOrdenDeCargaOperaciones() {
    var ddlNumeroOrden = $("#NumeroOrden");
    var selectedElement = cachedOrdenDeCargaOperaciones.filter(x => x.Id == ddlNumeroOrden.val())[0]
    BlockUI();
    $.ajax({
        url: $('#links').data().urlObtenerOrdenDeCargaOperacionesSeleccionada,
        dataType: 'json',
        data: {
            clienteCUIT: selectedElement.CUITCliente,
            transportistaCUIT: selectedElement.CUITTransporte,
            patente: selectedElement.PatenteChasis,
            acoplado: selectedElement.PatenteAcoplado,
            materialSAP: selectedElement.CodigoProducto
        },
        type: "GET",
        success: function (data) {

            if (data.TieneAdvertencias) {
                MostrarAlertaAdvertencia(data.Mensajes[0].Mensaje);
            }

            if (!data.EsValido) {
                MostrarAlertaError(data.Mensajes[0].Mensaje);
            } else {
               

                $("#PatenteAcoplado").val(selectedElement.PatenteAcoplado);

                $("#ClienteId").val(data.Data.ClienteId);
                $("#Cliente").val(data.Data.ClienteDescripcion);

                $("#TransportistaId").val(data.Data.TransportistaId);
                $("#Transportista").val(data.Data.TransportistaDescripcion);
       
                
                $("#Chofer_Cuil").val(convertirCuil(selectedElement.CUILChofer));
                $("#MaterialId").val(data.Data.MaterialId);

                $("#TipoVehiculo").val(data.Data.TipoDeVehiculo);

                $("#KmARecorrer").val(selectedElement.KmARecorrer);

                let $element = $("#Chofer_Cuil");
                $element.trigger('focusout');
                
            }
        }, complete: function () {
            $.unblockUI();
        }
    });


 
};

function limpiarCamposOrdenDeCargaOperaciones() {
    $("#PatenteAcoplado").val(null);
    $("#NumeroOrden").val(null);
    $("#ClienteId").val(null);
    $("#Cliente").val(null);
    $("#TransportistaId").val(null);
    $("#Transportista").val(null);
    $("#Chofer_Cuil").val(null);
    $("#MaterialId").val(null);
    $("#TipoVehiculo").val(null);
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





