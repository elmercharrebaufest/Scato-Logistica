function InhabilitarPantalla() {
    $('.row-bloqueable :input').attr('disabled', true);
    $('.row-bloqueable :button').attr('disabled', true);
    $('.row-bloqueable .btn').attr('disabled', true);
    $('.patente').attr('readonly', false);
    $(".patente").focus();
    Mousetrap.pause();
}

function HabilitarPantalla() {
    if ($(".patente").val().toLowerCase().replace('_', '') == $(".patenteOriginal").val().toLowerCase()) {
        $('.row-bloqueable :input').attr('disabled', false);
        $('.row-bloqueable :button').attr('disabled', false);
        $('.row-bloqueable .btn').attr('disabled', false);
        $('.nohabilitar').attr('disabled', true);
        $('.patente').attr('readonly', 'readonly');
        $('input:not([readonly="readonly"]):enabled:visible:first').focus();
        InicializarHumedimetro();
        InicializarNirs();
        Mousetrap.unpause();
    }
}

function TomarHumedad() {
    //Toma el humedad desde el orquestador
    if (!$('.boton-humedad').hasClass('play')) {
        $.getJSON($("#HumedimetroId").data().url, function (data) {
            if ($.isNumeric(data) && data != null) {
                if (escuchar) {
                    $(".textboxHumedad").val(formatFloat(data));                  
                    if ($("#rangosDeRedondeoJson").val() != null && $("#rangosDeRedondeoJson").val() != undefined) {                        
                        var listaRangos = JSON.parse($("#rangosDeRedondeoJson").val());
                        jQuery.each(listaRangos, function () {
                            if (this.ValorDesde <= data && this.ValorHasta >= data) {
                                data = this.ValorRedondeado;
                            }
                        });

                        $(".textboxCaracteristica.textboxHumedad").val(formatFloat(data));
                        $(".humedimetro-manual").val(1);
                    }
                    
                    $(".boton-manual").attr("disabled", false); 
                    $(".nroDeToma").val(parseInt($(".nroDeToma").val()) + 1);
                    $("form").validate().element($(".textboxCaracteristica.textboxHumedad"));
                }
                estadoHumedimetro = true;
            } else if (estadoHumedimetro) {
                if (data != null) {
                    //Devolvió error 
                    MostrarAlertaInfo($('#humedimetroNoResponde').val());
                    estadoHumedimetro = false;
                }
            }
        }).complete(function () {
            if (escuchar) {
                setTimeout(TomarHumedad, 1500);
            }
        });
    }
}

function TomarPH() {
    //Toma el peso hectolitrico desde el orquestador
    if (!$('.boton-PH').hasClass('play')) {
        $.getJSON($("#HumedimetroId_PH").data().url, function (data) {
            if ($.isNumeric(data) && data != null) {
                if (escuchar) {
                    $(".textboxPH").val(formatFloat(data));
                    if ($("#rangosDeRedondeoJson").val() != null && $("#rangosDeRedondeoJson").val() != undefined) {
                        var listaRangos = JSON.parse($("#rangosDeRedondeoJson").val());
                        jQuery.each(listaRangos, function () {
                            if (this.ValorDesde <= data && this.ValorHasta >= data) {
                                data = this.ValorRedondeado;
                            }
                        });

                        $(".textboxCaracteristica.textboxPH").val(formatFloat(data));
                        $(".humedimetro-manual-PH").val(1);
                    }

                    $(".boton-manual_PH").attr("disabled", false);
                    $(".nroDeToma").val(parseInt($(".nroDeToma").val()) + 1);
                    $("form").validate().element($(".textboxCaracteristica.textboxPH"));
                }
                estadoHumedimetro = true;
            } else if (estadoHumedimetro) {
                if (data != null) {
                    //Devolvió error 
                    MostrarAlertaInfo($('#humedimetroNoResponde').val());
                    estadoHumedimetro = false;
                }
            }
        }).complete(function () {
            if (escuchar) {
                setTimeout(TomarPH, 1500);
            }
        });
    }
}


var escuchar = false;
function Humedimetro(intervalo, segundos) {
    if (escuchar) {
        TomarHumedad();
        TomarPH();
        return setTimeout(function () { DetenerHumedimetro(intervalo); }, segundos * 1000);
    } else {
        if (intervalo != null) clearTimeout(intervalo);
        escuchar = false;
        return null;
    }
}

var estadoHumedimetro = true;
function IniciarHumedimetro(intervalo, segundos) {
    $('.boton-humedad').html('<i class="icon-pause"></i>');
    $('.boton-humedad').removeClass('play');
    $(".textboxHumedad").attr("readonly", true);
    estadoHumedimetro = true;
    escuchar = true;
    return Humedimetro(intervalo, segundos);
}

function DetenerHumedimetro(intervalo) {
    $('.boton-humedad').addClass('play');
    $('.boton-humedad').html('<i class="icon-play"></i>');
    escuchar = false;
    return Humedimetro(intervalo);
}

function InicializarHumedimetro() {
    if ($("#HumedimetroId").length > 0 && $("#HumedimetroId").data().automatico) {
        var intervalo = null;
        var segundos = 180;
        $('.boton-humedad').on('click', function () {
            if ($('.boton-humedad').hasClass('play')) {
                intervalo = IniciarHumedimetro(intervalo, segundos);              
            } else {
                intervalo = DetenerHumedimetro(intervalo);
            }
            $(".humedimetro-manual").addClass("hidden");
        });

        $(".boton-manual").on('click', function () {
            intervalo = DetenerHumedimetro(intervalo);
            $(".textboxHumedad").attr("readonly", false);
            $(".boton-manual").attr("disabled", true);
            $(".humedimetro-manual").removeClass("hidden");
            $(".textboxHumedad").focus();
        });

        $(".boton-manual_PH").on('click', function () {
            intervalo = DetenerHumedimetro(intervalo);
            $(".textboxPH").attr("readonly", false);
            $(".boton-manual_PH").attr("disabled", true);
            $(".humedimetro-manual-PH").removeClass("hidden");
            $(".textboxPH").focus();
        });
       
        $('.boton-humedad').click();     
    }
}


function TomarAnalisis() {
    //Toma el humedad desde el orquestador
    if (!$('.boton-nirs').hasClass('play')) {
        $('.boton-nirs').html('<i class="icon-time"></i>');
        $('.boton-nirs').removeClass('play');
        $('.boton-manual-nirs ').prop('disabled', true);
        $('.textboxnirs').val("");
        
        $.getJSON($("#NirsId").data().url, function (data) {
            if (data != null && typeof data != "string") {
                $.each(data, function (key, value) {
                    if ($(".textboxnirs" + key).length > 0) {
                        $(".textboxnirs" + key).val(formatFloat(value));

                        if ($("#rangosDeRedondeoJson").val() != null && $("#rangosDeRedondeoJson").val() != undefined) {
                            if ($(".textboxCaracteristica.textboxnirs" + key).data().eshumedad == "True") {

                                //round redondea el 0.15 hacia 0.2, mientras que -0.15 hacia 0.1. Se cambia el signo para que en 0.15 sea para abajo y se vuelve a cambiar para que quede positivo
                                value = -Math.round(-value * 10) / 10;

                                var listaRangos = JSON.parse($("#rangosDeRedondeoJson").val());

                                jQuery.each(listaRangos, function () {
                                    if (this.ValorDesde <= value && this.ValorHasta >= value) {
                                        value = this.ValorRedondeado;
                                    }
                                });
                            }
                            
                            $(".textboxCaracteristica.textboxnirs" + key).val(formatFloat(value));
                            $(".modalidad" + key).val(1);
                        }

                        $("form").validate().element($(".textboxCaracteristica.textboxnirs" + key));
                    }
                });

                    $(".boton-manual-nirs").attr("disabled", false);
                    $(".nroDeTomaNirs").val(parseInt($(".nroDeTomaNirs").val()) + 1);
            } else if (data != null)
                {
                    //Devolvió error 
                    MostrarAlertaInfo(data);
            }
            $('.boton-nirs').html('<i class="icon-play"></i>');
            $('.boton-nirs').addClass('play');
            $('.boton-nirs').prop('disabled', false);
            $('.textboxCaracteristica').trigger("change");
        });
    }
}

function IniciarNirs() {
    $('.boton-nirs').html('<i class="icon-time"></i>');
    $('.boton-nirs').removeClass('play');
    $(".textboxnirs").attr("readonly", true);
    $('.boton-nirs').prop('disabled', true);
    return TomarAnalisis();
}

function InicializarNirs() {
    if ($("#NirsId").length > 0 && $("#NirsId").data().automatico) {
        $('.boton-nirs').on('click', function () {
            if ($('.boton-nirs').hasClass('play')) {
                IniciarNirs();
                $(".humedimetro-manual-nirs").addClass("hidden");
            }
        });

        $(".boton-manual-nirs").on('click', function () {
            $('.textboxnirs'+ $(this).data().textbox).attr("readonly", false);
            $(this).attr("disabled", true);
            $('.textboxnirs' + $(this).data().textbox).focus();
            $(".humedimetro-manual-nirs").removeClass("hidden");
        });

        //$('.boton-nirs').click();
    }
}

function formatBoolean(val) {
    return val ? $("#ValorSi").val() : $("#ValorNo").val();
}

function formatFloat(val) {
    if (val != null) {
        return val.toFixed(2).replace(Globalize.cultures[Globalize.cultureSelector].numberFormat[","], Globalize.cultures[Globalize.cultureSelector].numberFormat["."]);
    }
    return "";
}

function cargarDialogoRechazar(data) {
    $("#mensajeRechazar").html(data);

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
    $.unblockUI();
}

var validarPatenteSingleton = true;
function validarPatente() {
    if (!$(".patente").is('[readonly="readonly"]') && validarPatenteSingleton) {
        if ($(".patente").val().toLowerCase().replace('_', '') != $(".patenteOriginal").val().toLowerCase()) {
            InhabilitarPantalla();
            if ($(".patente").val() != null && $(".patente").val() != '') {
                MostrarAlertaError($('.patente').data().invalida);
            }
        } else {
            validarPatenteSingleton = false;
            HabilitarPantalla();
        }
    }
}

$(document).ready(function () {
    $('input:not([readonly="readonly"]):enabled:visible:first').focus();
    if ($("#TipoVehiculo").val() != 1) {
        $(".patente-internacional").mask("?*******", {placeholder: ""});
    } else {
        $(".patente-internacional").mask("?9999999");
    }


    //Valido si ingresó la patente correcta
    if ($(".patente").val().toLowerCase().replace('_', '') != $(".patenteOriginal").val().toLowerCase()) {
        InhabilitarPantalla();
    } else {
        HabilitarPantalla();
    }
    $('.patente').on('keydown', function (e) {
        if (e.which == 9 && !$(".patente").is('[readonly="readonly"]')) {
                validarPatente();
                e.preventDefault();
                return false;
        }
        return true;
    });

    $(document).on('change', ".patente", function () {
        validarPatente();
    });
    
    $(document).on('click', '.rechazar-boton', function () {
        BlockUI();
        Mousetrap.pause();
        $.get(this.href, cargarDialogoRechazar);
        return false;
    });
    
    $(document).on('click', '.dialogo-rechazar-cerrar', function () {
        $("#dialogo-rechazar").modal('hide');
        $("#mensajeRechazar").html("");
        Mousetrap.unpause();
        return false;
    });
    
    $(document).on('click', '.checkBoxCaracteristica', function () {
        var check = $(this);
        var valor = check.parent().parent().find('input.textboxCaracteristica');
        if (check.is(':checked')) {
            if ($(valor).val() == '' || $(valor).val() == null) {
                $(valor).val(parseInt($(valor).data().min));
            }
            $(valor).addClass('caracteristicaNoNuleable');
        } else {
            $("form").validate().element($(valor));
            $(valor).removeClass('caracteristicaNoNuleable');
        }
    });
    $(document).on('change', '.caracteristicaAnalisisObligatorio', function () {
        var valor = $(this);
        var check = valor.parent().parent().parent().find('input.checkBoxCaracteristica');
        var hidden = valor.parent().parent().parent().find('input[type=hidden][name="' + $(check).attr('name') + '"]');
        
        if ($(valor).val() != '' && $(valor).val() != null && Globalize.parseFloat($(valor).val()) > 0) {
            $(check).prop('checked', true);
            $(check).attr('disabled', true);
            if (hidden.length > 0) {
                $(hidden).val(true);
            }
        } else {
            $(check).prop('checked', false);
            $(check).removeAttr('disabled');
            if (hidden.length > 0) {
                $(hidden).val(false);
            }
        }
    });
    $(document).on('change', '.textboxCaracteristica', function () {
        var valor = $(this);
        if (!valor.hasClass("caracteristicaAnalisisObligatorio")) {
            var check = valor.parent().parent().parent().find('.checkBoxCaracteristica');
            var tolerancia = valor.parent().parent().parent().find('input[type=hidden][name*="ToleranciaSinAnalisis"]').val();
            var hidden = valor.parent().parent().parent().find('input[type=hidden][name="' + $(check).attr('name') + '"]');

            if ($(valor).val() != '' && $(valor).val() != null && Globalize.parseFloat($(valor).val()) > 0 && tolerancia != '' && Globalize.parseFloat($(valor).val()) >= Globalize.parseFloat(tolerancia)) {
                $(check).prop('checked', true);
                $(check).attr('disabled', true);
                if (hidden.length > 0) {
                    $(hidden).val(true);
                }
            } else if ($(check).attr('disabled') && $(check).prop('checked') == true) {
                $(check).prop('checked', false);
                $(check).removeAttr('disabled');
                if (hidden.length > 0) {
                    $(hidden).val(false);
                }
            }
        }
    });
    
    $(document).on('click', '#aceptar', function () {
        if (!($(this).closest('form').valid())) {
            $('.validation-summary-errors').show();
        } else {
            aceptaFormulario = true;
            $(this).closest('form').submit();
        }
    });
    
    $(document).on('click', '#botonCancelar', function () {
        BlockUI();
        return true;
    });
    

    // Esto hace que se disparen los shortcuts aún cuando el foco esté en algún campo
    Mousetrap.stopCallback = function(e, element, combo) {
        return false;
    };

    Mousetrap.bind('alt+1', function () {
        $("#aceptar").click();
    });
    Mousetrap.bind('alt+2', function () {
        BlockUI();
        window.location.href = $("#botonCancelar").attr('href');
    });
    Mousetrap.bind('alt+3', function () {
        $(".rechazar-boton").click();
    });

    $(document).on("submit", "form.causaBlock", function () {
        Mousetrap.pause();
    });
    
    $(window).keydown(function (event) {
        return event.keyCode != 13;
    });
    
    $('#mensajeRechazar').bind('keypress', function (event) {
        if (event.keyCode == 27) {
            $('.close').click();
            return false;
        }
        if (event.keyCode === 13) {
            event.target.click();
            return false;
        }
        return true;
    });
    
    $.validator.addMethod("textboxCaracteristica", function (value, element) {
        return value == '' || (Globalize.parseFloat(value) >= parseFloat($(element).data().min) && Globalize.parseFloat(value) <= parseFloat($(element).data().max));
    }, $('#mensajeRangoValido').val());
    
    $.validator.addMethod("caracteristicaObligatoria", function (value, element) {
        return value.length > 0;
    }, $('#campoRequerido').val());
    
    $.validator.addMethod("caracteristicaNoNuleable", function (value, element) {
        return value.length > 0;
    }, $('#caracteristicaNoNuleable').val());


  //seteo de grado automatico trigo y maiz

    $(".gradoAutomatico").attr("readonly", true)
    $(".pesohectolitrico").change(setearGrado)
    $(".granosquebrados").change(setearGrado)
    $(".granosdañados").change(setearGrado)
    $(".materiasextrañas").change(setearGrado)
    $(".granospanzablanca").change(setearGrado)
    $(".totaldañados").change(setearGrado)
    $(".granosconcarbon").change(setearGrado)
    $('[class*="granosard"]').change(setearGrado)

    //////

});

function hasNumericValue(valor) {
    if (valor) {
        var value = parseFloat((valor.trim()).replace(",", "."), 10)
        return value != NaN;
    } return false;

}
function camposRequeridosTienenValor(campos) {
    return campos.every(hasNumericValue)
}

function setearGrado() {
    var ph = hasNumericValue($(".pesohectolitrico").val()) && parseFloat(($(".pesohectolitrico").val()).replace(",", "."), 10);
    var granosQuebrados = hasNumericValue($(".granosquebrados").val()) && parseFloat(($(".granosquebrados").val()).replace(",", "."), 10);
    var granosDañados = hasNumericValue($(".granosdañados").val()) && parseFloat(($(".granosdañados").val()).replace(",", "."), 10);
    var materiasExt = hasNumericValue($(".materiasextrañas").val()) && parseFloat(($(".materiasextrañas").val()).replace(",", "."), 10);
    var panzaBlanca = hasNumericValue($(".granospanzablanca").val()) && parseFloat(($(".granospanzablanca").val()).replace(",", "."), 10);
    var conCarbon = hasNumericValue($(".granosconcarbon").val()) && parseFloat(($(".granosconcarbon").val()).replace(",", "."), 10);
    var ardidos = hasNumericValue($('[class*="granosard"]').val()) && parseFloat(($('[class*="granosard"]').val()).replace(",", "."), 10);
    var totalDañados = hasNumericValue($(".totaldañados").val()) && parseFloat(($(".totaldañados").val()).replace(",", "."), 10);
    var esMaiz = $("#MaterialId").val() == 386;
    var esTrigo = $("#MaterialId").val() == 13;
    var camposTienenValor = false;
    var esGradoTres = false;
    var esGradoDos = false;
    var esGradoCero = false;
    var caracteristicasObligatoriasTrigo = [$(".pesohectolitrico").val(), $(".granosquebrados").val(), $(".totaldañados").val(), $(".materiasextrañas").val(), $('[class*="granosard"]').val(), $(".granosconcarbon").val(), $(".granospanzablanca").val()];
    var caracteristicasObligatoriasMaiz = [$(".pesohectolitrico").val(), $(".granosquebrados").val(), $(".granosdañados").val(), $(".materiasextrañas").val()];

    var caracteristicasCalidadPorMaterial = {
        trigo : {
            gQuebrados : {
                GradoTres :2,
                GradoDos: 1.20,
                GradoUno:0.5
            },
            pesoHectolitrico :{
                GradoTres :73,
                GradoDos: 76,
                GradoUno: 79,
                caladoMaximo : 100
            },
            tDañados : {
                GradoTres :3,
                GradoDos: 2,
                GradoUno:1
            },
            materiasExtrañas : {
                GradoTres :1.5,
                GradoDos: 0.8,
                GradoUno:0.2
            },
            gArdidos : {
                GradoTres :1.5,
                GradoDos: 1,
                GradoUno:0.5
            },
            gConCarbon : {
                GradoTres :0.3,
                GradoDos: 0.2,
                GradoUno:0.1
            },
            gPanzaBlanca : {
                GradoTres :40,
                GradoDos: 25,
                GradoUno:15
            }
        },
        maiz:{
            gQuebrados :  {
                GradoTres :5,
                GradoDos: 3,
                GradoUno:2
            },
            pesoHectolitrico :{
                GradoTres :69,
                GradoDos: 72,
                GradoUno: 75,
                caladoMaximo: 100

            },
            gDañados : {
                GradoTres :8,
                GradoDos: 5,
                GradoUno:3
            },
            materiasExtrañas : {
                GradoTres :2,
                GradoDos: 1.5,
                GradoUno:1
            }
        }
    }

    if (esMaiz || esTrigo) {
        if (esMaiz) {
            camposTienenValor = camposRequeridosTienenValor(caracteristicasObligatoriasMaiz)
            esGradoCero = ph > caracteristicasCalidadPorMaterial.maiz.pesoHectolitrico.caladoMaximo
                || ph < caracteristicasCalidadPorMaterial.maiz.pesoHectolitrico.GradoTres
                || granosDañados > caracteristicasCalidadPorMaterial.maiz.gDañados.GradoTres
                || granosDañados < 0
                || granosQuebrados > caracteristicasCalidadPorMaterial.maiz.gQuebrados.GradoTres
                || granosQuebrados < 0
                || materiasExt > caracteristicasCalidadPorMaterial.maiz.materiasExtrañas.GradoTres
                || materiasExt < 0;

            esGradoTres = (
                (ph >= caracteristicasCalidadPorMaterial.maiz.pesoHectolitrico.GradoTres && ph < caracteristicasCalidadPorMaterial.maiz.pesoHectolitrico.GradoDos) ||
                (granosDañados <= caracteristicasCalidadPorMaterial.maiz.gDañados.GradoTres && granosDañados > caracteristicasCalidadPorMaterial.maiz.gDañados.GradoDos) ||
                (granosQuebrados <= caracteristicasCalidadPorMaterial.maiz.gQuebrados.GradoTres && granosQuebrados > caracteristicasCalidadPorMaterial.maiz.gQuebrados.GradoDos) ||
                (materiasExt <= caracteristicasCalidadPorMaterial.maiz.materiasExtrañas.GradoTres && materiasExt > caracteristicasCalidadPorMaterial.maiz.materiasExtrañas.GradoDos)
            );
            esGradoDos =(
                (ph >= caracteristicasCalidadPorMaterial.maiz.pesoHectolitrico.GradoDos && ph < caracteristicasCalidadPorMaterial.maiz.pesoHectolitrico.GradoUno)
                || (granosDañados <= caracteristicasCalidadPorMaterial.maiz.gDañados.GradoDos && granosDañados > caracteristicasCalidadPorMaterial.maiz.gDañados.GradoUno)
                || (granosQuebrados <= caracteristicasCalidadPorMaterial.maiz.gQuebrados.GradoDos && granosQuebrados > caracteristicasCalidadPorMaterial.maiz.gQuebrados.GradoUno)
                || (materiasExt <= caracteristicasCalidadPorMaterial.maiz.materiasExtrañas.GradoDos && materiasExt > caracteristicasCalidadPorMaterial.maiz.materiasExtrañas.GradoUno));

        }

        if (esTrigo) {
            camposTienenValor = camposRequeridosTienenValor(caracteristicasObligatoriasTrigo);

            esGradoCero = ph > caracteristicasCalidadPorMaterial.trigo.pesoHectolitrico.caladoMaximo
                || ph < caracteristicasCalidadPorMaterial.trigo.pesoHectolitrico.GradoTres
                || totalDañados > caracteristicasCalidadPorMaterial.trigo.tDañados.GradoTres
                || totalDañados < 0
                || granosQuebrados > caracteristicasCalidadPorMaterial.trigo.gQuebrados.GradoTres
                || granosQuebrados < 0
                || materiasExt > caracteristicasCalidadPorMaterial.trigo.materiasExtrañas.GradoTres
                || materiasExt < 0
                || ardidos > caracteristicasCalidadPorMaterial.trigo.gArdidos.GradoTres
                || ardidos < 0
                || panzaBlanca > caracteristicasCalidadPorMaterial.trigo.gPanzaBlanca.GradoTres
                || panzaBlanca < 0
                || conCarbon > caracteristicasCalidadPorMaterial.trigo.gConCarbon.GradoTres
                || conCarbon < 0;


            esGradoTres = (
                (ph >= caracteristicasCalidadPorMaterial.trigo.pesoHectolitrico.GradoTres && ph < caracteristicasCalidadPorMaterial.trigo.pesoHectolitrico.GradoDos)
                || (materiasExt <= caracteristicasCalidadPorMaterial.trigo.materiasExtrañas.GradoTres && materiasExt > caracteristicasCalidadPorMaterial.trigo.materiasExtrañas.GradoDos)
                || (ardidos <= caracteristicasCalidadPorMaterial.trigo.gArdidos.GradoTres && ardidos > caracteristicasCalidadPorMaterial.trigo.gArdidos.GradoDos)
                || (totalDañados <= caracteristicasCalidadPorMaterial.trigo.tDañados.GradoTres && totalDañados > caracteristicasCalidadPorMaterial.trigo.tDañados.GradoDos)
                || (conCarbon <= caracteristicasCalidadPorMaterial.trigo.gConCarbon.GradoTres && conCarbon > caracteristicasCalidadPorMaterial.trigo.gConCarbon.GradoDos)
                || (panzaBlanca <= caracteristicasCalidadPorMaterial.trigo.gPanzaBlanca.GradoTres && panzaBlanca > caracteristicasCalidadPorMaterial.trigo.gPanzaBlanca.GradoDos)
                || (granosQuebrados <= caracteristicasCalidadPorMaterial.trigo.gQuebrados.GradoTres && granosQuebrados > caracteristicasCalidadPorMaterial.trigo.gQuebrados.GradoDos)
            );

            esGradoDos = !esGradoTres &&
                (
                (ph >= caracteristicasCalidadPorMaterial.trigo.pesoHectolitrico.GradoDos && ph < caracteristicasCalidadPorMaterial.trigo.pesoHectolitrico.GradoUno)
                || (materiasExt <= caracteristicasCalidadPorMaterial.trigo.materiasExtrañas.GradoDos && materiasExt > caracteristicasCalidadPorMaterial.trigo.materiasExtrañas.GradoUno)
                || (ardidos <= caracteristicasCalidadPorMaterial.trigo.gArdidos.GradoDos && ardidos > caracteristicasCalidadPorMaterial.trigo.gArdidos.GradoUno)
                || (totalDañados <= caracteristicasCalidadPorMaterial.trigo.tDañados.GradoDos && totalDañados > caracteristicasCalidadPorMaterial.trigo.tDañados.GradoUno)
                || (conCarbon <= caracteristicasCalidadPorMaterial.trigo.gConCarbon.GradoDos && conCarbon > caracteristicasCalidadPorMaterial.trigo.gConCarbon.GradoUno)
                || (panzaBlanca <= caracteristicasCalidadPorMaterial.trigo.gPanzaBlanca.GradoDos && panzaBlanca > caracteristicasCalidadPorMaterial.trigo.gPanzaBlanca.GradoUno)
                || (granosQuebrados <= caracteristicasCalidadPorMaterial.trigo.gQuebrados.GradoDos && granosQuebrados > caracteristicasCalidadPorMaterial.trigo.gQuebrados.GradoUno)
                );


        }


        if (camposTienenValor) {
            if (esGradoCero) {
                $(".grado").val(0)
            }else if (esGradoTres) {
                $(".grado").val(3)
            } else if (esGradoDos) {
                $(".grado").val(2)
            } else {
                $(".grado").val(1)
            }

            if($(".grado").val() == 0) {
                var check = $(".grado").parent().parent().parent().find('input.checkBoxCaracteristica');
                $(check).prop('checked', true);
            }
        }
    }
   
}
