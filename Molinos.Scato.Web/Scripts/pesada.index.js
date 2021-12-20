$(document).ready(function () {
    //Máscaras
    if ($("#TipoVehiculo").val() != 1) {
        $(".patente-internacional").mask("?*******", {placeholder: ""});
    } else {
        $(".patente-internacional").mask("?9999999");
    }
    //Solo Numérico
    $("#PesoManual").keydown(function (event) {
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 || event.keyCode == 13) {
        } else {
            if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });

    if ($('#BalanzaId').has('option').length == 0) {
        MostrarAlertaError($("#PuestoDeTrabajoSinBalanzasErrorMensaje").val());
    }

    //$(document).bind("keypress", function (event) {
    //    if (event.keyCode == 32) {
    //        event.target.click();
    //    }
    //});

    Mousetrap.stopCallback = function (e, element, combo) {
        return false;
    };

    //atajosPantallaGeneral();

    $(document).on("submit", "form.causaBlock", function () {
        Mousetrap.pause();
    });

    ReiniciarBalanza();
    $("#BalanzaId").change(ReiniciarBalanza);

    var nombrePc = ObtenerNombrePC();
    if (nombrePc.length == 0 || nombrePc == "NoTienePuesto") {
        MostrarAlertaError($("#PuestoDeTrabajoErrorMensaje").val());
    }

    //Hacer foco en Peso Manual al tomar peso
    $('#dialogo-pesar').on('shown', function () {
        $(this).find("#PesoManual").select();
        $('#dialogo-pesar').on('keydown', function(event) {
            if (event.keyCode == 13) {
                if ($("#dialogo-pesar-cancelar").is(':focus')) {
                    $("#dialogo-pesar-cancelar").click();
                } else {
                    $("#dialogo-pesar-guardar").click();
                }
            }
        });
    });

    $('#dialogo-pesar').on('hidden', function () {
        $('#btnAceptar').focus();
    });

    $('#dialogo-controlBalanza').on('hidden', function () {
        $('#CheckBoxControlBalanza').focus();
    });

    //Valido si ingresó la patente correcta
    $("#Patente").change(function () {
        InhabilitarPantalla();
        $.getJSON($("#Patente").data().patenteUrl, { patente: $("#Patente").val(), patenteOriginal: $("#PatenteOriginal").val() }, function (data) {
            if (data.resultado == "OK")
                HabilitarPantalla();
            else {
                window.location = data.url;
            }
        });
    });

    //Inhabilito pantalla hasta que se ingrese una patente correcta
    if ($("#Patente").val().toLowerCase() != $("#PatenteOriginal").val().toLowerCase())
        InhabilitarPantalla();


    $('form').off('keypress');

    $('form').keypress(function (event) {
        if (event.which == 13) {
            //if ($("#dialogo-pesar").data('modal') != undefined && $("#dialogo-pesar").data('modal').isShown) {
            //    //event.preventDefault();
            //    GuardarPesoManual();
            //    return false;
            //}

            if ($("#dialogo-BalanzaACero").data('modal') != undefined && $("#dialogo-BalanzaACero").data('modal').isShown) {
                event.preventDefault();
                $("#dialogo-BalanzaACero").find('.btn:first').click();
                return false;
            }

            if ((event.target.type != "textarea")) {
                event.preventDefault();
                $(this).submit();
            }
        }
        return true;
    });

    $(window).keydown(function (event) {
        return event.keyCode != 13;
    });

    $(document).on('click', "#dialogo-BalanzaACero-enCero", BalanzaEnCero);

    $(document).on('click', "#dialogo-BalanzaACero-dialogoCerear", cargarDialogoCereo);

    $(document).on('click', "#dialogo-cerear-cerear", CerearBalanza);

    $("#btnACero").on('click', function () {
        $("#btnACero").attr("disabled", true);
        cargarDialogoBalanzaACero(ReiniciarBalanza);
    });

    $("#dialogo-pesar-guardar").on('click', GuardarPesoManual);

    $("#tomarPeso").on('click', TomarPeso);


    $("#botonCancelar").bind('keypress', function (event) {
        if (event.keyCode == 32) {
            InhabilitarPantalla();
            event.target.click();
        }
    });


    $("#dialogo-pesar-cancelar").on('click', function () {
        $('#dialogo-pesar').modal('hide');
        atajosPantallaGeneral();
    });

    $("#CheckBoxControlBalanza").on('click', cargarDialogoControlBalanza);

    $("#dialogo-controlBalanza-confirmar").on('click', function () {
        $('#CheckBoxControlBalanza').prop('checked', true);
        $('#dialogo-controlBalanza').modal('hide');
        atajosPantallaGeneral();
    });

    $("#dialogo-controlBalanza-close").on('click', function () {
        $('#CheckBoxControlBalanza').attr('checked', false);
        $('#dialogo-controlBalanza').modal('hide');
        atajosPantallaGeneral();
    });
    
    $("#dialogo-controlBalanza-cancelar").on('click', function () {
        $('#CheckBoxControlBalanza').attr('checked', false);
        $('#dialogo-controlBalanza').modal('hide');
        atajosPantallaGeneral();
    });

    ReiniciarTabPantallaPesada();
    ReiniciarTabPopUpTomarPeso();
    ActualizarDif();
    $(document).on('click', '#btnRechazar', MostrarDialogoRechazar);
    $(document).on('click', '.dialogo-rechazar-cerrar', function () {
        $("#dialogo-rechazar").modal('hide');
        return false;
    });
    $(document).on('click', '#btnAceptar', function () {
        $('#Mensaje').val(null);
        $('#Comentario').val(null);
    });

     if ($("#Automatizado").val() == "True") {
        $("body").css("background-color", "rgba(169, 219, 169, 1)");
    }

});
activoAutomatico = true;
pesoAutomatico = 0;
pesadasAutomatico = new Array();
function TomarPesoAutomatico() {
    $.getJSON($("#Peso").data().pesoUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {//pesada tomarpeso
        if ($.isNumeric(data)) {
            $("#Peso").val(data);
            pesadasAutomatico.push(data);
            console.log(pesadasAutomatico);
            if (pesadasAutomatico.length >= 2) {
                var delta = Math.abs(pesadasAutomatico[pesadasAutomatico.length - 1] - pesadasAutomatico[pesadasAutomatico.length - 2]);
                if (delta < 200 && pesadasAutomatico[0] > 0) {
                    //console.log("esta pesando lo mismo en todas: " + pesadasAutomatico[pesadasAutomatico.length - 1]);
                    pesadasAutomatico = new Array();
                    activoAutomatico = false;
                    $("#btnAceptar").click();
                } else {
                    console.log("Error: " + delta);
                }
            }
            if (pesadasAutomatico.length == 3) {
                activoAutomatico = false;
                pesadasAutomatico = new Array();
                $("body").css("background-color", "rgba(252, 239, 161, 1)");
            }
            ActualizarPesos();
        } else { //Devolvió error
            MostrarAlertaError(data);
            activoAutomatico = false;
        }
    }).complete(function () {
        $("#tomarPeso").attr("disabled", false);
        $("#btnAceptar").focus();
        if (activoAutomatico) {
            setTimeout(TomarPesoAutomatico, 1000);
        }
    });
   
}

function InhabilitarPantalla() {
    $("#btnAceptar").attr("disabled", true);
    $("#btnRechazar").attr("disabled", true);
    $("#tomarPeso").attr("disabled", true);
    if (("#AlmacenId").length > 0)
        $("#AlmacenId").attr("disabled", true);
    if (("#HidraulicaId").length > 0)
        $("#HidraulicaId").attr("disabled", true);
    if (("#CalleId").length > 0)
        $("#CalleId").attr("disabled", true);
    if (("#ProximaBalanzaId").length > 0)
        $("#ProximaBalanzaId").attr("disabled", true);
    Mousetrap.pause();
}

function HabilitarPantalla() {
    if ($("#Patente").val().toLowerCase() == $("#PatenteOriginal").val().toLowerCase() && $("#btnACero").hasClass("hide")) {
        $("#btnAceptar").attr("disabled", false);
        $("#btnRechazar").attr("disabled", false);
        $("#tomarPeso").attr("disabled", false);
        if (("#AlmacenId").length > 0)
            $("#AlmacenId").attr("disabled", false);
        if (("#HidraulicaId").length > 0)
            $("#HidraulicaId").attr("disabled", false);
        if (("#CalleId").length > 0)
            $("#CalleId").attr("disabled", false);
        if (("#ProximaBalanzaId").length > 0)
            $("#ProximaBalanzaId").attr("disabled", false);
        atajosPantallaGeneral();
        ConfigurarFocos();
    }
}

function ReiniciarBalanza() {
    if ($("#BalanzaId").val() > 0) {
        $("#Peso").val("");
        $("#PesoManual").val("");
        $.getJSON($("#BalanzaId").data().balanzaUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            $(".balanzaColor").css("background-color", data.Color);
            $("#Modalidad").val(data.Modalidad);
            $("#BalanzaPuestoDeTrabajo").val(data.PuestoDeTrabajo);
            if (!data.EstaEnCero) {
                $("#tomarPeso").attr("disabled", true);
                $("#btnACero").removeClass("hide");
                $("#dialogo-cerear-body").css("background-color", data.Color);
                $("#dialogo-BalanzaACero-body").css("background-color", data.Color);
                ConfigurarFocos();

            } else {
                $("#btnACero").addClass("hide");
                HabilitarPantalla();
                ConfigurarFocos();
            }

            if ($("#Automatizado").val() == "True" && $("#Modalidad").val() == 1 && $("#tomarPeso").attr("disabled") != true) {
                TomarPesoAutomatico();
            }
            else if ($("#Automatizado").val() == "True") {
                $("body").css("background-color", "rgba(252, 239, 161, 1)");
            }

            $.get($("#balanzaACero").data().balanzaCeroUrl, { modalidad: $("#Modalidad").val() }, function (data) {
                $('#balanzaACero').html(data);
                
            });
        }).complete(function () {
            var nombrePc = ObtenerNombrePC();
            //if (nombrePc != $("#BalanzaPuestoDeTrabajo").val()) {
            //    MostrarAlertaError($("#PuestoDeTrabajoPcBalanzaErrorMensaje").val());
            //}
        });
    }
}

//Dialogo Peso Manual
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

    Mousetrap.bind('alt+3', function () {
        $(this).find("#PesoManual").focus();
    });
}

//Dialogo Control Balanza
function cargarDialogoControlBalanza() {
    if ($('#CheckBoxControlBalanza').prop('checked')) {

        $('#dialogo-controlBalanza').modal({
            backdrop: 'static', keyboard: false
        }).css({
            width: '300px',
            'margin-left': function () {
                return -($(this).width() / 2);
            },
            'top': '50%',
            'margin-top': function () {
                return -($(this).height() / 2);
            }
        });
        atajosControlBalanzaPopUp();
    }
}

function TomarPeso() {
    var nombrePc = ObtenerNombrePC();
    //if (nombrePc.length == 0 || nombrePc == "NoTienePuesto") {
    //    MostrarAlertaError($("#PuestoDeTrabajoErrorMensaje").val());
    //    return true;
    //}
    //if (nombrePc != $("#BalanzaPuestoDeTrabajo").val()) {
    //    MostrarAlertaError($("#PuestoDeTrabajoPcBalanzaErrorMensaje").val());
    //    return true;
    //}
    if ($("#Modalidad").val() == 0) {
        cargarDialogoPesar();
        atajosTomarPesoPopUp();
    } else {
        //Toma el peso desde el orquestador
        var label = $("#tomarPeso").html();
        $("#tomarPeso").html($("#tomarPeso").data().mensajeEsperar);
        $("#tomarPeso").attr("disabled", true);
        $.getJSON($("#Peso").data().pesoUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            if ($.isNumeric(data)) {
                $("#Peso").val(data);
                ActualizarPesos();
            } else { //Devolvió error
                MostrarAlertaError(data);
            }
        }).complete(function () {
            $("#tomarPeso").html(label);
            $("#tomarPeso").attr("disabled", false);
            $("#btnAceptar").focus();
        });
    }
}

function GuardarPesoManual() {
    //Salvar Peso Manual
    $('#Peso').val($('#PesoManual').val());
    $('#dialogo-pesar').modal('hide');
    ActualizarPesos();
    atajosPantallaGeneral();
}

function ActualizarPesos() {
    //Peso Neto Balanza
    if ($(".peso1").val().length > 0 && $(".peso2").val().length > 0) {
        $("#PesoNeto").val(parseInt($(".peso1").val()) - parseInt($(".peso2").val()));
    }
    ActualizarDif();
    //Revalidar peso
    ValidarObjeto($("#pesada-form"), $('#Peso'));
}

function ActualizarDif() {
    //Diferencia
    if ($("#PesoNeto").val().length > 0 && $("#PesoNetoOrigen").html().length > 0) {
        $("#difPesoNeto").html(parseInt($("#PesoNeto").val()) - parseInt($("#PesoNetoOrigen").html()));
    }
    if ($(".peso2").val().length > 0 && $("#PesoTaraOrigen").html().length > 0) {
        $("#difPesoTara").html(parseInt($(".peso2").val()) - parseInt($("#PesoTaraOrigen").html()));
    }
    if ($(".peso1").val().length > 0 && $("#PesoBrutoOrigen").html().length > 0) {
        $("#difPesoBruto").html(parseInt($(".peso1").val()) - parseInt($("#PesoBrutoOrigen").html()));
    }
}

function AceptarFormulario() {
    //Espera hasta que se guarde el peso
    if ($("#pesada-form").valid() && $("#PatenteOriginal").val().toLowerCase() == $("#Patente").val().toLowerCase())
        BlockUI($("#pesada-form").data().mensajeEspera);
}

function EvaluarResultado(data) {
    if (data.responseText == "ERROR")
        window.location = window.location;
    else if (data.responseText == "OK" || data.responseText == "ErrorActividadYaEjecutada") {
        window.location = $("#ListaDeCamionesUrl").val();
    }
    else {
        $.unblockUI();
        MostrarAlertaAdvertencia(data.responseText);
        ReiniciarBalanza();
    }
}

/*BALANZA A CERO*/
//Como parametro se recibe la funcion a ejecutar cuando se cierra algún diálogo
var activo;
function cargarDialogoBalanzaACero(callbackFunction) {
    activo = true;
    if ($("#Modalidad").val() == 1) {
        $('#dialogo-BalanzaACero-balanzaPeso').text($('#dialogo-BalanzaACero-balanzaPeso').data().obteniendoPeso);
        TomarPesoRecursivo();
    }

    $('#dialogo-BalanzaACero').on('hide', function () {
        activo = false;
        $("#btnACero").attr("disabled", false);
        callbackFunction();
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

function ConfigurarFocos() {

    var botonCeroOculto = $("#btnACero").hasClass("hide");
    if (!botonCeroOculto) {
        $('#btnACero').focus();

    } else if ($("#Patente").val().length == 0) {
        $("#Patente").focus();
    } else {
        $('#tomarPeso').focus();
    }
}

function ReiniciarTabPantallaPesada() {
    $('#botonCancelar').focusout(function () {
        ConfigurarFocos();
    });
}

function ReiniciarTabPopUpTomarPeso() {
    $('#dialogo-pesar-cancelar').on("focusout", function () {
        $('#PesoManual').focus();
    });
}

function atajosPantallaGeneral() {
    Mousetrap.bind('alt+1', function () {
        $("#btnAceptar").click();
    });

    Mousetrap.bind('alt+2', function () {
        InhabilitarPantalla();
        BlockUI();
        window.location.href = $("#botonCancelar").attr('href');
    });
    Mousetrap.bind('alt+3', function () {
        $("#tomarPeso").click();
    });
}

function atajosTomarPesoPopUp() {
    Mousetrap.bind('alt+1', function () {
        $("#dialogo-pesar-guardar").click();
    });
    Mousetrap.bind('alt+2', function () {
        $("#dialogo-pesar-cancelar").click();
    });
}

function atajosControlBalanzaPopUp() {
    Mousetrap.bind('alt+1', function () {
        $("#dialogo-controlBalanza-confirmar").click();
    });
    Mousetrap.bind('alt+2', function () {
        $("#dialogo-controlBalanza-cancelar").click();
    });
    Mousetrap.bind('alt+3', function () {
        $(this).find("#dialogo-controlBalanza-confirmar").focus();
    });
}

function MostrarDialogoRechazar() {
    $('#dialogo-rechazar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-rechazar').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
};
