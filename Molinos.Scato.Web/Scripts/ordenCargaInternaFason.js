$(document).ready(function () {
    $("#TipoVehiculo").css({ 'pointer-events': 'none', 'background-color': '#f0f0f0', 'color': '#666' });
    $("#Chofer_Cuil").prop("disabled", true);
    $("#KmARecorrer").prop('readonly', true);
    $("#Chofer_TipoDocumentoIdentidadId").prop('disabled', true);
    var esCamionDemorado = window.location.pathname.toLowerCase().includes("CamionDemorado".toLowerCase());
    var esModificarDocumento = window.location.pathname.toLowerCase().includes("ModificarDocumentoDeIngreso".toLowerCase());

    // Manejar click en Rechazar
    $("#dialogo-rechazar-fason-confirmar").click(function () {
        let valido = true;
        let motivo = $("#MotivoRechazoTexto").val();

        $("#error-rechazo-requerido").hide();
        $("#error-rechazo-largo").hide();

        if (motivo == '') {
            valido = false;
            $("#error-rechazo-requerido").show();
        } else if (motivo.length < 10) {
            valido = false;
            $("#error-rechazo-largo").show();
        }

        if (valido) {
            $("#Rechazado").val("True");
            $("#Demorado").val("False");
            $("#MotivoRechazo").val(motivo);
            //lleno campos requeridos
            if ($("#ClienteId").val() == "" || $("#ClienteId").val() == "0") $("#ClienteId").val(99999);
            if ($("#TransportistaId").val() == "" || $("#TransportistaId").val() == "0") $("#TransportistaId").val(99999);
            if ($("#TipoVehiculo").val() == "" || $("#TipoVehiculo").val() == null) $("#TipoVehiculo").val(0);
            if ($('#tiposComerciales').val() == "") $("#tiposComerciales").val(0);
            if ($("#NumeroOrden").val() == "") $("#NumeroOrden").val(0);
            if ($("#Chofer_Cuil").val() == "") $("#Chofer_Cuil").val("99-99999999-9");
            if ($("#Chofer_Cuil[type = 'hidden']").val() == "") $("#Chofer_Cuil[type = 'hidden']").val("99-99999999-9");
            if ($("#Chofer_NumeroDeDocumento").val() == "") $("#Chofer_NumeroDeDocumento").val("99999999");
            if ($("#Chofer_Nombre").val() == "") $("#Chofer_Nombre").val("a");
            if ($("#Chofer_Apellido").val() == "") $("#Chofer_Apellido").val("a");
            if ($("#NumeroOrdenExterno").val() == "") $("#NumeroOrdenExterno").val("99999");
            if ($("#Cliente").val() == "" || $("#Cliente").val() == "0") $("#Cliente").val("a");

            modalRechazarOrdenCargaInterna.close();
            $("#ordenCargaInternaFason-form").submit();
        }
    });

    $('#MaterialDemorado').on('change', function () {
        // Obtiene el valor seleccionado del select
        const valorSeleccionado = $(this).val();
        $('#MaterialDemorado').val(valorSeleccionado);
    });

    // Manejar click en Demorar 
    $("#dialogo-demorar-fason-confirmar").click(function () {
        const regex1 = /^[A-Z]{3}\d{3}$/;  // Formato ABC123
        const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Formato AB123CD
        let valido = true;
        let material = $("#MaterialDemorado").val();
        let motivo = $("#MotivoDemoraTexto").val();
        var patente = $("#PatenteCamion").val().toUpperCase();
        
        $("#error-demorado-patenteFormato").hide();
        $("#error-demorado-requerido").hide();
        $("#error-demorado-largo").hide();
        $("#error-material").hide();
               

        if (!motivo || motivo.trim() === '') {
            valido = false;
            $("#error-demorado-requerido").show();
        } else if (motivo.length < 10) {
            valido = false;
            $("#error-demorado-largo").show();
        }

        if (!material) {
            valido = false;
            $("#error-material").show();
        }

        if (patente && !regex1.test(patente) && !regex2.test(patente)) {
            valido = false;
            $("#error-demorado-patenteFormato").show();
        }

        if (valido) {
            $("#Demorado").val("True");
            $("#Rechazado").val("False");
            $("#MotivoDemora").val(motivo);
            $('input[type="hidden"][name="MaterialId"]').val(material);
            $('#dialogo-demorar-fason').modal('hide');
            //lleno campos requeridos
            if ($("#ClienteId").val() == "" || $("#ClienteId").val() == "0") $("#ClienteId").val(99999);
            if ($("#TransportistaId").val() == "" || $("#TransportistaId").val() == "0") $("#TransportistaId").val(99999);
            if ($("#TipoVehiculoLectura").val() == "" || $("#TipoVehiculoLectura").val() == null) $("#TipoVehiculoLectura").val(0);
            if ($("#TipoVehiculo").val() == "" || $("#TipoVehiculo").val() == null) $("#TipoVehiculo").val(0);
            if ($('#tiposComerciales').val() == "") $("#tiposComerciales").val(0);
            if ($("#NumeroOrden").val() == "") $("#NumeroOrden").val(0);
            if ($("#Chofer_Cuil").val() == "") $("#Chofer_Cuil").val("99-99999999-9");
            if ($("#Chofer_Cuil[type = 'hidden']").val() == "") $("#Chofer_Cuil[type = 'hidden']").val("99-99999999-9");
            if ($("#Chofer_NumeroDeDocumento").val() == "") $("#Chofer_NumeroDeDocumento").val("99999999");
            if ($("#Chofer_Nombre").val() == "") $("#Chofer_Nombre").val("a");
            if ($("#Chofer_Apellido").val() == "") $("#Chofer_Apellido").val("a");
            if ($("#NumeroOrdenExterno").val() == "") $("#NumeroOrdenExterno").val("99999");
            if ($("#Cliente").val() == "" || $("#Cliente").val() == "0") $("#Cliente").val("a");

            $("#ordenCargaInternaFason-form").submit();
        }
    });

    $('#LocalidadDestinoId').removeAttr('data-val');
    $('#LocalidadDestinoId').removeAttr('data-val-number');
    $('#LocalidadDestinoId').removeAttr('data-val-required');

    if (cargaCupoIdValue === "true" || (numeroOrderExterno !== "" || esCamionDemorado)) {
        $('.btn.btn-primary[type="submit"]').focus();

    } else {
        //Foco en primer elemento
        $("#ordenCargaInternaFason-form").find(':input:not([readonly]):enabled:visible:first').focus();
    }

    ChecKilometros();
    
    // Cuando se abre el modal de demorar
    $("#dialogo-demorar-fason").on("show", function () {
        // Obtener el MaterialId del campo oculto
        var materialId = $("#MaterialId").val();
        if (materialId) {
            // Preseleccionar el material en el dropdown
            $("#Material").val(materialId);
            $("#MaterialDemorado").prop("disabled", true);
        } else {
            $("#MaterialDemorado").prop("disabled", false);
        }

        if ($("#MotivoDemora").val() != "") {
            $("#MotivoDemoraTexto").val($("#MotivoDemora").val());
        }
    });

    $(".btnValidacionD[onclick*='dialogo-demorar-fason']").click(function (e) {
        e.preventDefault();
        var materialId = $("#MaterialId").val();
        if (materialId) {
            $("#Material").val(materialId);
        }
        $('#dialogo-demorar-fason').modal('show');
    });

    $.validator.addMethod("materialValido", function (value, element, params) {
        return materialesPermitidos.includes(parseInt(value));
    }, txtErrorMaterial);

    // Asocia la regla personalizada al campo MaterialId
    $("#MaterialId").rules("add", {
        materialValido: true
    });

    // Opcional: Validar cada vez que se cambie el valor del campo
    $("#MaterialId").change(function () {
        $(this).valid();
    });

    if (esCamionDemorado == true) {
        $("#PatenteCamion").prop("disabled", true);
    }

    if (esModificarDocumento) {
        CargarPlantas();
        CargarDomicilios();
    }

})


var cachedOrdenDeCargaOperaciones;
var domicilioConcat = "";
var $selectOption;


function seleccionarMaterial(nuevoMaterialId) {
    // Verificar si el nuevoMaterialId está en la lista de materialesPermitidos
    var materialValido = materialesPermitidos.some(material => parseInt(material.Value) === nuevoMaterialId);

    if (!materialValido) {
        // Mostrar error
        var campo = $("#MaterialId");
        campo.addClass("input-validation-error");
        var mensaje = txtErrorMaterial;

        var mensajeElemento = campo.next("span.field-validation-valid");
        mensajeElemento.text(mensaje).addClass("field-validation-error");
        var selectCampo = $("select#MaterialId");
        selectCampo.addClass("input-validation-error");

        $("#MaterialId[type='hidden']").val("");

    } else {
        // Limpiar errores si el material es válido
        var selectCampo = $("select#MaterialId");
        selectCampo.removeClass("input-validation-error");
        var campo = $("#MaterialId");
        campo.removeClass("input-validation-error");
        var mensajeElemento = campo.next("span.field-validation-valid");
        mensajeElemento.text("").removeClass("field-validation-error");
    }
} 

function obtenerOrdenDeCargaOperacionesPorPatente() {
    if ($("#NumeroOrdenExterno").is('input')) return;
    if ($("#PatenteCamion").val() === "") {
        limpiarCamposOrdenDeCargaOperaciones();
        return;
    };
    const regex1 = /^[A-Z]{3}\d{3}$/;  // Formato ABC123
    const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Formato AB123CD
    let url = window.location.search;
    let urlParams = new URLSearchParams(url);
    let workflowId = urlParams.get("workflow");
    let esCamionDemorado = window.location.pathname.toLowerCase().includes("CamionDemorado".toLowerCase());
    workflowId = esCamionDemorado ? workflowCodigo : workflowId;

    var patente = $("#PatenteCamion").val().toUpperCase();
    $("#NumeroOrdenExterno").empty();
    if (typeof hayError !== 'undefined' && hayError !== "True") {
        limpiarCamposOrdenDeCargaOperaciones();
    }

    if (patente && !regex1.test(patente) && !regex2.test(patente)) {
        ValidarDerivadoGranario();
        return;
    }

    let tipoComercial = $('#tiposComerciales').find('option').eq(1).val();
    $('#tiposComerciales').find('select').val(tipoComercial);
    $("#TipoComercialId[type='hidden']").val(tipoComercial);

    BlockUI();
    $.ajax({
        url: $('#links').data().urlObtenerOrdenDeCargaOperacionesPorPatente,
        dataType: 'json',
        data: { patente: patente, workflow: workflowId, esDemorado: esCamionDemorado, ordenId: numeroOrderExterno },
        type: "GET",
        success: function (data) {
            manejarRespuestaExitosa(data);
        },
        error: function (xhr, status, error) {
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function manejarRespuestaExitosa(data) {
    var selectedValue = '';

    if (!data || (!data.Data && !Array.isArray(data.Mensajes)) || (!Array.isArray(data.Data) && data.Mensajes.length === 0)) {
        mostrarInfoAlerta();
        return;
    }

    if (data.TieneAdvertencias) {
        MostrarAlertaInfo(data.Mensajes[0].Mensaje);
    }

    if (!data.EsValido) {
        MostrarAlertaError(data.Mensajes[0].Mensaje);
        return;
    }

    cachedOrdenDeCargaOperaciones = data.Data;
    $("#NumeroOrdenExterno").append($("<option></option>").attr("value", "").text("(Ninguno)"));
    var materialesDerivadoGranario = JSON.parse($("#ListaMaterialesDerivadoGranario").val())
    seleccionarMaterial($("#MaterialId[type='hidden']").val());

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
        if (numeroOrderExterno !== "") {
            selectedValue =  numeroOrderExterno;
        } else {
            selectedValue = $("#NumeroOrdenExterno").data('selected-value');
        }
        
        if (data.Data.length === 1 && selectedValue !== undefined && selectedValue !== null) {
            $("#NumeroOrdenExterno").val(data.Data[0].Id);
            seleccionarOrdenDeCargaOperaciones();
        } else if (selectedValue) {
            $("#NumeroOrdenExterno").val(selectedValue);
            seleccionarOrdenDeCargaOperaciones();
        } else if (data.Data.length > 1) {
            MostrarAlertaInfo(textoVariasOrdenes);
        }
    }

}


function mostrarInfoAlerta() {
    MostrarAlertaAdvertencia(patenteNoEncontrada);
    ValidarDerivadoGranario()
    limpiarCamposOrdenDeCargaOperaciones()
}


function seleccionarOrdenDeCargaOperaciones(ordenOperaciones = "") {
    var ddlNumeroOrden = $("#NumeroOrdenExterno");
    var selectedElement = ordenOperaciones ? obtenerElementoSeleccionado(ordenOperaciones) : obtenerElementoSeleccionado(ddlNumeroOrden.val());

    if (!selectedElement) {
        limpiarCamposOrdenDeCargaOperaciones();
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
    $("#LocalidadDestinoDescripcion[type = 'hidden']").val(data.Data.Orden.LocalidadDescripcion);
    $("#DestinoGranario").val(data.Data.DestinoDescripcion);
    $("#DestinoGranarioId").val(data.Data.DestinoId);
    $("#Corredor").val(data.Data.Orden.Corredor);
    $("#Observaciones").val(data.Data.Orden.Observacion);
    $("#Remitente").val(data.Data.Orden.RemitenteComercial);
    $("#PagadorFlete").val(data.Data.Orden.PagadorFlete);
    $("#LocalidadDescripcion").val(data.Data.Orden.LocalidadDescripcion);
    $('#DestinatarioId').val(data.Data.Orden.DestinatarioId);
    $('#PagadorFleteId').val(data.Data.Orden.PagadorFleteId);
    $('#KmARecorrer').val(data.Data.Orden.KmARecorrer);

    //rellenar campos ocultos
    $("#Chofer_Cuil[type='hidden']").val(convertirCuil(selectedElement.CUILChofer));
    $("#PatenteAcoplado[type='hidden']").val(selectedElement.PatenteAcoplado);
    $("#MaterialId[type='hidden']").val(data.Data.MaterialId);
    $("#Chofer_NumeroDeDocumento[type='hidden']").val(selectedElement.CUILChofer.slice(2, -1));
    $("#PlantaSeleccionada[type='hidden']").val(data.Data.Orden.PlantaCodigo);
    $("#PlantaDGDestino[type='hidden']").val(data.Data.Orden.PlantaCodigo);
    $("#OrdenDomicilioDestino[type='hidden']").val(data.Data.Orden.DomicilioOrden);
    $("#TipoDomicilioDestino[type='hidden']").val(data.Data.Orden.DomicilioTipo);
    $("#DestinoMercaderia[type='hidden']").val(data.Data.Orden.DestinoMercaderia);
    $("#TipoComercialId[type='hidden']").val(tipoComercial)
    $("#RemitenteId[type='hidden']").val(data.Data.Orden.RemitenteComercialId)

    if (data.Data.TieneErrorCNRT) {
        $('#TipoVehiculo').css({ 'pointer-events': '', 'background-color': '', 'color': '' });
        $("#TipoVehiculo").val("");
    } else {
        $('#TipoVehiculo').css({ 'pointer-events': 'none', 'background-color': '#f0f0f0', 'color': '#666' });
    }

    seleccionarMaterial(data.Data.MaterialId);
    $("#MaterialDemorado").val(data.Data.MaterialId);

    ChecKilometros();

    if (numeroOrderExterno !== "") {
        $("#NumeroOrdenExterno").val(numeroOrderExterno);
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
    let esDerivadoGranario = materialesDerivadoGranario.includes(parseInt(materialId));

    if (esDerivadoGranario) {
        $('#DerivadoGranarioHabilitado').val('true');
        $('.derivadoGranario').removeClass('hidden');
        $("label[for='Cliente']").text('Destino');
        $("#Cliente").val($("#DestinoGranario").val());
        $("#ClienteId").val($("#DestinoGranarioId").val());

        CargarPlantas();
        CargarDomicilios();
    } else {
        $("#Cliente").val($("#ClienteOriginal").val());
        $("#ClienteId").val($("#ClienteOriginalId").val());
        $("#DerivadoGranarioHabilitado").val("false");
        $(".derivadoGranario").addClass('hidden');
        $("label[for='Cliente']").text('Cliente');
        $("#PlantaDGDestino").val("");
        $("#TipoYOrdenDestino").val("");
        $("#PagadorFlete").val("");
        $("#PagadorFleteId").val("");
        $("#PlantaSeleccionada").val("");
    }
}

function limpiarCamposOrdenDeCargaOperaciones() {
    $("#PatenteAcoplado").val(null);
    $("#NumeroOrdenExterno").val(null);
    $("#ClienteId").val(null);
    $("#Cliente").val(null);
    $("#ClienteOriginal").val(null);
    $("#ClienteOriginalId").val(null);
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
    $("#Destinatario").val(null); /*DA*/
    $("#DestinoGranario").val(null)
    $("#DestinoGranarioId").val(null)
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
    $("#DestinoMercaderia").val(null);
    $("#LocalidadDescripcion").val(null);
    $('#DestinatarioId').val(null);
    $('#PagadorFleteId').val(null);
    $('#KmARecorrer').attr('readonly', true);
    $("#MaterialDemorado").val(null);
    $('input[style*="border-width: 1px 5px 1px 1px; border-style: solid; border-color: green; border-image: initial;"], select[style*="border-width: 1px 5px 1px 1px; border-style: solid; border-color: green; border-image: initial;"]').removeAttr('style');
    ValidarDerivadoGranario();
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


function CargarPlantas() {
    let plantaSeleccionada = $("#PlantaSeleccionada").val();
    let cliente = $("#DestinoId").val() != null ? $("#DestinoId").val() : $("#ClienteId").val();
    if ($("#DerivadoGranarioHabilitado").val().toLowerCase() === 'true' && cliente.length > 0) {
        $.getJSON($('#links').data().urlObtenerPlantasPorCliente, { clienteId: cliente },
            function (allData) {
                let options = '<option value="">(nro. planta)</option>';
                $('#PlantaDGDestino').html(options);
                if (!allData.HayErrores) {
                    for (let i = 0; i < allData.Plantas.length; i++) {
                        options += `<option value="${allData.Plantas[i]}">Planta Nro. ${allData.Plantas[i]}</option>`;
                    }
                    $('#PlantaDGDestino').html(options);
                    if (plantaSeleccionada.length > 0 && allData.Plantas.includes(parseInt(plantaSeleccionada))) {
                        $('#PlantaDGDestino').val(parseInt(plantaSeleccionada))
                    }
                } else {
                    MostrarAlertaError("Error al consultar Plantas - " + allData.Errores["2"]);
                }
            }
        );
    }
}

function CargarDomicilios() {
    let ordenDomicilioSeleccionado = $("#OrdenDomicilioDestino").val();
    let tipoDomicilioSeleccionado = $("#TipoDomicilioDestino").val();
    let cliente = $("#DestinoId").val() != null ? $("#DestinoId").val() : $("#ClienteId").val();
    if ($("#DerivadoGranarioHabilitado").val().toLowerCase() === 'true' && cliente.length > 0) {
        $.getJSON($('#links').data().urlObtenerDomiciliosDerivadoGranarioPorCliente, { clienteId: cliente },
            function (allData) {
                let options = '<option value="">(domicilio)</option>';
                $('#TipoYOrdenDestino').html(options);

                if (!allData.HayErrores) {
                    for (let i = 0; i < allData.Domicilios.length; i++) {
                        options += `<option value="${allData.Domicilios[i].Tipo}-${allData.Domicilios[i].Orden}">(${allData.Domicilios[i].Tipo} - ${allData.Domicilios[i].Orden}) ${allData.Domicilios[i].Descripcion}</option>`;
                    }
                    $('#TipoYOrdenDestino').html(options);
                    if (ordenDomicilioSeleccionado.length > 0
                        && tipoDomicilioSeleccionado.length > 0
                        && allData.Domicilios.some(domicilio => domicilio.Orden == ordenDomicilioSeleccionado && domicilio.Tipo == tipoDomicilioSeleccionado)) {
                        $('#TipoYOrdenDestino').val(`${tipoDomicilioSeleccionado}-${ordenDomicilioSeleccionado}`)
                        $("#TipoYOrdenDestino[type = 'hidden']").val(`${tipoDomicilioSeleccionado}-${ordenDomicilioSeleccionado}`);
                        $('#TipoYOrdenDestino').attr('title', $('#TipoYOrdenDestino :selected').text());
                    }
                } else {
                    MostrarAlertaError("Error al consultar Domicilios - " + allData.Errores["2"]);
                }
            }
        );
    }
}

function ChecKilometros() {
    var kmaRecorrer = $("#KmARecorrer").val();
    if (kmaRecorrer === '') kmaRecorrer = $("#KmARecorrer[type='hidden']").val();

    if (!kmaRecorrer || kmaRecorrer == '0') {
        $('#KmARecorrer').removeAttr('readonly');
    } else {
        $('#KmARecorrer').attr('readonly', true);
    }
}



