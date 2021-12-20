pesoManual = null;

var cancelarTimeout = false;

$(document).ready(function () {
    if ($('#BalanzaId').val() == 0) {
        MostrarAlertaError($("#PuestoDeTrabajoSinBalanzasErrorMensaje").val());
    }

    $(document).on('shown', "#dialogo-pesar", function () {
        $(this).find("#PesoManual").select();
        $('#dialogo-pesar').on('keydown', function (event) {
            if (event.keyCode == 13) {
                if ($("#dialogo-pesar-cancelar").is(':focus')) {
                    $("#dialogo-pesar-cancelar").click();
                } else {
                    $("#dialogo-pesar-guardar").click();
                }
            }
        });
    });
    $(document).on('keydown', "#PesoManual", function (event) {
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 || event.keyCode == 13) {
        } else {
            if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });

    $(document).on('click', "#dialogo-pesar-guardar", GuardarPesoManual);
    $(document).on('click', "#tomarPesoTara", TomarPeso);
    $(document).on('click', "#tomarPesoBruto", TomarPeso);
    $(document).on('click', "#dialogo-pesar-cancelar", function () { $('#dialogo-pesar').modal('hide'); });
    $(document).on('click', "#dialogo-BalanzaACero-enCero", BalanzaEnCero);
    $(document).on('click', "#dialogo-BalanzaACero-dialogoCerear", cargarDialogoCereo);
    $(document).on('click', "#dialogo-cerear-cerear", CerearBalanza);
    $(document).on('click', "#btnACeroTara", cargarDialogoBalanzaACero);
    $(document).on('click', "#btnACeroBruto", cargarDialogoBalanzaACero);
    $(document).on('click', '.confirmar-boton', cambiarAtributoValue);
    $(document).on('click', '#boton-parcial-confirmar', cerrarModal);
    $(document).on('click', '#cancelar', botonCancelar);
    $(document).on('click', '#boton-cancelar', modalCancelar);
    $(document).on('click', '#dialogo-confirmar-cancelar', setearTimeOut);

    ReiniciarBalanza();
});

function TomarPeso() {
    self = this;
    var peso = $(self).parent().parent().find("input");
    var nombrePc = ObtenerNombrePC();
    if (nombrePc.length == 0 || nombrePc == "NoTienePuesto") {
        MostrarAlertaError($("#PuestoDeTrabajoErrorMensaje").val());
        return true;
    }
    if ($("#Modalidad").val() == 0) {
        pesoManual = peso;
        cargarDialogoPesar();
    } else {
        //Toma el peso desde el orquestador
        var label = $(self).html();
        $(self).html($(self).data().mensajeEsperar);
        $(self).attr("disabled", true);

        $.getJSON($(peso).data().pesoUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            if ($.isNumeric(data)) {
                $(peso).val(data);
                ActualizarPesoNeto(peso);
            } else { //Devolvió error
                MostrarAlertaError(data);
            }
        }).complete(function () {
            $(self).html(label);
            $(self).attr("disabled", false);
            $("#btnAceptar").focus();
        });
    }
}

function GuardarPesoManual() {
    //Salvar Peso Manual
    $(pesoManual).val($('#PesoManual').val());
    $('#dialogo-pesar').modal('hide');
    ActualizarPesoNeto(pesoManual);
    pesoManual = null;
}

function ActualizarPesoNeto(peso) {
    //Peso Neto Balanza
    if ($(".peso1").val().length > 0 && $(".peso2").val().length > 0) {
        $("#PesoNeto").val(parseInt($(".peso1").val()) - parseInt($(".peso2").val()));
    }
    //Revalidar peso
    ValidarObjeto($(peso).closest("form"), $(peso));
}

function cargarDialogoPesar() {
    $('#dialogo-pesar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-pesar').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $("#PesoManual").focus();
}

function ReiniciarBalanza() {
    if ($("#EstaEnCero").val() === "False") {
        $("#tomarPesoTara").attr("disabled", true);
        $("#tomarPesoBruto").attr("disabled", true);

        $("#btnACeroTara").removeClass("hide");
        $("#btnACeroBruto").removeClass("hide");
        $("#dialogo-cerear-body").css("background-color", $('balanza').css("color"));
        $("#dialogo-BalanzaACero-body").css("background-color", $('balanza').css("color"));
    } else {
        $("#btnACeroTara").addClass("hide");
        $("#btnACeroBruto").addClass("hide");
        $("#tomarPesoTara").attr("disabled", false);
        $("#tomarPesoBruto").attr("disabled", false);
    }

    $.get($("#balanzaACero").data().balanzaCeroUrl, { modalidad: $("#Modalidad").val() }, function (data) {
        $('#balanzaACero').html(data);
    });
}


/*BALANZA A CERO*/
//Como parametro se recibe la funcion a ejecutar cuando se cierra algún diálogo
var activo;
function cargarDialogoBalanzaACero() {
    activo = true;
    if ($("#Modalidad").val() == 1) {
        $('#dialogo-BalanzaACero-balanzaPeso').text($('#dialogo-BalanzaACero-balanzaPeso').data().obteniendoPeso);
        TomarPesoRecursivo();
    }

    $('#dialogo-BalanzaACero').on('hide', function () {
        activo = false;
        ReiniciarBalanza();
    });

    $('#dialogo-BalanzaACero').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-BalanzaACero').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });

    $("#dialogo-BalanzaACero").find('.btn:first').focus();
}

function TomarPesoRecursivo() {
    $.getJSON($("#TomarPesoUrl").val(), { balanzaId: $("#BalanzaId").val() }, function (data) {
        var peso = data;
        if ($.isNumeric(data)) {
            data += "kg";
        }
        $('#dialogo-BalanzaACero-balanzaPeso').text(data);
        if ($.isNumeric(peso) && peso == 0 && activo) {
            BalanzaEnCero();
        }
    }).complete(function () {
        if (activo) {
            setTimeout(TomarPesoRecursivo, 2000);
        }
    });
}

function cambiarAtributoValue() {
    var idBoton = $(this).attr('id');
    if (idBoton == "boton-parcial") {
        $("#boton-parcial-confirmar").attr('value', 'True');
    }
    else {
        $("#boton-parcial-confirmar").attr('value', 'False');
    }
}

function cerrarModal() {
    $('#dialogo-confirmar').modal('hide');
}

function cargarDialogoCereo() {
    activo = false;
    $('#dialogo-BalanzaACero').html($('#dialogo-Cerear').html());
    $("#dialogo-BalanzaACero").find('.btn:first').focus();
}

function BalanzaEnCero() {
    $.getJSON($("#BalanzaEnCeroUrl").val(), { balanzaId: $("#BalanzaId").val() }).complete(function (data) {
        if (data != null && data.responseJSON.data != 'true') {
            MostrarAlertaError(data.data);
        } else {
            MostrarAlertaExitosa();
            $("#EstaEnCero").val("True");
        }
    }).complete(function () { $('#dialogo-BalanzaACero').modal('toggle'); });
}

function CerearBalanza() {
    $.getJSON($("#CerearBalanzaUrl").val(), { balanzaId: $("#BalanzaId").val() }, function (data) {
        if (data != null && data.data != 'true') {
            MostrarAlertaError(data.data);
            $('#dialogo-BalanzaACero').modal('toggle');
        } else {
            MostrarAlertaExitosa();
            $('#dialogo-BalanzaACero').modal('toggle');
        }
    });
}

function AceptarFormularioTara() {
    if ($("#pesada-tara-form").valid())
        BlockUI($("#pesada-tara-form").data().mensajeEspera);
}

function AceptarFormularioCarga() {
    var val = $("#carga-form").find("button[type=submit]:focus").val();
    $("#carga-form").append('<input id="CargaParcial" type="hidden" name="CargaParcial" value="' + val + '" />');
    BlockUI($("#pesada-tara-form").data().mensajeEspera);
    activo = false;
}

function AceptarFormularioBruto() {
    if ($("#pesada-bruto-form").valid())
        BlockUI($("#pesada-tara-form").data().mensajeEspera);
}

function EvaluarResultadoTara(data) {
    if (data.responseText == "ERROR")
        window.location = window.location;
    else if (data.responseText == "OK" || data.responseText == "ErrorActividadYaEjecutada") {
        $("#peso-tara").addClass("disabled-div").prop('readonly', true);
        $.get(pesadaCargaUrl, function (data) {
            $('#cargaDiv').html(data);
        }).complete(function () { $.unblockUI(); });
    }
    else {
        $.unblockUI();
        MostrarAlertaAdvertencia(data.responseText);
    }
}

function EvaluarResultadoCarga(data) {
    if (data.responseText == "ERROR")
        window.location = window.location;
    else if (data.responseJSON != undefined && (data.responseJSON.responseText == "OK" || data.responseJSON.responseText == "ErrorActividadYaEjecutada")) {
        if (data.responseJSON.CargaParcial) {
            window.location = $("#ListaDeCamionesUrl").val() + "?evitarRedireccion=true";
        }
        else {
            $("#carga").addClass("disabled-div").prop('readonly', true);
            CargarPesadaBruto();
        }
    }
    else {
        $.unblockUI();
        MostrarAlertaAdvertencia(data.responseText);
    }
}

function CargarPesadaBruto() {
    $.get(pesadaBrutoUrl, function (data) {
        $('#pesadaBrutoDiv').html(data);
    }).complete(function () {
        if ($('#pesadaBrutoDiv').find('.disabled-div').length > 0) {
            setTimeout(function () { CargarPesadaBruto(); }, 1000);
        }
        else {
            $.unblockUI();
        }
    });
}

function CargarPesadaTara() {
    $.get(pesadaTaraUrl, function (data) {
        $('#pesadaTaraDiv').html(data);
    }).complete(function () {
        if ($('#pesadaTaraDiv').find('.disabled-div').length > 0) {
            setTimeout(function () { CargarPesadaTara(); }, 1000);
        }
        else {
            $.unblockUI();
        }
    });
}

function CalcularPeso() {
    var netoEsperado = $("#pesoNetoEsperado").val();
    var netoEsperadoAumento = Math.round(netoEsperado / 100 * 40);
    var netoEsperadoAdicional = +netoEsperado + netoEsperadoAumento;

    if ($("#Modalidad").val() == 0) {
        $("#pesoNetoContainer").addClass('hidden');

    } else {
        $("#pesoNetoContainer").removeClass('hidden');
        activo = true;
        $.getJSON($("#TomarPesoUrl").val(), { balanzaId: $("#BalanzaId").val() }, function (data) {
            data = data - $("#pesoTaraText").val();
            var peso = data;


            if (activo) {
                if ($.isNumeric(data)) {
                    data += " Kg";
                    $("#pesoNetoBalanza").text(data);
                } else {
                    MostrarAlertaError(data);
                }

            }

            if (peso <= netoEsperado) {
                var color = 255 * peso / netoEsperado;
                color = Math.round(color);
                $("#pesoNetoBalanza").css("background-color", 'rgb(0,' + color + ',0)');
            }
            else if (peso > netoEsperado && peso <= netoEsperadoAdicional) {
                var color = ((netoEsperado - peso) * 255 / netoEsperadoAumento) + 255;
                color = Math.round(color);
                $("#pesoNetoBalanza").css("background-color", 'rgb(0,' + color + ',0)');
            }

            else if ($.isNumeric(peso)) {
                $("#pesoNetoBalanza").css("background-color", 'rgb(0,0,0)');
            }

        }).complete(function () {
            if (activo) {
                setTimeout(CalcularPeso, 2000);
            } else {
                return;
            }
        });
    }


}

function EvaluarResultadoBruto(data) {
    if (data.responseText == "ERROR")
        window.location = window.location;
    else if (data.responseText == "OK" || data.responseText == "ErrorActividadYaEjecutada") {
        window.location = $("#ListaDeCamionesUrl").val();
    }
    else {
        $.unblockUI();
        MostrarAlertaAdvertencia(data.responseText);
    }
}

function botonCancelar() {
    $.getJSON($("#cancelar").attr("href"), function (data) {
        if (data.responseText == "ERROR")
            window.location = window.location;
        else if (data.responseText == "OK" || data.responseText == "ErrorActividadYaEjecutada") {
            $("#carga").addClass("disabled-div").prop('readonly', true);
            window.location = $("#ListaDeCamionesUrl").val() + "?evitarRedireccion=true";
        }
        else {
            $.unblockUI();
            MostrarAlertaAdvertencia(data.responseText);
        }

    });
    return false;
}

function modalCancelar() {
    $("#dialogo-cancelar").modal('show');
    cancelarTimeout = true;
    return false;
}

function setearTimeOut() {
    cancelarTimeout = false;
    setTimeout(function () {
        $.get(pesadaCargaUrl, function (data) {
            $('#cargaDiv').html(data);
        });
    }, 1500);
} 