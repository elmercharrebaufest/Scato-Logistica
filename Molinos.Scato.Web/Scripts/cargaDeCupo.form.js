var errorPatente = false;
var errorInhabilitacion = false;
sinAfip = false;
sinCupo = false;
sinFotoCartaPorte = false;

$(document).ready(function () {
    activarCPE();

    var notificaLectura = $.connection.notificaLectura;

    notificaLectura.client.informarLecturaCpe = function (notificacion) {
        console.log("Notificacion : ", notificacion);
        if (notificacion.NroCtg !== null && notificacion.NroCtg !== "") {
            if ($('#cpe').is(':checked') && !$('#CTG').val()) {
                $("#CTG").val("");
                $('#CTG').focus();
                $("#CTG").val(notificacion.NroCtg);
                if ($("#CTG").val().length == 11 || $("#CTG").val().length == 12) {
                    $('#CTG').trigger('change');
                    $('#Cupo').focus();
                }
            }
        }
    };

    $.unblockUI();

    $('#dialogo-confirmar').modal('hide');
    $('#dialogo-advertir').modal('hide');
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();

    $(document).on('click', '.confirmar-boton', function () {
        var mensaje = "";
        if (errorInhabilitacion) {
            mensaje = "El vehículo se encuentra inhabilitado";
        }
        if (errorPatente) {
            if (mensaje !== "") {
                mensaje += " y ";
            }
            mensaje += $('#circuitoNoGranos').is(':checked') ? "La patente leida en la imagen no coincide con indicada en el formulario" : "La patente leida en la imagen no coincide con la obtenída de AFIP";
        }
        if (errorPatente || errorInhabilitacion) {
            $("#dialogo-confirmar-body").text(mensaje + ", desea confirmarlo de todas formas?");
            $('#dialogo-confirmar').css({
                'top': '30%',
                'margin-left': function () {
                    return -($(this).width() / 2);
                },
                'left': '50%',
                'margin-top': function () {
                    return -($(this).height() / 2.6);
                }
            });
            $("#dialogo-confirmar").modal('show');
            return false;
        }
    });


    lastValue = '';
    $('#checkSinCupo').change(function () {
        if ($('#checkSinCupo').is(':checked')) {
            $("#Cupo").val("MOL1111/11111111");
            $('#divSpan6').removeClass('error');
            $('#Cupo').valid();
        } else {
            $("#Cupo").val("");
        };
    });
    $("#Cupo").on('keydown', function () {
        if ($("#Cupo").val() !== "MOL1111/11111111") {
            $('#checkSinCupo').prop('checked', false);
        }
    });

    var today = new Date();
    var dd = today.getDate().toString();
    var mm = today.getMonth() + 1;
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    today = dd + mm + yyyy;

    $("#Cupo").inputmask("MOL9999/99999999", { "placeholder": "MOL____/" + today });

    $("#validation-fason-close").on("click", function () {
        $("#validation-fason-error").addClass("hide");
        return false;
    });
    $("#validation-patente-close").on("click", function () {
        $("#validation-patente-alert").addClass("hide");
        return false;
    });
    $("#validation-danger-close").on("click", function () {
        $("#validation-patente-danger").addClass("hide");
        return false;
    });

    $("#validation-cupo-close").on("click", function () {
        $("#validation-cupo").addClass("hide");
        return false;
    });
    $("#validation-ctg-close").on("click", function () {
        $("#validation-ctg-alert").addClass("hide");
        return false;
    });
    var puestoDeTrabajo = null;

    if ($("#puestoDeTrabajo").val() !== "") {
        puestoDeTrabajo = JSON.parse($("#puestoDeTrabajo").val());

        sinAfip = puestoDeTrabajo.SinAfip;
        sinCupo = puestoDeTrabajo.SinCupo;
        sinFotoCartaPorte = puestoDeTrabajo.SinFotoCartaPorte;
        $('#ImprimeCartaPorte').val(puestoDeTrabajo.ImprimeCartaPorte);
        $('#ImprimeTarjetaDeAcceso').val(puestoDeTrabajo.ImprimeTarjetaDeAcceso);
        $('#NoAsignaCalleEnGaritaEntrada').val(puestoDeTrabajo.NoAsignaCalleEnGaritaEntrada);
        $('#SinFotoCartaPorte').val(puestoDeTrabajo.SinFotoCartaPorte);
        if (sinCupo) {
            $('#checkSinCupo').prop('checked', true);
            $('#checkSinCupo').change();
        }
        if (sinFotoCartaPorte) {
            $('.divFotoCP').addClass('hide');
        }
        $('#NumeroCartaPorte').focus();
    }
    if (puestoDeTrabajo === null || !puestoDeTrabajo.Automatico) {
        $("#labelConectado").addClass('hidden');
        $("#labelDesconectado").addClass('hidden');
        $("#Numero").removeAttr('readonly');
        $("#Numero").attr("placeholder", "");
    }
    if (puestoDeTrabajo !== null) {
        $("#PuestoDeTrabajoId").val(puestoDeTrabajo.Id);
    }


    if ($.cookie("checkvalidarPatente") !== undefined) {
        $('#checkvalidarPatente').prop('checked', true);
        TomarFotoConPatente();
    } else {
        $("#marco-patente").addClass('hide');
    }

    $('#checkvalidarPatente').change(function () {
        BlockUI();
        if ($('#checkvalidarPatente').is(':checked')) {
            $.cookie("checkvalidarPatente", 1);
            $("#marco-patente").removeClass('hide');
            TomarFotoConPatente();
        } else {
            $.removeCookie("checkvalidarPatente");
            $("#marco-patente").addClass('hide');

        };
        setTimeout($.unblockUI, 500);
    });

    if ($("#ImagenCartaPorte").val() != "") {
        $("#imagen-cp").attr("src", $("#ImagenCartaPorte").val());
    }

    $('#NumeroCartaPorte').change(function () {
        if (ValidarCP()) {
            if (ValidarNumeroTarjeta()) {
                TomarFotoCP();
            }
            cargarCupoPorCTG();
        }
    });
    $('#NumeroCartaPorte').on('keydown', function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });
    $("#Cupo").keypress(function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });
    $('.tomarFoto1').click(function () {
        if (ValidarCP()) {
            if (ValidarNumeroTarjeta()) {
                TomarFotoCP();
            }
        }
        if ($('#cpe').is(':checked')) {
            TomarFotoCP();
        }
    });

    ListarCupos();

    $('#circuitoNoGranos').change(function () {
        if ($('#circuitoNoGranos').is(':checked')) {
            ConfiguracionNoGranosActiva();
        } else {
            ConfiguracionNoGranosInactiva();
        };
    });
    if ($('#circuitoNoGranos').is(':checked')) {
        $('#circuitoNoGranos').trigger('click');
    }

    $('#Numero').change(function () {
        if ($('#Numero').val() && $('#circuitoNoGranos').is(':checked') && $('#checkvalidarPatente').is(':checked')) {
            TomarFotoConPatente();
        }
    });

    $('#cpe').change(function () {
        $('#MaterialId').prop('disabled', false);
        $('#checkSinCupo').prop('disabled', false);
        setTimeout(function () {
            $('#CTG').focus();
        }, 100);
        if ($('#cpe').is(':checked')) {
            ConfiguracionCPEActiva(true);
        } else {
            ConfiguracionCPEInactiva(true);
        };
    });

    $("#MaterialId").change(function () {
        $("#MaterialId").val($("#MaterialId").val());
        ajustarFleteMoa($("#MaterialId").val())
    });

    $('#CTG').rules('remove', 'required');
    $('#CTG').removeAttr("minlength", "11");
    $('#CTG').removeAttr("maxlength", "12");

    //$('#CTG').change(function () { 
    //    if (ValidarCPE() && $('#cpe').is(':checked')) {
    //        if (ValidarCtgCpe()) {
    //            TomarFotoCP();
    //        }
    //    }
    //});

    if ($('#cpe').is(':checked')) {
        ConfiguracionCPEActiva(false);
    } else if ($('#circuitoNoGranos').is(':checked')) {
        ConfiguracionNoGranosActiva();
    } else {
        ConfiguracionNoGranosInactiva();
        ConfiguracionCPEInactiva(false);
    }

    $('#CTG').change(function () {
        var nroCTG = $('#CTG').val();
        var tarjeta = $('#Numero').val();
        var esEpecial = $('#Especial').val();
        if ((nroCTG.length == 11 || nroCTG.length == 12) && $.isNumeric(nroCTG) && $('#cpe').is(':checked')) {
            BlockCupos($("#MensajeBuscandoCartaPorte").val());
            $.getJSON($("#links").data().urlObtenerCpe, { numeroCtg: nroCTG, tarjeta: tarjeta, esEpecial: esEpecial}, function (data) {
                if (data.CodigoDeError == 1) {
                    MostrarAlertaInfo(data.Error);
                } else if (data.CodigoDeError == 3 || data.CodigoDeError == 4) {
                    if (data.Cpe) {
                        if (data.Cpe.Vehiculos.length > 0) {
                            $("#Patente").val(data.Cpe.Vehiculos[0]["Patente"]);
                            if (!data.Cpe.Vehiculos[0]["PatenteAcoplado"]) {
                                MostrarAlertaAdvertencia("Vehiculo sin acoplado");
                            }
                        }
                        if (data.Cpe.MaterialId != "" || data.Cpe.MaterialId != null || data.Cpe.MaterialId != undefined) {
                            $("#MaterialId").val(data.Cpe.MaterialId);
                        }
                        if (!data.Cpe.Cupo == null || !data.Cpe.Cupo == '') {
                            $("#Cupo").val(validacionLongitudCupoAFIP(data.Cpe.Cupo));
                        }
                        if (!data.Cpe.RtteComercialCodigoSap == null || !data.Cpe.RtteComercialCodigoSap == '') {
                            $("#RtteComercialCodigoSap").val(data.Cpe.RtteComercialCodigoSap);
                        }
                        if (!data.Cpe.TitularCartaPorteCodigoSap == null || !data.Cpe.TitularCartaPorteCodigoSap == '') {
                            $("#TitularCartaPorteCodigoSap").val(data.Cpe.TitularCartaPorteCodigoSap);
                        }
                    }

                    if (data.CodigoDeError == 4) {
                        MostrarAlertaAdvertencia(data.Error);
                    }

                    $('#checkSinCupo').prop('checked', false);
                    if (esEpecial == "true") {
                        SetearFotoCP(data.PdfImageSustentableBase64 ? "" : "error", data.PdfImageSustentableBase64, $("#CodigoCamaraCPDir").val(), true);
                    } else {
                        SetearFotoCP(data.PdfImageBase64 ? "" : "error", data.PdfImageBase64, $("#CodigoCamaraCPDir").val(), false);
                    }
                } else {
                    MostrarAlertaError(data.Error);
                }
            }).complete(function () {
                UnblockCupos();
            });
        }
    });

    if ($('#imagen-cp').attr('src') == "" ) {
        setTimeout(function () {
            $.removeData($('imagen-cp'), 'elevateZoom');
            $('.zoomContainer').remove();
        }, 200);
    }

});

let blockui = [];
let dataFleteMoa = new Map();

function llenarFleteMoa(data) {
    if (Object.keys(dataFleteMoa).length >= 0) {
        dataFleteMoa.clear();
    }

    data.forEach(item => {
        dataFleteMoa.set(item.CodigoProducto, item.FleteMOA);
    });
}

function ajustarFleteMoa(materialId = "") {
    let valor = dataFleteMoa.get(materialId);
    $('#FleteMOA').val(valor);
    $('#matId').val(materialId);
}

function BlockCupos(msg) {
    if (blockui.length == 0) {
        BlockUI(msg);
    }
    blockui.push(1);
    console.log("B:" + blockui.length);
}

function UnblockCupos() {
    blockui.pop(1);
    console.log("U:" + blockui.length);
    if (blockui.length == 0) {
        $.unblockUI();
    }
}

function ValidarCP() {
    var numeroCartaPorte = $('#NumeroCartaPorte').val();
    return numeroCartaPorte.length == 12 && $.isNumeric(numeroCartaPorte);
}

function ValidarNumeroTarjeta() {
    return ValidarObjeto($("form"), $("#Numero"));
}

function cargarCupoPorCTG() {
    if (sinAfip) {
        $("#btnAceptar").focus();
        return;
    }
    var numeroCartaPorte = $('#NumeroCartaPorte').val();
    BlockCupos($("#MensajeBuscandoCartaPorte").val());
    $.getJSON($("#links").data().urlObtenerCartaPorteCtg, { numeroCartaPorte: numeroCartaPorte }, function (data) {
        console.log(data);
        $("#validation-ctg").html("");
        errorInhabilitacion = false;
        $("#validation-ctg-alert").addClass("hide");
        if (data.CodigoDeError === "0" || data.CodigoDeError === 1) {
            cargarCP(data);
        } else if (data.CodigoDeError === "2") {
            MostrarAlertaInfo("AFIP no respondió el número de CUPO. Debe cargarlo manualmente: " + data.Error);
        } else if (data.CodigoDeError === "3") {
            cargarInhabilitacion(data);
            cargarCP(data);
        } else {
            MostrarAlertaError(data.Error);
        }
        ValidarPatentesIguales();
    }).complete(function () {
        UnblockCupos();
    });
}

function TomarFotoCP() {
    if (!sinFotoCartaPorte) {
        BlockCupos($("#MensajeBuscandoCartaPorte").val());
        $.getJSON($("#links").data().urlObtenerPatente, {
            puestodetrabajoid: $("#PuestoDeTrabajoId").val(), codigoCamara: $("#CodigoCamaraCP").val(), directorio: $("#CodigoCamaraCPDir").val(), fotoPatente: false, numero: $('#Numero').val(), numeroCartaPorte: $('#NumeroCartaPorte').val()
        }, function (data) {

            SetearFotoCP(data.error, data.imagen, data.directorio);

            //if (data.error === "") {
            //    $('#ImagenCartaPorte').val(data.imagen);
            //    $('#FotoRutaDestino').val(data.directorio);
            //    $('#imagen-cp').load(function () {
            //        UnblockCupos();
            //    }).attr('src', data.imagen);
            //    $('#imagen-cp').attr('alt', "Cargando...");
            //} else {
            //    $('#imagen-cp').attr('alt', "Error al obtener la imagen");
            //    $('#imagen-cp').attr('src', '');
            //    UnblockCupos();
            //}
            //$('.tomarFoto1').show();
        });
    }
}

function SetearFotoCP(error, imagen, directorio, esSustentable) {
    if (error === "") {
        if (esSustentable) {
            $('#ImagenCartaPorteSustentable').val(imagen);
        } else {
            $('#ImagenCartaPorte').val(imagen);
        }
        $('#FotoRutaDestino').val(directorio);
        $('#imagen-cp').load(function () {
            UnblockCupos();
        }).attr('src', imagen);
        $('#imagen-cp').attr('alt', "Cargando...");
        $('#imagen-cp').elevateZoom({
            zoomType: "inner",
            cursor: "crosshair"
        });
    } else {
        $('#imagen-cp').attr('alt', "Error al obtener la imagen");
        $('#imagen-cp').attr('src', '');
        UnblockCupos();
    }
    $('.tomarFoto1').show();
}

function getDateIfDate(d) {
    var m = d.match(/\/Date\((\d+)\)\//);
    return m ? (new Date(+m[1])).toLocaleDateString('es-es', { month: '2-digit', day: '2-digit', year: 'numeric' }) : d;
}

function cargarInhabilitacion(data) {
    errorInhabilitacion = true;
    var error = "<strong>Vehículo(" + data.CartaPorte.Patente + ") inhabilitado :</strong>";
    $.each(data.Error, function (k, v) {
        error += "<div>Desde " + getDateIfDate(v.FechaDesde) + " - Hasta: " + getDateIfDate(v.FechaHasta) + " Motivo : " + v.Motivo + "</div>";
    });
    $("#validation-ctg").html(error);
    $("#validation-ctg-alert").removeClass("hide");
}
function cargarCP(data) {
    if (!data.CartaPorte.Cupo) {
        MostrarAlertaAdvertencia("AFIP no respondió el número de CUPO. Debe ingresarlo manualmente");
    }
    $("#Cupo").val(data.CartaPorte.Cupo);
    $("#Patente").val(data.CartaPorte.Patente);

    $("#CTG").val(data.CartaPorte.CTG);
    $("#CodEstab").val(data.CartaPorte.CodEstab);
    $("#RtteComercialCodigoSap").val(data.CartaPorte.RtteComercialCodigoSap);
    $("#TitularCartaPorteCodigoSap").val(data.CartaPorte.TitularCartaPorteCodigoSap);
    $("#MaterialId").val(data.CartaPorte.MaterialId);
    $('#checkSinCupo').prop('checked', false);
    $("#btnAceptar").focus();
}

var patenteNoReconocida = 'Patente no reconocida';

function ValidarPatentesIguales() {
    errorPatente = false;
    if ($('#checkvalidarPatente').is(':checked')) {
        if ($("#patenteALPR").html() !== patenteNoReconocida &&
            $("#patenteALPR").html() !== '' &&
            $("#Patente").val() !== '' &&
            $("#patenteALPR").html().toUpperCase() !== $("#Patente").val().toUpperCase()) {
            if ($('#circuitoNoGranos').is(':checked')) {
                $("#validation-danger").html("<strong>La patente reconocida en la imagen (" + $("#patenteALPR").html().toUpperCase() + ") no coincide con :  (" + $("#Patente").val().toUpperCase() + ")</strong>");
            } else {
                $("#validation-danger").html("<strong>La patente reconocida en la imagen (" + $("#patenteALPR").html().toUpperCase() + ") no coincide con la obtenida de AFIP (" + $("#Patente").val().toUpperCase() + ")</strong>");
            }
            $("#validation-patente-danger").removeClass("hide");
            errorPatente = true;
        } else if (
            $("#patenteALPR").html() === patenteNoReconocida ||
            $("#patenteALPR").html() === '' &&
            $("#Patente").val() !== '') {
            $("#validation-danger").html("<strong>No se pudo reconocer la patente del vehículo en la imagen, debe validarla manualmente</strong>");
            $("#validation-patente-danger").removeClass("hide");
            errorPatente = true;
        } else {
            $("#validation-patente-danger").addClass("hide");
            $("#validation-danger").html('');
        }
    }
}

function ListarCupos() {
    $.ajax({
        url: $('#links').data().urlListar,
        data: { },
        type: "GET",
        success: function (data) {
            $('#listaCupos').html(data);
        },
        complete: function (data) {
            setTimeout(ListarCupos, 4000);
        }
    });
}


function crearOpcionesCaracteristicasDeCalidad(esGrano) {

    const url = $('#links').data().urlObtenerMaterial;

    BlockUI("Cargando Material...");
    $.getJSON(
        url,
        { esGrano: esGrano },
        function (resultado) {
            $('#MaterialId').each(function () {
                var value = $(this).val();

                var combo = $(this);
                combo.empty();
                combo.append($('<option/>', {
                    value: "",
                    text: "(material)",
                    selected: true
                }));
                $.each(resultado, function (index, data) {
                    combo.append($('<option/>', {
                        value: data.Value,
                        text: data.Text,
                        selected: data.Value == value
                    }));
                });
                combo.val(value);
            });
        }).complete(function () {
            $.unblockUI();
        });
}

function DisabledControlers(status) {
    $('#checkvalidarPatente').prop('checked', true);
    $('#checkvalidarPatente').trigger("change");
    $('#checkSinCupo').prop('checked', status);
    $('[name="SinCupo"]').val(status);
    //$('#checkvalidarPatente').prop('disabled', status);
    $('#checkSinCupo').prop('disabled', status);
    $('#Cupo').prop('readonly', status);
    $('#Cupo').prop('readonly', status);
    $("#CTG").val("");
    $("#NumeroCartaPorte").val("");
}

function DisableControlersCPE(status, clearinpunts) {
    $('#Patente').prop('readonly', true);
    //$('#circuitoNoGranos').prop('disabled', status);
    $('#circuitoNoGranos').prop('checked', false);
    $('#NumeroCartaPorte').attr('disabled', status);
    $("#NumeroCartaPorte").val("");
    $('#Cupo').prop('readonly', false);
    if (clearinpunts) {
        $("#CTG").val("");
        $("#CUITSolicitante").val("");
        $("#Patente").val("");
        $("#Cupo").val("");
    }
}

function clearValidation() {
    $('.validation-summary-errors').hide();
    $(".error").removeClass("error");
    $(".field-validation-error").html("");
    $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
}

function isNumberKey(e) {
    if (e.keyCode == '9' || e.keyCode == '16') {
        return;
    }
    var code;
    if (e.keyCode) code = e.keyCode;
    else if (e.which) code = e.which;
    if (e.which == 46)
        return false;
    if (code == 8 || code == 46)
        return true;
    if (code < 48 || code > 57)
        return false;
}

function ValidarCPE() {
    var ctg = $('#CTG').val();
    return ctg.length >= 11 && $.isNumeric(ctg);
}

function ValidarCtgCpe() {
    return ValidarObjeto($("form"), $("#CTG"));
}

function ConfiguracionCPEActiva(clear) {
    $('#CTG').rules('add', 'required');
    $('#CTG').attr("minlength", "11");
    $('#CTG').attr("maxlength", "12");
    $.validator.messages.minlength = 'El campo CTG debe tener una longitud mínima de 11';
    $.validator.messages.maxlength = 'El campo CTG debe tener una longitud máxima de 12';
    DisableControlersCPE(true, clear);
    $('#CTG').prop('readonly', false);
    $('#NumeroCartaPorte').prop('required', false);
    clearValidation();
    $('#imagen-cp').attr('src', '');
    $('#checkSinCupo').prop('checked', true);
    $("#Cupo").val("MOL1111/11111111");
    $('#divSpan6').removeClass('error');
    $('#Cupo').valid();
    $('#Patente').prop('readonly', false);
    $("label[for*='Patente']").text("Patente");
}

function ConfiguracionCPEInactiva(clear) {
    DisableControlersCPE(false, clear);
    setTimeout(function () {
        $('#NumeroCartaPorte').focus();
    }, 100);
    $('#CTG').prop('readonly', true);
    $('#NumeroCartaPorte').prop('required', true);
    $('#CTG').rules('remove', 'required');
    $('#CTG').removeAttr("minlength", "11");
    $('#CTG').removeAttr("maxlength", "12");
    clearValidation();
    //$("#imagen-cp").attr("src", $("#ImagenCartaPorte").val());
    $("#ImagenCartaPorte").val("");
    $('#imagen-cp').attr('src', '');
    $('#checkSinCupo').prop('checked', false);
    $('#Patente').prop('readonly', true);
    $("label[for*='Patente']").text("Patente AFIP");
}

function ConfiguracionNoGranosActiva() {
    crearOpcionesCaracteristicasDeCalidad(false);
    $('#NumeroCartaPorte').attr('disabled', true);
    $('#CTG').attr('disabled', true);
    $('#checkSinCupo').attr('checked', true);
    $('[name="SinCupo"]').val(true);
    $("#Cupo").val("MOL1111/11111111");
    $('#divSpan6').removeClass('error');
    $("label[for*='Patente']").text("Patente");
    $('#Patente').prop('readonly', false);
    $("#Patente").val("");
    $('#MaterialId').prop('disabled', true);
    $('#Patente').focus();
    $('#cpe').prop('disabled', true);
    DisabledControlers(true);
    clearValidation();
}

function ConfiguracionNoGranosInactiva() {
    crearOpcionesCaracteristicasDeCalidad(true);
    //$('#NumeroCartaPorte').attr('disabled', false);
    $('#CTG').attr('disabled', false);
    $('#checkSinCupo').attr('checked', false)
    $('[name="SinCupo"]').val(false);
    $("#Cupo").val("");
    $("label[for*='Patente']").text("Patente AFIP");
    $('#Patente').prop('readonly', true);
    $("#Patente").val("");
    $('#MaterialId').prop('disabled', false);
    $('#MaterialId').val("");
    $('#matId').val("");
    $('#cpe').prop('disabled', false);
    DisabledControlers(false);
    clearValidation();
}

function validacionLongitudCupoAFIP(cupo) {
    let resultado = '';

    try {
        if (cupo.length > 16) {
            let split = cupo.split("/");
            let fechaCupo = '';
            if (split.length > 0) {
                let split0 = split[0];
                let split1 = split[1];

                if (split0 != null || split0 != undefined) {
                    if (split0.length > 7) {
                        resultado = split0.substring(0, 7);
                    } else {
                        resultado = split0;
                    }
                }

                if (split1 != null || split1 != undefined) {
                    if (split1.length > 8) {
                        fechaCupo = split1.substring(0, 8);
                    } else {
                        fechaCupo = split1;
                    }
                }

                resultado = resultado + '/' + fechaCupo;
                if (resultado.length > 16) {
                    resultado = resultado.substring(0, 16);
                }
            }
        } else {
            resultado = cupo;
        }
    } catch (e) {

    }
    return resultado;
}
