$(document).ready(function () {
    
    HabilitarCampos(false);
    $('#Patente').on("change", function () {
        if ($('#Patente').val().length >= 6) {
            $("#form-pesar").submit();
        }
    });
    $(".patente-internacional").mask("?*******", {placeholder: ""});

    $("#controlDeBalanzaId").val(0);
    if ($("#Error").val().length > 0) {
        MostrarAlertaError($("#Error").val());
    }
    var model = new PesoListViewModel();
    ko.applyBindings(model, document.getElementById('muestrasDePeso'));
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
    $("#Patente").change(function () {
        model.getMuestras();
    });
    $.validator.addMethod("valorVacio", function (value, element) {
        if (value.length > 0) {
            return ($.isNumeric(Globalize.parseFloat(value)));
        } else {
            return true;
        }
    }, $('#valor').data().errorNumerico);
    ReiniciarBalanza();
    $(".balanza").change(ReiniciarBalanza);
    $('.agregarMuestra').click(function () {
        var nombrePc = ObtenerNombrePC();
        if (nombrePc.length == 0 || nombrePc == "NoTienePuesto") {
            MostrarAlertaError($("#PuestoDeTrabajoErrorMensaje").val());
            return true;
        }
        if (nombrePc != $("#puestoDeTrabajoPc").val()) {
            MostrarAlertaError($("#PuestoDeTrabajoPcBalanzaErrorMensaje").val());
            return true;
        }
        if ($("#modalidad").val() != 0) {
            TomarPeso();
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
    $('#dialogo-editar-muestras-guardar').click(function () {
        if ($('#muestras').valid()) {
            $('#muestras').submit();
        }
    });
    $('#btnFinalizar').click(function () {
        $('#finalizar').val(true);
        $('#btnAceptar').click();
    });
    $('#muestras').submit(function () {
        return false;
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
    $('#btnAceptar').click(function () {
        BlockUI($("#confirmar-form").data().mensajeEspera);
        //Copio datos al otro form
        $("#confirmar-form").submit();
        $("#finalizar").val(false);
    });
});


function EvaluarResultado(data) {
    if (data.responseText != "OK" && data.responseText != "ErrorActividadYaEjecutada") {
        $.unblockUI();
        MostrarAlertaError(data.responseText);
    } else {
        window.location = $("#ListaDeCamionesUrl").val();
    }
}

function HabilitarCampos(value) {
    $("input").prop("disabled", !value);
    $("button").prop("disabled", !value);
    $("select").prop("disabled", !value);
    $("#Patente").prop("disabled", value);
    $(".volver").prop("disabled", false);
}


function ReiniciarBalanza() {
    if ($(".balanza").val() > 0) {
        $("#valor").val("");
        $.getJSON($(".balanza").data().balanzaUrl, { balanzaId: $(".balanza").val() }, function (data) {
            $("#modalidad").val(data.Modalidad);
            $("#puestoDeTrabajoPc").val(data.PuestoDeTrabajo);
            $("#balanzaId").val($(".balanza").val());
        });
    }
}
function TomarPeso() {
    //Toma el peso desde el orquestador
    var label = $(".agregarMuestra").html();
    $(".agregarMuestra").html($(".agregarMuestra").data().mensajeEsperar);
    $(".agregarMuestra").attr("disabled", true);
    $.getJSON($(".agregarMuestra").data().pesoUrl, { balanzaId: $(".balanza").val() }, function (data) {
        if ($.isNumeric(data)) {
            $("#valor").val(formatFloat(data));
        } else { //Devolvió error
            MostrarAlertaError(data);
        }
    }).complete(function () {
        $(".agregarMuestra").html(label);
        $(".agregarMuestra").attr("disabled", false);
        $('#dialogo-editar-muestras-guardar').click();
    });
}

function PesoMuestra(id, balanza, balanzaId, valor) {
    if (id == 0) {
        this.Id = id;
        this.Peso = Math.ceil(Globalize.parseFloat(valor));
        var date = new Date();
        this.Fecha = "/Date(" + date.getTime() + ")/";
        this.FechaFormateada = Globalize.format(date, 'd');
        this.HoraFormateada = Globalize.format(date, 't');
        this.BalanzaNombre = balanza;
        this.BalanzaId = parseInt(balanzaId);
  } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Peso = id.Peso;
        this.Fecha = id.Fecha;
        this.FechaFormateada = Globalize.format(new Date(parseInt(id.Fecha.replace('/Date(', ''))), 'd');
        this.HoraFormateada = Globalize.format(new Date(parseInt(id.Fecha.replace('/Date(', ''))), 't');
        this.BalanzaNombre = id.BalanzaNombre;
        this.BalanzaId = id.BalanzaId;
    }
}

function PesoListViewModel() {
    // Data
    var self = this;
    self.muestras = ko.observableArray([]);
    self.newValor = ko.observable();
    
    // Operations
    self.addMuestra = function () {
        var match;
        ko.utils.arrayFirst(self.muestras(), function (item) {
            if (item.BalanzaId == $(".balanza").val()) {
                match = item;
            }
        });
        if (self.newValor() > "") {
            self.muestras.push(new PesoMuestra(0, $(".balanza option:selected").text(), $(".balanza").val(), this.newValor()));
            self.newValor("");
            if (match) {
                self.muestras.remove(match);
            }
        } else if (!self.newValor() && $('#valor').val() > "") {
            self.muestras.push(new PesoMuestra(0, $(".balanza option:selected").text(), $(".balanza").val(), $('#valor').val()));
            $('#valor').val("");
            if (match) {
                self.muestras.remove(match);
            }
        }
        $('#dialogo-editar-muestras').modal('hide');
        ActualizarDiferencias(self.muestras());
    };
    self.getMuestras = function () {
        if ($("#Patente").val().length >= 6) {
            $.getJSON($("#ObtenerPesadasUrl").val(), { patente: $("#Patente").val() },
                function (allData) {
                    if (allData != "0") {
                        var mappedPesos = $.map(allData, function (item) { return new PesoMuestra(item); });
                        self.muestras(mappedPesos);
                        ActualizarDiferencias(self.muestras());
                    }
                }
            );
        }
    };



}

function ActualizarDiferencias(muestras) {
    var diferencias = '';
    var balanzas = [];
    $.each(muestras, function () {
        var muestra = this;
        $.each(muestras, function () {
            if (this.BalanzaId != muestra.BalanzaId && $.inArray(this.BalanzaId + muestra.BalanzaId, balanzas) == -1) {
                diferencias += $("#DiferenciaMensaje").val().toString().replace("BALANZA1", muestra.BalanzaNombre).replace("BALANZA2", this.BalanzaNombre).replace("PESO", muestra.Peso - this.Peso) + "<br/>";
                balanzas.push(this.BalanzaId + muestra.BalanzaId);
            }
        });
    });
    $("#mostrarDiferencias").html(diferencias);
}

function formatFloat(val) {
    if (val != null) {
        return Math.ceil(val).toFixed(2).replace(Globalize.cultures[Globalize.cultureSelector].numberFormat[","], Globalize.cultures[Globalize.cultureSelector].numberFormat["."]);
    }
    return "";
}


function Completado() {
    $.unblockUI();
    //Mostrar error de modelo
    if ($("#Error").val().length > 0) {
        MostrarAlertaError($("#Error").val());
    } else {
        if ($("#RecorridoId").val() > 0) {
            HabilitarCampos(true);
            //Copio datos al otro formulario
            $("#observaciones").val($("#Observaciones").val());
            $("#instanceId").val($("#InstanciaWorkflow").val());
            $("#controlDeBalanzaId").val($("#ControlDeBalanzaId").val());
            $("#tipoPesada").val($("#TipoPesada").val());
        } else {
            HabilitarCampos(false);
        }
    }
}