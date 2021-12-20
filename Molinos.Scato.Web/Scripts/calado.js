function CaladoPorCaracteristica(id, caracteristica, caracteristicaId, analisisPreliminar, envioACamara, valorCalado, unidad, rango, esHumedad) {
    if (id == 0) {
        this.Id = id;
        this.Caracteristica = caracteristica;
        this.CaracteristicaId = caracteristicaId;
        this.AnalisisPreliminar = analisisPreliminar;
        this.EnvioACamara = envioACamara;
        this.ValorCalado = valorCalado;
        this.Unidad = unidad;
        this.Rango = rango;
        this.Humedimetro = "";
        this.EsHumedad = esHumedad;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Caracteristica = id.Caracteristica;
        this.CaracteristicaId = id.CaracteristicaId;
        this.AnalisisPreliminar = id.AnalisisPreliminar;
        this.EnvioACamara = id.EnvioACamara;
        this.ValorCalado = id.ValorCalado;
        this.Unidad = id.Unidad;
        this.Rango = id.Rango;
        this.EsHumedad = id.EsHumedad;
    }
}

function CaladoPorCaracteristicaListViewModel() {
    // Data
    var self = this;
    self.caladosPorCaracteristica = ko.observableArray([]);
    self.newCaracteristicaId = ko.observable();
    self.newAnalisisPreliminar = ko.observable(false),
    self.newEnvioACamara = ko.observable(false);
    self.newValorCalado = ko.observable();
    self.newUnidad = ko.observable();
    self.newRango = ko.observable();

    // Operations
    self.addCaladoPorCaracteristica = function () {
        if (!(!this.newValorCalado() && $('#caladoObligatorio').val() == "true") && $.isNumeric(Globalize.parseFloat($('#valorCalado').val()))
            && Globalize.parseFloat(($('#valorCalado').val())) >= Globalize.parseFloat($('#valorMin').val())
            && Globalize.parseFloat(($('#valorCalado').val())) <= Globalize.parseFloat($('#valorMax').val())) {
            var existe = false;
            var caladosPorCaracteristicaActuales = self.caladosPorCaracteristica();
            $.each(caladosPorCaracteristicaActuales, function (index, value) {
                if (value.Caracteristica == $('#caracteristica option:selected').text() && value._destroy != true) {
                    existe = true;
                }
            });
            if (!existe) {
                var valor;
                if (!(this.newValorCalado()) || (Globalize.parseFloat(this.newValorCalado()) < 0)) {
                    valor = null;
                } else {
                    valor = Globalize.parseFloat(this.newValorCalado());
                }
                if (!(this.newAnalisisPreliminar())) {
                    this.newAnalisisPreliminar(false);
                }
                if (!(this.newEnvioACamara())) {
                    this.newEnvioACamara(false);
                }
                self.caladosPorCaracteristica.push(new CaladoPorCaracteristica(0, $('#caracteristica option:selected').text(), this.newCaracteristicaId(), this.newAnalisisPreliminar(), $('#envioACamara').prop('checked'), valor, $('#unidad').val(), $('#rango').val(),false));
                self.newAnalisisPreliminar(false);
                self.newEnvioACamara(false);
                self.newUnidad("");
                self.newRango("");
            } else {
                $.get(this.href, cargarDialogoVer($('#error')));
            }
            if (self.EsHumedad) $('#aceptar').removeAttr('disabled');
            $('#dialogo-editar').modal('hide');
        }
    };

    self.removeCaladoPorCaracteristica = function(caladoPorCaracteristica) {
        self.caladosPorCaracteristica.remove(function (d) {
            if (d.EsHumedad && d.CaracteristicaId == caladoPorCaracteristica.CaracteristicaId) {
                $('#muestrasTomadas').val("[]");
                $('#muestraElegida').val("");
            }
            return d.CaracteristicaId == caladoPorCaracteristica.CaracteristicaId;
        });
        if (caladoPorCaracteristica.CaracteristicaId == $('#humedadId').val()) {
            $('.tomarHumedad').removeAttr('disabled');
        }
        var vacio = true;
        $.each(self.caladosPorCaracteristica(), function (index, value) {
            if (value._destroy != true && value.EsHumedad) {
                vacio = false;
            }
        });
        if (vacio) {
            $('#aceptar').attr('disabled', 'disabled');
        }
    };
}

function InhabilitarPantalla() {
    $(".agregar").attr("disabled", true);
    $(".aceptar").attr("disabled", true);
    $(".tomarHumedad").attr("disabled", true);
}

function HabilitarPantalla() {
    if ($(".patente").val().toLowerCase() == $(".patenteOriginal").val().toLowerCase()) {
        $(".agregar").attr("disabled", false);
        $(".tomarHumedad").attr("disabled", false);
    }
}

function MuestraDeHumedad(id, numero, valor, fecha, fechaFormateada, humedimetro) {
    if (id == 0) {
        this.Id = id;
        this.NumeroMuestra = numero;
        this.Valor = Globalize.parseFloat(valor);
        this.Fecha = fecha;
        this.FechaFormateada = fechaFormateada;
        this.NombreHumedimetro = humedimetro;
        this.ValorId = "muestra" + numero;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.NumeroMuestra = id.Numero;
        this.Valor = Globalize.parseFloat(id.Valor);
        this.Fecha = id.Fecha;
        this.FechaFormateada = id.Fecha.toString();
        this.NombreHumedimetro = id.Humedimetro;
        this.ValorId = "muestra" + id.Numero;
    }
}

function MuestrarDeHumedadListViewModel() {
    // Data
    var self = this;
    self.muestras = ko.observableArray([]);
    self.newValor = ko.observable();

    // Operations
    self.addMuestra = function() {
        var fecha = new Date();
        var fechaFormateada = Globalize.format(fecha, 'd') + ", " + Globalize.format(fecha, 't');
        if (self.newValor() > "") {
            self.muestras.push(new MuestraDeHumedad(0, this.muestras().length + 1, this.newValor(), "/Date(" + (fecha.getTime()) + ")/", fechaFormateada, $('.humedimetro option:selected').text()));
            self.newValor("");
            $('#maxVal').val(this.muestras().length);
        } else if (!self.newValor() && $('#valor').val() > "") {
            self.muestras.push(new MuestraDeHumedad(0, this.muestras().length + 1, $('#valor').val(), "/Date(" + (fecha.getTime()) + ")/", fechaFormateada, $('.humedimetro option:selected').text()));
            $('#valor').val("");
            $('#maxVal').val(this.muestras().length);
        }
        $('#dialogo-editar-muestras').modal('hide');
    };
}

function ReiniciarHumedimetro() {
    if ($(".humedimetro").val() > 0) {
        $("#valor").val("");
        $.getJSON($(".humedimetro").data().humedimetroUrl, { humedimetroId: $(".humedimetro").val() }, function (data) {
            $("#modalidad").val(data.Modalidad);
        });
    }
}

function TomarHumedad() {
    //Toma el humedad desde el orquestador
    var label = $(".agregarMuestra").html();
    $(".agregarMuestra").html($(".agregarMuestra").data().mensajeEsperar);
    $(".agregarMuestra").attr("disabled", true);
    $.getJSON($(".agregarMuestra").data().humedadUrl, { humedimetroId: $(".humedimetro").val() }, function (data) {
        if ($.isNumeric(data)) {
            $("#valor").val(formatFloat(data));
        } else { //Devolvió error
            MostrarAlertaError(data);
        }
    }).complete(function() {
        $(".agregarMuestra").html(label);
        $(".agregarMuestra").attr("disabled", false);
        $('#dialogo-editar-muestras-guardar').click();
    });
}

function LeerCaracteristica() {
    if ($("#caracteristica").val() > 0) {
        $.getJSON($("#caracteristica").data().caracteristicaUrl, { caracteristicaId: $("#caracteristica").val() }, function (data) {
            $('#valorMin').val(formatFloat(data.RangoMin));
            $('#valorMax').val(formatFloat(data.RangoMax));
            $('#rango').val(formatFloat(data.RangoMin).toString().replace() + " - " + formatFloat(data.RangoMax).toString());
            $('#unidad').val(data.Unidad.toString());
            if (data.CargaEnCalado == true) {
                $('#caladoObligatorio').val("true");
            } else {
                $('#caladoObligatorio').val("false");
            }
            if (data.EnvioACamara == 0) {
                $('#envioACamara').prop('checked', true);
                $('#envioACamara').attr('disabled', 'disabled');
            }
            if (data.EnvioACamara == 1) {
                $('#envioACamara').prop('checked', false);
                $('#envioACamara').attr('disabled', 'disabled');
            }
            if (data.EnvioACamara == 2) {
                $('#envioACamara').removeAttr('disabled');
            }
        });
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
}

$(document).ready(function () {
    var viewModel = new CaladoPorCaracteristicaListViewModel();
    ko.applyBindings(viewModel, document.getElementById('calado'));
    
    //Foco en primer elemento
    if ($("#patente").val().length == 0)
        $("#patente").focus();


    ko.applyBindings(new MuestrarDeHumedadListViewModel(), document.getElementById('muestrasDeHumedad'));


    ko.bindingHandlers.formattedBoolean = {
        update: function(element, valueAccessor) {
            ko.bindingHandlers.text.update(element, function () { return formatBoolean(valueAccessor()); });
        }
    };
    
    ko.bindingHandlers.formattedFloat = {
        update: function (element, valueAccessor) {
            ko.bindingHandlers.text.update(element, function () { return formatFloat(valueAccessor()); });
        }
    };

    ko.extenders.formatted = function (target, callback) {
        target.formatted = ko.dependentObservable(function () {
            return callback(ko.utils.unwrapObservable(target));
        });
        return target;
    };
 
    $.validator.addMethod("CaladoObligatorias", function (value, element) {
        if ($('#caracteristicasObligatorias').val() == "") {
            return true;
        }
        var obligatorias = $('#caracteristicasObligatorias').val().split(',');
        var vmcaracteristicas = ko.dataFor(document.getElementById('calado'));
        var cargadas = new Array();
        $.each(vmcaracteristicas.caladosPorCaracteristica(), function (index, value2) {
            if (value2._destroy != true) {
                cargadas.push(value2.CaracteristicaId);
            }
        });
        var valido = true;
        $.each(obligatorias, function (index, value3) {
            if ($.inArray(value3, cargadas) == -1) {
                valido = false;
            }
        });
        return valido;
    }, $('#CaladoObligatorias').data().errorObligatorias);

    $.validator.addMethod("valorCaladoVacio", function (value, element) {
        if ($('#valorCalado').val().length > 0 /*&& $('#dialogo-editar').hasClass('in')*/) {
            return ($.isNumeric(Globalize.parseFloat($('#valorCalado').val()))
                && Globalize.parseFloat(($('#valorCalado').val())) >= Globalize.parseFloat($('#valorMin').val())
                && Globalize.parseFloat(($('#valorCalado').val())) <= Globalize.parseFloat($('#valorMax').val()));
        } else {
            return ($('#caladoObligatorio').val() == "false");
        }
    }, $('#valorCalado').data().errorNumerico);

    $.validator.addMethod("numeroSeleccionadaVacio", function (value, element) {
        if ($('#numeroSeleccionada').val().length > 0) {
            return ($.isNumeric($('#numeroSeleccionada').val()) && $('#numeroSeleccionada').val() >= 1 && $('#numeroSeleccionada').val() <= $('#maxVal').val() && parseInt($('#numeroSeleccionada').val()).toString() == $('#numeroSeleccionada').val());
        } else {
            return false;
        }
    }, $('#numeroSeleccionada').data().errorRango);

    $.validator.addMethod("valorVacio", function (value, element) {
        if ($('#valor').val().length > 0) {
            return ($.isNumeric(Globalize.parseFloat($('#valor').val())));
        } else {
            return true;
        }
    }, $('#valor').data().errorNumerico);
    
    $.validator.addMethod("validarRango", function (value, element) {
        if ($("#muestra" + value).length > 0) {
            var rangoValido;
            var parsedValue = Globalize.parseFloat($("#muestra" + value).text());
            var rangoValores = $("#humedadRango").val().split("-");
            if (parsedValue >= Globalize.parseFloat(rangoValores[0]) && parsedValue <= Globalize.parseFloat(rangoValores[1]))
                rangoValido = true;
            else
                rangoValido = false;
            //Modifico la visualización de la validación
            if (!rangoValido) {
                element.parentElement.parentElement.className = "control-group error";
            } else {
                element.parentElement.parentElement.className = "control-group";
            }
            return rangoValido;
        } else {
            return true;
        }
    }, $("#ErrorRangoValor").val());

    $('#muestrasDeHumedad').hide();
    if ($("#TipoVehiculo").val() != 1) {
        $(".patente-internacional").mask("?*******", {placeholder: ""});
    } else {
        $(".patente-internacional").mask("?9999999");
    }
    ReiniciarHumedimetro();
    $(".humedimetro").change(ReiniciarHumedimetro);

    $(document).on('change', '#caracteristica', (function () {
        LeerCaracteristica();
    }));

    $('.agregar').click(function () {
        $('#valorCalado').val('');
        LeerCaracteristica();
        $("#dialogo-editar-guardar").attr("disabled", false);
        $('#dialogo-editar').modal({
            backdrop: 'static',
            keyboard: false
        }).css({
            'top': '50%',
            'margin-top': function() {
                return -($(this).height() / 2);
            }
        });        
        $('.validation-summary-errors').hide();
        $(".error").removeClass("error");
        $(".field-validation-error").html("");
        $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
    });
    
    $('#envioACamara').change(function () {
        viewModel.newAnalisisPreliminar(false);
    });
    
    $('#analisisPreliminar').change(function () {
        viewModel.newEnvioACamara(false);
    });

    //si es egreso deshabilito el rechazo
    //if ($('#esEgreso').val() == "True") {
    //    $('.rechazar').attr('disabled', 'disabled');
    //}
    
    $(document).on('click', '.rechazar-boton', function () {
        $.get(this.href, cargarDialogoRechazar);
        return false;
    });
    $(document).on('click', '.dialogo-rechazar-cerrar', function () {
        $("#dialogo-rechazar").modal('hide');
        $("#mensajeRechazar").html("");
        return false;
    });
    ////
    
    //Valido si ingresó la patente correcta
    $(".patente").change(function () {
        $.getJSON($(".patente").data().patenteUrl, { patente: $(".patente").val(), patenteOriginal: $(".patenteOriginal").val() }, function (data) {
            if (data.resultado == "OK")
                HabilitarPantalla();
            else {
                window.location = data.url;
            }
        });
    });

    //Inhabilito pantalla hasta que se ingrese una patente correcta
    if ($(".patente").val().toLowerCase() != $(".patenteOriginal").val().toLowerCase())
        InhabilitarPantalla();

    $(".patente").keyup(function () {
        if ($(".patente").val().toLowerCase() != $(".patenteOriginal").val().toLowerCase())
            InhabilitarPantalla();
    });
    
    $('.agregarMuestra').click(function () {
        if ($("#modalidad").val() != 0) {
            TomarHumedad();
        } else {
            $("#dialogo-editar-muestras-guardar").attr("disabled", false);
            $('#dialogo-editar-muestras').modal({
                backdrop: 'static',
                keyboard: false
            }).css({
                'top': '50%',
                'margin-top': function() {
                    return -($(this).height() / 2);
                }
            });
        }
    });
    
    $('#dialogo-editar-muestras').on('shown', function () {
        $(this).find('.modal-body').find(':input:enabled:visible:first').focus();
    });

    $('#dialogo-editar-muestras-cancelar').click(function () {
        $('#valor').val("");
        $('#dialogo-editar-muestras').modal('hide');
        $(".field-validation-error").html("");
        $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
    });

    $('#dialogo-editar-muestras-guardar').click(function () {
        if ($('#muestras').valid()) {
            $('#dialogo-editar-muestras form').submit();
        }
    });
    
    $('#dialogo-editar-cancelar').click(function () {
        $('#valorCalado').val("");
        $(".error").removeClass("error");
        $(".field-validation-error").html("");
        $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
    });

    $('.cancelar').click(function () {
        $('#muestrasTomadas').val("[]");
        var vm = ko.dataFor(document.getElementById('muestrasDeHumedad'));
        vm.muestras.removeAll();
        $('#calado').show();
        $('#muestrasDeHumedad').hide();
        $(".field-validation-error").html("");
        $(".field-validation-error").removeClass("field-validation-error").addClass("field-validation-valid");
    });

    $('.tomarHumedad').click(function () {
        $('#calado').hide();
        $('#muestrasDeHumedad').show();
        $('.validation-summary-errors').hide();
    });
    $('#form-muestras').submit(function () {
        var focused = $(':focus');
        if (focused.attr('id') == 'numeroSeleccionada') {
            $('.aceptar-muestra').click();
        } else {
            $(focused).click();
        }
        return false;
    });
    $('.aceptar-muestra').click(function () {
        $('#form-muestras').validate();
        var valid = $('#form-muestras').valid();
        //jQuery.validator.methods.numeroSeleccionadaVacio.call(this, value, element);
        if (valid) {
            var vm = ko.dataFor(document.getElementById('muestrasDeHumedad'));
            $('#muestraElegida').val($('#numeroSeleccionada').val());
            if (vm.muestras().length > 0) {
                $('#muestrasTomadas').val(ko.toJSON(vm.muestras()));
            }
            
            //agregar caracteristica humedad
            //busco el valor de la muestra elegida
            var valorHumedad = 0;
            var muestrasFinales = vm.muestras();
            $.each(muestrasFinales, function (index, value) {
                if (value.NumeroMuestra == $('#muestraElegida').val()) {
                    valorHumedad = value.Valor;
                }
            });
            
            var envioACamara = false;
            $.ajax({
                url: $("#caracteristica").data().caracteristicaUrl,
                dataType: "json",
                data: { caracteristicaId: $("#humedadId").val() },
                success: function (data) {
                    if (data.EnvioACamara == 0) {
                        envioACamara = true;
                    }

                    var vmcaracteristicas = ko.dataFor(document.getElementById('calado'));

                    var match = ko.utils.arrayFirst(vmcaracteristicas.caladosPorCaracteristica(), function (item) {
                        return $('#humedadId').val() === item.CaracteristicaId;
                    });
                    if (!match) {
                        vmcaracteristicas.caladosPorCaracteristica.push(new CaladoPorCaracteristica(0, $('#humedadDescripcion').val(), $('#humedadId').val(), false, envioACamara, valorHumedad, $('#humedadUnidad').val(), $('#humedadRango').val(), true));
                    }
                    
                    $('.tomarHumedad').attr('disabled', 'disabled');
                    $('#aceptar').removeAttr('disabled');

                    $('#calado').show();
                    $('#muestrasDeHumedad').hide();
                }
            });
        }
    });

    $('#aceptar').attr('disabled', 'disabled');
    $('#aceptar').click(function () {
        if (!($(this).closest('form').valid())) {
            $('.validation-summary-errors').show();
        }
    });
    
    if ($('#humedadId').val() == 0) {
        $('.tomarHumedad').attr('disabled', 'disabled');
        $('.agregar').attr('disabled', 'disabled');
        $('.rechazar').attr('disabled', 'disabled');
        $('#sinHumedad').removeClass('hidden');
    }
});
