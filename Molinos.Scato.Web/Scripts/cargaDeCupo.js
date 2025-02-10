jQuery(document).ready(function ($) {
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
            if (puestoDeTrabajo != null && puestoDeTrabajo.Automatico) {
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
    $(document).on('change', '#Patente', validarEgresoVentaFas);
    $("#formCargaDeCupo").submit(function(event) {
        let materialValido = ValidarMaterialNoGranoSeleccionado();
        if (!materialValido) {
            event.stopPropagation();
            event.preventDefault();
        }
    });
});
var patenteNoReconocida = 'Patente no reconocida';

var iniciarLoopFotoPatenteActivo = false;
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
    let checkNoGranosActivo = document.getElementById('circuitoNoGranos').checked;
    const patente = document.getElementById('Patente').value.trim().toUpperCase();
    if (checkNoGranosActivo && EsPatenteValida(patente)) {
        ObtenerDatosFasonInsumos(patente);
    }
}

function EsPatenteValida(patente) {
    const regex = /^[A-Z]{3}[0-9]{3}$|^[A-Z]{2}[0-9]{3}[A-Z]{2}$/;
    return regex.test(patente);
}


function ObtenerDatosFasonInsumos(patente) {
    BlockUI($("#MensajeBuscandoDatos").val());

    $.getJSON($("#links").data().urlObtenerOrdenesInsumos, { patente: patente }, function (data) {
        LimpiarComboMateriales(false);
        if (data.HayErrores) {
            if (data.Errores["ClienteDuplicado"]) {
                MostrarErrorPopup(data.Errores["ClienteDuplicado"]);
            } else {
                MostrarErrorLabel(data.Errores["Error"]);
            }
        } else {
            CompletarMaterialesPorOrdenNoGranos(data.Ordenes)
        }
    }).fail(function (xhr, status, error) {
        // Manejo de errores
    }).always(function () {
        $.unblockUI();
    });
}

function MostrarErrorPopup(errorMessage) {
    $("#dialogo-advertir-body").html(`<strong>${errorMessage}</strong>`);
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
}

function MostrarErrorLabel(errorMessage) {
    $("#validation-error-message").html(`<strong>${errorMessage}</strong>`);
    $("#validation-error-alert").removeClass("hide");
}

function LimpiarComboMateriales(valorPorDefecto) {
    $('#MaterialId').empty();
    if (valorPorDefecto) {
        $('#MaterialId').append('<option value="">(material)</option>');
    }
}

function activarCPE() {
    $('#cpe').prop('checked', true);
    $('#cpe').val(true)
    $('#cpe').trigger("change");
    $('.check-cpe').hide()
}

function CompletarMaterialesPorOrdenNoGranos(ordenesNoGranos) {
    if (ordenesNoGranos.length > 1) {
        $('#MaterialId').append('<option value="">(material)</option>');
        $("#HayVariosMateriales").val("true");
    } else {
        $("#HayVariosMateriales").val("false");
    }
    $.each(ordenesNoGranos, function (index, orden) {
        $("#MaterialId").append(`<option value=${orden.MaterialId} data-tipo-carga=${orden.TipoOrden}>${orden.MaterialDescripcion}</option>`);
    });

    ValidarMaterialNoGranoSeleccionado();
}