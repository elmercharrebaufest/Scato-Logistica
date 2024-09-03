var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');
$('#FechaEmision').mask(formatoFecha);
$("#FechaEmision").datepicker();

var cachedOrdenDeCargaOperaciones;
var domicilioConcat = "";
var $selectOption;
const listarProveedores = $('#links').data().urlBuscarProveedores;
const obtenerProveedor = $('#links').data().urlBuscarProveedor;
const obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

document.addEventListener("DOMContentLoaded", function (event) {
    init();
});

//Initial Options
const ordenInicial = [{ value: '', text: '(Ninguno)' }];
const localidadInicial = [{ value: '', text: '(Localidad)' }];
const plantaInicial = [{ value: '', text: '(nro. planta)' }];
const domicilioInicial = [{ value: '', text: '(Domicilio)' }];
const almacenInicial = [{ value: '', text: '(Almacen)' }];
const materialGoma = 6427;

const ordenModel = {
    patenteCamion: "",
    ordenes: [],
    selectedOrden: "",
    localidades: [],
    selectedLocalidad: "",
    plantas: [], 
    selectedPlanta: "",
    domicilios: [],
    selectedDomicilio: "",
    patenteAcopado: "",
    transportista: "",
    transportistaId: "",
    chofer_Cuil: "",
    materialId: "",
    tipoVehiculo: "",
    destino: "",
    destinoId: "",
    plantaDg: "",
    destinatario: "",
    intermediarioFlete : "",
    corredor: "",
    tipoComerciales: [],
    selectedTipoComercial: "",
    almacenes: [],
    selectedAlmacen: "",
    fechaEmision: "",
    kmARecorrer: "",
    subscribers: []
};

const patenteCamionInput = document.getElementById("PatenteCamion");
const patenteAcopladoInput = document.getElementById("PatenteAcoplado");
const transportistaInput = document.getElementById("Transportista");
const transportistaIdInput = document.getElementById("TransportistaId");
const chofer_CuilInput = document.getElementById("Chofer_Cuil");
const materialIdInput = document.getElementById("MaterialId");
const tipoVehiculoInput = document.getElementById("TipoVehiculo");
const destinoInput = document.getElementById("Destino");
const destinoIdInput = document.getElementById("DestinoId");
const destinatarioInput = document.getElementById("Destinatario");
const intermediarioFleteInput = document.getElementById("IntermediarioFlete");
const localidadSeleccionadaInput = document.getElementById("LocalidadSeleccionada");
const corredorInput = document.getElementById("Corredor");
const tipoComercialInput = document.getElementById("TipoComercialId");
const fechaEmisionInput = document.getElementById("FechaEmision");
const kmARecorrerInput = document.getElementById("KmARecorrer");


const ordenSelect = document.getElementById("NumeroOrdenExterno");
const localidadSelect = document.getElementById("localidadDestinoDropdown");
const plantaSelect = document.getElementById("PlantaDGDestino");
const domicilioSelect = document.getElementById("TipoYOrdenDestino");
const almacenSelect = document.getElementById("Almacen_Id");


function init()
{
    llenarSelectOrdenes(ordenInicial);
    llenarSelectLocalidad(localidadInicial);
    llenarSelectPlanta(plantaInicial);
    llenarSelectDomicilio(domicilioInicial);
    llenarSelectAlmacen(almacenInicial);
}

// Llenar el select con opciones

function llenarSelectOrdenes(ordenes)
{
    ordenSelect.innerHTML = '';
    ordenes.forEach(function (orden) {
        const option = document.createElement("option");
        option.value = orden.value;
        option.textContent = orden.text
        ordenSelect.appendChild(option);
    });
}

function llenarSelectLocalidad(localidades) {

    localidadSelect.innerHTML = '';
    localidades.forEach(function (localidad) {
        const option = document.createElement("option");
        option.value = localidad.value;
        option.textContent = localidad.text
        localidadSelect.appendChild(option);
    });
}

function llenarSelectPlanta(plantas) {

    plantaSelect.innerHTML = '';
    plantas.forEach(function (planta) {
        const option = document.createElement("option");
        option.value = planta.value;
        option.textContent = planta.text
        plantaSelect.appendChild(option);
    });
}

function llenarSelectDomicilio(domicilios) {

    domicilioSelect.innerHTML = '';
    domicilios.forEach(function (domicilio) {
        const option = document.createElement("option");
        option.value = domicilio.value;
        option.textContent = domicilio.text
        domicilioSelect.appendChild(option);
    });
}

function llenarSelectAlmacen(almacenes) {

    almacenSelect.innerHTML = '';
    almacenes.forEach(function (almacen) {
        const option = document.createElement("option");
        option.value = almacen.value;
        option.textContent = almacen.text
        almacenSelect.appendChild(option);
    });
}

function llenarInput(value, element , att) {
    element.innerHTML = '';
    element.value = value;
    att = value;
}

function llenarInputDate(value, element, att) {

    var datePattern = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    datePattern = datePattern.replace(/[A-Z]/g, '9');
    // Asumir que el valor es en formato aaaa/mm/dd
    const dateSinHora = value.split(' ')[0];
    const dateParts = dateSinHora.split('/');
    if (dateParts.length === 3) {
        const [day, month, year] = dateParts;

        // Reemplazar en el patrón global usando los valores
        const formattedDate = datePattern
            .replace('99', day.padStart(2, '0'))
            .replace('99', month.padStart(2, '0'))
            .replace('9999', year);

        // Asignar el valor formateado al elemento de entrada
        element.innerHTML = '';
        element.value = formattedDate;
        att = formattedDate;
    }
}
function seleccionarElemento(value, element, att) {
    element.value = value;
    att = value;
}

function activarInput(element)
{
    element.disabledbled = false;
}
function desactivarInput(element)
{
    element.disabledbled = true;
}

function ObtenerPlantas() {
    const cliente = $("#DestinoId").val() || $("#ClienteId").val();
    const derivadoGranarioHabilitado = $("#DerivadoGranarioHabilitado").val().toLowerCase() === 'true';

    if (derivadoGranarioHabilitado && cliente) {
        $.getJSON($('#links').data().urlObtenerPlantasPorCliente, { clienteId: cliente })
            .done(function (allData) {

                if (allData && !allData.HayErrores && Array.isArray(allData.Plantas)) {
                    const plantasSelect = plantaInicial.concat(allData.Plantas.map(planta => ({ value: planta, text: `Planta Nro. ${planta}` })))
                    llenarSelectPlanta(plantasSelect);
                } else {

                    llenarSelectPlanta(plantaInicial);
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.error('Error en la solicitud para obtener plantas', textStatus, errorThrown);
            });
    }
}

function ObtenerAlamacenesPorMaterial()
{
    const material = $("#MaterialId").val();

    if (!material) {
        return;
    }
    $.getJSON($('#links').data().urlObtenerAlmacenesPorMaterial, { materialId: material })
        .done(function (allData) {
            if (Array.isArray(allData) && allData.length > 0) {
                const almacenesSelect = almacenInicial.concat(allData.map(almacen => ({ value: almacen.Id, text: almacen.Descripcion })))

                llenarSelectAlmacen(almacenesSelect);

            } else {

                llenarSelectAlmacen(almacenInicial);
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.error('Error en la solicitud para obtener almacenes', textStatus, errorThrown);
            llenarSelectAlmacen(almacenInicial);
        })

}

function validarPatenteCamion() {
    const regex1 = /^[A-Z]{3}\d{3}$/;  // Regex para formato ABC123
    const regex2 = /^[A-Z]{2}\d{3}[A-Z]{2}$/;  // Regex para formato AB123CD
    let url = window.location.search;
    let urlParams = new URLSearchParams(url);
    let workflowId = urlParams.get("workflow");

    var patente = ordenModel.patenteCamion.toUpperCase();

    if (!regex1.test(patente) && !regex2.test(patente)) {
        return;
    }
    else {
        servicioObtenerOrden(patente, workflowId)
    }
}

function servicioObtenerOrden(patente, workflowId) {
    //let tipoComercial = $('#tiposComerciales').find('option').eq(1).val();
   
    //$('#tiposComerciales').find('select').val(tipoComercial)
    //$("#TipoComercialId[type='hidden']").val(tipoComercial)
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

    if (Array.isArray(data.Data)) {

        const ordenesSelect = ordenInicial.concat(data.Data.map(x => ({ value: x.Id, text: x.Id.toString().padStart(8, '0') })))

        llenarSelectOrdenes(ordenesSelect);
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
    //ValidarDerivadoGranario()
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

function ValidarDerivadoGranario() {
    let materialId = $("#MaterialId").val();
    let materialesDerivadoGranario = JSON.parse($("#ListaMaterialesDerivadoGranario").val())
    if (materialesDerivadoGranario.includes(parseInt(materialId))) {
        $('#DerivadoGranarioHabilitado').val('true')
        $('.derivadoGranario').removeClass('hidden');
        $("label[for='Cliente']").text('Destino');
        $("#Cliente").val($("#DestinoGranario").val());
        $("#ClienteId").val($("#DestinoGranarioId").val());

        //CargarPlantas();
        CargarDomicilios();
    } else {
        $("#Cliente").val($("#ClienteOriginal").val());
        $("#ClienteId").val($("#ClienteOriginalId").val());
        $('#DerivadoGranarioHabilitado').val('false')
        $('.derivadoGranario').addClass('hidden');
        $("label[for='Cliente']").text('Cliente');
        //$('#PlantaDGDestino').val('');
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
    $("#MaterialId").val(null);
    $("#TipoVehiculo").val(null);
    $("#Destinatario").val(null);
    $("#TipoVehiculo").val(null);
    $("#Destinatario").val(null); /*DA*/
    $("#Remitente").val(null);
    $("#TipoComercialId").val(null);
    $("#IntermediarioFlete").val(null);
    $("#LocalidadDestinoId").val(null);
   // $("#PlantaDGDestino").val(null);
    $("#TipoYOrdenDestino").val(null);
    $("#PagadorFlete").val(null);
    $("#Corredor").val(null);
    $("#localidadDestinoDropdown").empty();
    $("#Observaciones").val(null);
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
                        $('#TipoYOrdenDestino').attr('title', $('#TipoYOrdenDestino :selected').text());
                    }
                }
            }
        );
    }
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

function rellenarCampos(data, selectedElement) {
    ObtenerAlamacenesPorMaterial();
    ObtenerPlantas();
    domicilioConcat = `${selectedElement.DomicilioTipo}-${selectedElement.DomicilioOrden}`;

    llenarInput(selectedElement.PatenteAcoplado, patenteAcopladoInput, ordenModel.patenteAcoplado)
    llenarInput(data.Data.TransportistaId, transportistaIdInput, ordenModel.transportistaId)
    llenarInput(data.Data.TransportistaDescripcion, transportistaInput, ordenModel.transportista)
    llenarInput(convertirCuil(selectedElement.CUILChofer), chofer_CuilInput, ordenModel.chofer_Cuil)
    seleccionarElemento(data.Data.MaterialId, materialIdInput, ordenModel.materialId)
    seleccionarElemento(data.Data.TipoDeVehiculo, tipoVehiculoInput, ordenModel.tipoVehiculo)
    llenarInput(data.Data.DestinoDescripcion, destinoInput, ordenModel.destino)
    llenarInput(data.Data.DestinoId, destinoIdInput, ordenModel.destinoId)
    llenarInput(data.Data.Orden.LocalidadId, localidadSeleccionadaInput, ordenModel.localidadSelect)
    seleccionarElemento(data.Data.Orden.PlantaCodigo, plantaSelect, ordenModel.selectedPlanta)
    data.Data.TieneErrorCNRT ? activarInput(tipoVehiculoInput) : desactivarInput(tipoVehiculoInput)
    seleccionarElemento(data.Data.MaterialId === materialGoma ? 2 : 6, tipoComercialInput, ordenModel.selectedTipoComercial)
    //console.log(plantaSelect)
    seleccionarElemento(data.Data.AlmacenId, almacenSelect, ordenModel.selectedAlmacen)
    llenarInputDate(data.Data.Orden.FechaCreacion, fechaEmisionInput, ordenModel.fechaEmision)
    llenarInput(data.Data.Orden.KmARecorrer, kmARecorrerInput, ordenModel.kmARecorrer)

    const select = document.getElementById('localidadDestinoDropdown');
    const option = document.createElement('option');

    // Asignar valor y texto
    option.value = data.Data.Orden.LocalidadId;
    option.text = data.Data.Orden.LocalidadDescripcion;
    option.selected = true;
    select.appendChild(option);

    //let elementos = ["#Chofer_Cuil", "#Cliente", "#Destinatario", "#Transportista", "#IntermediarioFlete", "#PagadorFlete"];
    let elementos = [chofer_CuilInput , transportistaInput ];

    elementos.forEach(function (selector) {
        let $element = $(selector);

         //Solo disparar eventos si el campo no está vacío
        if ($element.val().trim() !== "") {
            $element.trigger('keydown').trigger('focusout');
            $element.blur()
            //setTimeout(() => $element.blur(), 100);
        }
    });
   
    if (destinoIdInput.value.length > 0) {

        DefinirAutocompletarConSAP(
            '#Destino',
            '#DestinoId',
            '#autocompleteDestino',
            $('#links').data().urlBuscarClientes,
            $('#links').data().urlBuscarClienteUnico,
            $('#links').data().urlObtenerClientesSap,
            function () {
                validarClienteNoBloqueado();
                cargarMaterial();
                CargarPlantas();
                CargarDomicilios();
            },
            function () {
                deshabilitarKmRecorrerYLocalidad();
                cargarMaterial();
            }
        );
    }


    setTimeout(function () {
        var existeDomicilio = $("#TipoYOrdenDestino option[value='" + domicilioConcat + "']").end();
        //$('#KmARecorrer').val($('#localidadDestinoDropdown :selected').data('kilometros'));
        //$('#PlantaDGDestino').val(data.Data.Orden.PlantaCodigo);

        if (existeDomicilio.length > 0) {
            $("#TipoYOrdenDestino").val(domicilioConcat);
        } else {
            MostrarAlertaAdvertencia('El domicilio recibido no coincide con los datos de Scato, verifique ó elija uno correcto.');
        }
    }, 2000);
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

//Listerners 
ordenSelect.addEventListener("change", function (event) {
    ordenModel.selectedOrden = event.target.value;  
});

localidadSelect.addEventListener("change", function (event) {
    ordenModel.selectedLocalidad = event.target.value;
});

tipoComercialInput.addEventListener("change", function (event) {
    ordenModel.selectedTipoComercial = event.target.value;
    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    if (!$('#Transportista').hasClass('transportistaRequerido')) ValidarObjeto($("#orden-form"), $("#Transportista"));
});

function notifySubscribers() {
    ordenModel.subscribers.forEach(function (callback) {
        callback();
    });
}

function subscribe(callback) {
    ordenModel.subscribers.push(callback);
}

subscribe(function () {
    validarPatenteCamion()
});

patenteCamionInput.addEventListener("input", function (event) {
    ordenModel.patenteCamion = event.target.value;
    notifySubscribers();
});
