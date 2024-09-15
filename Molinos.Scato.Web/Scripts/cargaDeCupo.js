const materialInicial = [{ value: '', text: '(Material)' }];
const [getMaterial, setMaterial] = useState([]);
const [getMaterialInput, setMaterialInput] = useState(null)
document.addEventListener("DOMContentLoaded", function (event) {
    activarCPE();
    var notificaLectura = $.connection.notificaLectura;

    notificaLectura.client.informarLectura = function (notificacion) {        
        $("#validation-patente-alert").addClass("hide");
        $("#validation-patente-danger").addClass("hide");
        if (!notificacion.TarjetaValida && !notificacion.EsTarjetaSupervisor) {
            $("#validation-patente").html(notificacion.MensajeError);
            $("#validation-patente-alert").removeClass("hide");
            $("#Numero").val('');
        } else if (notificacion.EsTarjetaSupervisor) {
            $("#validation-danger").html(notificacion.MensajeError);
            $("#validation-patente-danger").removeClass("hide");
            $("#Numero").val('');
        } else if (notificacion.NumeroDeTarjeta !== null && notificacion.NumeroDeTarjeta !== "") {
            $("#Numero").val(notificacion.NumeroDeTarjeta);
            
            if (ValidarCP()) {
                if (ValidarNumeroTarjeta()) {
                    TomarFotoCP();
                }
            }
        }
        $('#NumeroCartaPorte').focus();
    };

    notificaLectura.client.informarEstadoConexion = function (notificacion) {
        if (notificacion.Estado) {
            $("#labelConectado").addClass('hidden');
            $("#labelDesconectado").removeClass('hidden');
        } else {
            $("#labelConectado").removeClass('hidden');
            $("#labelDesconectado").addClass('hidden');
        }
    };

    BlockUI($("#cargando").val());
    // Start the connection
    try {
        var puestoDeTrabajo = null;
        if ($("#puestoDeTrabajo").val() !== "") {
            puestoDeTrabajo = JSON.parse($("#puestoDeTrabajo").val());
        }

        window.hubReady.done(function () {
            if (puestoDeTrabajo != null &&  puestoDeTrabajo.Automatico) {
                notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), puestoDeTrabajo.Id);
            }
            $.unblockUI();
        }).fail(function (error) {
            window.location.href = window.location.href;
        });
    }
    catch (err) {
        window.location.href = window.location.href;
    }
    //////////////


    var cupoValido = false;
    $.validator.addMethod("cupoValidacion", function (value, element) {
        return cupoValido || $('#checkSinCupo').is(':checked');
    }, '');
    lastValue = '';
    setInterval(function () {
        if ($("#Cupo").val() !== lastValue) {
            lastValue = $("#Cupo").val();
            cupoValido = false;
            $("#validation-cupo").addClass("hide");
            $("#validation-cupo").removeClass("alert-block");
            $("#validation-cupo").removeClass("alert-success");
            $("#validation-cupo").removeClass("alert-info");
            $("#validation-cupo").removeClass("alert-error");
            var cupo = $('#Cupo').val();
            if (/^MOL[0-9]{4}\/[0-9]{8}$/.test(cupo) && !$('#checkSinCupo').is(':checked')) {
                BlockCupos($("#ValidandoCupo").val());
                var request = {
                    cupo: cupo,
                    imagen: $('#ImagenCartaPorte').val(),
                    nroCartaPorte: $('#CTG').val()
                }
                $.ajax({
                    type: 'POST',
                    dataType: "json",
                    url: $("#ValidarCupoUrl").val(),
                    data: request,
                    success: function (data) {
                        if (data.error !== undefined) {
                            $("#validation-cupo-body").html("<strong>" + data.error + "</strong>");
                            $("#validation-cupo").removeClass("hide");
                            $("#validation-cupo").addClass("alert-error");
                            $("#Cupo").focus();
                        } else {
                            var especial = "";
                            if (data.model.Especial && data.model.MaterialId == 4) {
                                especial = " Sustentable";
                                SetearFotoCP(data.PdfImageSustentableBase64 ? "" : "error", data.PdfImageSustentableBase64, $("#CodigoCamaraCPDir").val(), true);
                            }
                            else if (data.model.Especial) {
                                especial = " Especial";
                            }
                            $("#validation-cupo-body").html("<h4><strong>" + data.model.RespuestaSap + "</strong></h4>  Fecha: <strong>" + data.model.FechaSap + "</strong>  Material: <strong>" + data.model.MaterialDescripcion + especial + "</strong>  Proveedor: <strong>" + data.model.ProveedorDescripcion + "(" + data.model.ProveedorCuit + ")</strong>");
                            $("#validation-cupo").removeClass("hide");
                            $("#validation-cupo").addClass(data.model.RespuestaSap == 'Cupo del día' ? "alert-success" : data.model.RespuestaSap == "Cupo vencido" ? "alert-block" : data.model.RespuestaSap == "Cupo futuro" ? "alert-info" : "");
                            cupoValido = true;
                            $("#MaterialId").val(data.model.MaterialId);
                            $("#FechaSap").val(data.model.FechaSap);
                            $("#Especial").val(data.model.Especial);
                            $("#RespuestaSap").val(data.model.RespuestaSap);
                            $("#Camara").val(data.model.Camara);
                            if ($("#cupoValidation").text() === '') {
                                $("#btnAceptar").focus();
                            }
                        }
                    },
                    error: function (error) {
                    },
                }).always(function () {
                    UnblockCupos();
                });
            }
        }
    }, 800);


    $('#NumeroCartaPorte').focus();
    TomarFotoConPatente();

    $('#Patente').change(validarEgresoVentaFas)
    $('#circuitoNoGranos').change(validarEgresoVentaFas)

    $("#validation-ventaFas-close").on("click", function () {
        $("#validation-ventaFas-error").addClass("hide");
        return false;
    });
    if ($("#matId").val() != $("#MaterialId").val())
        $("#matId").val("");

    setMaterialInput(document.getElementById('MaterialId'))

    $.unblockUI();
    //OptenerSelectOptionsValues()
});




var patenteNoReconocida = 'Patente no reconocida';
var iniciarLoopFotoPatenteActivo = false;
function OptenerSelectOptionsValues() {
    var material = getMaterialInput().options;
    const materialSelect = materialInicial.concat(
        Array.from(material).map(x => ({ value: x.value, text: x.text }))
    );
    setMaterial(materialSelect);
}
function TomarFotoConPatente() {
    if ($('#checkvalidarPatente').is(':checked') && !iniciarLoopFotoPatenteActivo) {
        RefrescarFotoPatente();
    }
}

function RefrescarFotoPatente() {
    iniciarLoopFotoPatenteActivo = true;
    $.ajax({
        url: $("#links").data().urlObtenerPatente,
        dataType: 'json',
        data: { puestodetrabajoid: $("#PuestoDeTrabajoId").val(), codigoCamara: $("#CodigoCamaraPatente").val(), directorio: $("#CodigoCamaraPatenteDir").val() },
        type: "GET",
        success: function (data) {
            if (data.error === "") {
                if (data.patente !== 'NULL') {
                    $("#patenteALPR").html(data.patente);
                    if ($('#circuitoNoGranos').is(':checked') && !$('#Patente').val()) {
                        $('#Patente').val(data.patente);
                        validarEgresoVentaFas();
                    }
                    if ($('#cpe').is(':checked') && !$('#Patente').val()) {
                        $('#Patente').val(data.patente);
                    }
                } else {
                    $("#patenteALPR").text(patenteNoReconocida);
                }
                $('#imagen-patente').attr('src', data.imagen);
                $('#imagen-patente').attr('alt', "Cargando...");
            } else {
                $("#patenteALPR").html('');
                $('#imagen-patente').attr('alt', "Error al obtener la imagen");
                $('#imagen-patente').attr('src', '');
            }
            ValidarPatentesIguales();
        },
        complete: function (data) {
            if ($('#checkvalidarPatente').is(':checked')) {
                setTimeout(RefrescarFotoPatente, 3000);
            } else {
                iniciarLoopFotoPatenteActivo = false;
            }
        }
    });
}

function validarEgresoVentaFas() {
    const existePatenteYesNoGranos = $('#Patente').val().length > 0 && $('#circuitoNoGranos').is(':checked')
    if (existePatenteYesNoGranos) {
       // DefinirFlujoFasFason($('#Patente').val());
        DefinirFlujoInsumos($('#Patente').val())
    }

}

async function DefinirFlujoFasFason(patente) {
    const respuesta = await ObtenerDatosFason(patente);
    if (!respuesta) {
        ObtenerDatosSap();
    }
}

async function DefinirFlujoInsumos(patente) {
    const respuesta = await ObtenerDatosInsumos(patente);
    if (!respuesta) {
        ObtenerDatosSap();
    }
}

async function ObtenerDatosFason(patente) {

    desactivarInput(getMaterialInput());
    $('#FleteMOA').val("");
    BlockUI($("#MensajeBuscandoDatos").val());

    return new Promise((resolve, reject) => {
        $.getJSON($("#links").data().urlObtenerOrdenesFason, { patente: patente }, function (data) {
            if (typeof data.errorResponse === 'object') {
                if (data.errorResponse.duplicado === true || data.errorResponse.duplicado === false) {
                    crearRespuestaErrorFason(data.errorResponse);
                    resolve(false);
                }
            }

            if (data.sonVariosMateriales) {
                llenarMateriales(data, false, false);
                resolve(true);
            } else if (typeof data.ordenes === 'object' && data.ordenes.length > 0) {
                llenarMateriales(data, true);
                resolve(true);
            } else {
                limpiarComboMateriales();
                resolve(false);
            }
        }).fail(function (xhr, status, error) {
            reject(error); // Manejo de errores
        }).always(function () {
            $.unblockUI();
        });
    });
}

async function ObtenerDatosInsumos(patente) {

    desactivarInput(getMaterialInput());
    $('#FleteMOA').val("");
    BlockUI($("#MensajeBuscandoDatos").val());

    return new Promise((resolve, reject) => {
        $.getJSON($("#links").data().urlObtenerOrdenesInsumos, { patente: patente }, function (data) {
            if (typeof data.errorResponse === 'object') {
                if (data.errorResponse.duplicado === true || data.errorResponse.duplicado === false) {
                    crearRespuestaErrorFason(data.errorResponse);
                    resolve(false);
                }
            }
            if (data.sonVariosMateriales) {
                llenarMateriales(data, false, false);
                resolve(true);
            } else if (typeof data.ordenes === 'object' && data.ordenes.length > 0) {
                llenarMateriales(data, true);
                resolve(true);
            } else if (typeof data.ordenesInsumos === 'object' && data.ordenesInsumos.length > 0) {
                llenarMateriales(data, true);
                resolve(true);
            }
            else {
                limpiarComboMateriales();
                resolve(false);
            }
        }).fail(function (xhr, status, error) {
            reject(error); // Manejo de errores
        }).always(function () {
            $.unblockUI();
        });
    });
}

function crearRespuestaErrorFason(data) {
    let message = "";
    if (data.duplicado == false) {
        message = data.error ;
    } else {
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
        return false;
    }

    $("#validation-fason").html("<strong>" + "Fason. " + message + "</strong>");
    $("#validation-fason-error").removeClass("hide");
}

function limpiarComboMateriales() {
    setMaterial(materialInicial)
}

function llenarMateriales(data, comboDisable, preSeleccionable = true) {
    let value = "";
    let materialSelect = [];
    llenarFleteMoa(data.ordenes);

    if (comboDisable && preSeleccionable) {
        if (data.ordenes.length > 0) {
            value = data.ordenes[0].CodigoProducto;
        } else if (data.ordenesInsumos.length > 0) {
            value = data.ordenesInsumos[0].CodigoProducto;
        }
    }

    materialSelect = [
        ...data.ordenes.map(x => ({ value: x.CodigoProducto, text: x.DescripcionProducto })),
        ...data.ordenesInsumos.map(x => ({ value: x.CodigoProducto, text: x.DescripcionProducto }))
    ];

    materialInputState.options = materialSelect;
    setMaterial(materialSelect);

    ajustarFleteMoa(value);

    if (!comboDisable) {
        activarInput(getMaterialInput());
        ajustarFleteMoa();
    }
}

function onFailure(xhr, status, error) {
    
    $.unblockUI();
    $('#dialogo-confirmar').modal('hide');
    MostrarAlertaError("Error en flujo material no productivo");
}

function onSuccess(response) {
    if (response && response.workflow && response.cargaDeCupoId) {
        const workflow = encodeURIComponent(response.workflow);
        const cargaDeCupoId = encodeURIComponent(response.cargaDeCupoId);

        const url = new URL('/Scato.Web/IngresarOrdenCargaInterna', window.location.origin);
        const params = new URLSearchParams({
            workflow: workflow,
            cargaDeCupoId: cargaDeCupoId
        });

        url.search = params.toString();
        window.location.href = url.toString();
    } else {
        console.error('El objeto respuesta no tiene las propiedades obligatorias.');
    }
}

function ObtenerDatosSap() {
    if ($('#Patente').val().length == 0) {
        //$('input').attr('disabled', 'disabled');
        //$('select').attr('disabled', 'disabled');
        //$('#Patente').removeAttr('disabled');
    } else {
        BlockUI($("#MensajeBuscandoDatos").val());
        $.getJSON($("#Patente").data().numeroUrl, { numero: $('#Patente').val() }, function (data) {
            if (data.datosSap && data.datosSap != -1 && $('#MaterialId')) {
                
                if (data.datosSap.length == 1) {
                    
                    $('#MaterialId').val(data.datosSap[0].MaterialId);
                    $('#matId').val(data.datosSap[0].MaterialId);
                } else if (data.datosSap.length > 1) {
                    $("#validation-ventaFas").html("<strong>La patente tiene más de una orden creada, al aceptar el camion debe dirigirse a mesa FAS</strong>");
                    $("#validation-ventaFas-error").removeClass("hide");
                }

            }
        }).complete(function () {
            $.unblockUI();
        });
    }
}

function activarCPE() {
    $('#cpe').prop('checked', true);
    $('#cpe').val(true)
    $('#cpe').trigger("change");
    $('.check-cpe').hide()
}

function useState(initialValue) {
    let value = initialValue;
    function setValue(newValue) {
        value = newValue;
    }
    return [() => value, setValue];
}
function llenarSelect(element, options) {
    element.innerHTML = '';
    options.forEach(function (opt) {
        const option = document.createElement("option");
        option.value = opt.value;
        option.textContent = opt.text
        element.appendChild(option);
    });
}
function activarInput(element) {
    element.disabledbled = false;
}
function desactivarInput(element) {
    element.disabledbled = true;
}

// Handler para observar cambios en materialInput
const materialHandler = {
    set(target, property, value) {
        target[property] = value; // Actualizamos el valor
        // Si cambiamos las opciones, renderizamos nuevamente el select
        if (property === 'options') {
            llenarSelect(getMaterialInput(), value);
        }
        return true;
    }
};

// Creación del proxy que manejará el estado de materialInput
const materialInputState = new Proxy({ options: materialInicial }, materialHandler);

