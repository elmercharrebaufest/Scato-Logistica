function HabilitarPantalla() {
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

var escuchar = false;
function Humedimetro(intervalo, segundos) {
    if (escuchar) {
        TomarHumedad();
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
    $("#cladoEnPlantaForm").html($('#form').html());
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

$(document).ready(function () {
    $('input:not([readonly="readonly"]):enabled:visible:first').focus();
    HabilitarPantalla();

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
});
