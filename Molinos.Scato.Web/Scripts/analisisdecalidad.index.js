$(document).ready(function () {
    
    //Inicializo Knockout
    var viewModel = new AnalisisPorCaracteristicaListViewModel();
    ko.applyBindings(viewModel);
    ko.bindingHandlers.commaDecimalFormatter = {
        init: function(element, valueAccessor) {
            var observable = valueAccessor();
            var interceptor = ko.computed({
                read: function() {
                    return formatWithComma(observable());
                },
                write: function(newValue) {
                    observable(reverseFormat(newValue));
                }
            });
            if (element.tagName == 'INPUT')
                ko.applyBindingsToNode(element, {
                    value: interceptor
                });
            else
                ko.applyBindingsToNode(element, {
                    text: interceptor
                });
        }
    };

    //Máscaras
    if ($("#TipoVehiculo").val() != 1) {
        $(".patente-internacional").mask("?*******", { placeholder: "" });
    } else {
        $(".patente-internacional").mask("?9999999");
    }
    //Foco en primer elemento
    if ($("#Patente").val().length == 0)
        $("#Patente").focus();
    
    //Valido si ingresó la patente correcta
    $("#Patente").change(function ()
    {
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

    $.validator.addMethod("requerido", function (value, element) {
        return value.length > 0;
    }, $('#ErrorCampoRequerido').val());
    
    $.validator.addMethod("validarNumero", function (value, element) {
        if (value.length > 0) {
            var resultado = $.isNumeric(Globalize.parseFloat(value));
            //Modifico la visualización de la validación
            if (!resultado) {
                element.parentElement.className = "control-group error";
            } else {
                element.parentElement.className = "control-group";
            }
            return resultado;
        } else {
            return true;
        }
    }, $("#ErrorCampoNumerico").val());
    

    $.validator.addMethod("validarRango", function (value, element) {
        if (value.length > 0 && $.isNumeric(Globalize.parseFloat(value))) {
            var rangoValido;
            value = value.toString().replace(Globalize.culture().numberFormat[","], Globalize.culture().numberFormat["."]);
            var parsedValue = Globalize.parseFloat(value);
            var rangoValores = $("#" + element.id).data().rango.split("-");
            if (parsedValue >= Globalize.parseFloat(rangoValores[0]) && parsedValue <= Globalize.parseFloat(rangoValores[1]))
                rangoValido = true;
            else
                rangoValido = false;

            //Modifico la visualización de la validación
            if (!rangoValido) {
                element.parentElement.className = "control-group error";
            } else {
                element.parentElement.className = "control-group";
            }
            return rangoValido;
        } else {
            return true;
        }
    }, $("#ErrorRangoValor").val());

});


// Formatting Functions
function formatWithComma(x, precision) {
    if (!$.isNumeric(x))
        return "";
    var options = {
        precision: precision || 2,
        seperator: Globalize.cultures[Globalize.cultureSelector].numberFormat["."]
    };
    var formatted = parseFloat(x, 10).toFixed(options.precision);
    var regex = new RegExp(
            '^(\\d+)[^\\d](\\d{' + options.precision + '})$');
    formatted = formatted.replace(
        regex, '$1' + options.seperator + '$2');
    return formatted;
};

function reverseFormat(x) {
    return Globalize.parseFloat(x);
};



function InhabilitarPantalla() {
    $("#btnAceptar").attr("disabled", true);
    $("#agregarCaracteristica").attr("disabled", true);
}

function HabilitarPantalla() {
    if ($("#Patente").val().toLowerCase() == $("#PatenteOriginal").val().toLowerCase()) {
        $("#btnAceptar").attr("disabled", false);
        $("#agregarCaracteristica").attr("disabled", false);
   }
}

function AceptarFormulario() {
    $("#btnAceptar").focus();
    if ($("#calidad-form").valid() && $("#PatenteOriginal").val().toLowerCase() == $("#Patente").val().toLowerCase())
        BlockUI($("#calidad-form").data().mensajeEspera);
    
}

function EvaluarResultado(data) {
    if (data.responseText == "OK" || data.responseText == "ErrorActividadYaEjecutada") {
        window.location = $("#ListaDeCamionesUrl").val();
    }
    else {
        $.unblockUI();
        MostrarAlertaError(data.responseText);
    }
}


function AnalisisPorCaracteristicaListViewModel() {
    // Data
    var self = this;
    self.analisisPorCaracteristicaLista = ko.observableArray([]);
    
    ko.numericObservable = function (initialValue) {
        var actual = ko.observable(initialValue);
        var result = ko.dependentObservable({
            read: function () {
                return actual();
            },
            write: function (newValue) {
                var parsedValue = Globalize.parseFloat(newValue);
                actual(isNaN(parsedValue) ? newValue : parsedValue);
            }
        });

        return result;
    };

    //Obtengo las características del calado para análisis
    var guid = window.location.href.substring(window.location.href.lastIndexOf("/") + 1);
    $.getJSON($("#ObtenerCaracteristicasCaladasUrl").val(), { instanceId: guid },
        function (allData) {
            if (allData != "0") {
                
                if ($("#EsTrigoPan").val() == "True") {
                    $.each(allData, function (key, value) {
                        if (value.EsAutomatizable == true) {
                            this.ValorAnalisis = this.ValorCalado;
                        }
                    });
                }
                var mappedCaracteristicas = $.map(allData, function (item) { return new AnalisisPorCaracteristica(item); });
                $.each(mappedCaracteristicas, function (key, value) {
                    this.Eliminable = false;
                });
                self.analisisPorCaracteristicaLista(mappedCaracteristicas);
                
                //Foco en primer elemento
                if (mappedCaracteristicas.length != 0)
                    $("#calidad-form").find(".control:not([readonly='readonly']):enabled:visible:first").focus();
                else {
                    $("#calidad-form").find(":input:not([readonly='readonly']):enabled:visible:first").focus();
                }
            }
        }
    );

    // Operations
    self.addAnalisisPorCaracteristica = function () {
        var existe = false;
        var analisisPorCaracteristicaListaActuales = self.analisisPorCaracteristicaLista();
        $.each(analisisPorCaracteristicaListaActuales, function (key, value) {
            if (this.CaracteristicaId == $("#Caracteristicas").val() && value._destroy != true)
                existe = true;
        });
        
        if (existe)
            $.get(this.href, cargarDialogoVer($('#error')));
        else {
            
            $.getJSON($("#agregarCaracteristica").data().caracteristicaUrl, { caracteristicaId: $("#Caracteristicas").val(), instanceId: guid }, function (data) {
                
                var match = ko.utils.arrayFirst(self.analisisPorCaracteristicaLista(), function (item) {
                    return data.Id === item.CaracteristicaId;
                });
                if (!match) {
                    var valorAnalisis = "";
                    if ($("#EsTrigoPan").val() == "True" && data.EsAutomatizable) {
                        valorAnalisis = data.ValorCalado;
                    }

                    self.analisisPorCaracteristicaLista.push(new AnalisisPorCaracteristica(0, data.Descripcion, data.Id, data.ValorCalado, valorAnalisis, data.UnidadDeMedida.toString(), formatFloat(data.CaladoMinimo).toString() + " - " + formatFloat(data.CaladoMaximo).toString(), data.EsAutomatizable,data.CaladoMinimo, data.CaladoMaximo));
                }
            });
 
        }
    };

    self.removeAnalisisPorCaracteristica = function(analisisPorCaracteristica) {
        self.analisisPorCaracteristicaLista.remove(analisisPorCaracteristica);
    };   
    self.aceptarFormulario = function (formElement) {
        convertToObservable(self.analisisPorCaracteristicaLista());
        $.each(self.analisisPorCaracteristicaLista(), function (key, value) {
            this.ValorAnalisis = reverseFormat(this.newValorAnalisis() != null ? this.newValorAnalisis().replace('.', Globalize.culture().numberFormat["."]) : "");
        });
        $("#AnalisisPorCaracteristicas").val(ko.toJSON(self.analisisPorCaracteristicaLista()));
        $("#btnAceptar").focus();
        if ($("#calidad-form").valid() && $("#PatenteOriginal").val().toLowerCase() == $("#Patente").val().toLowerCase())
            BlockUI($("#calidad-form").data().mensajeEspera);

    };
}
function convertToObservable(list) {
    var newList = [];
    $.each(list, function (i, obj) {
        var newObj = {};
        Object.keys(obj).forEach(function (key) {
            newObj[key] = ko.observable(obj[key]);
        });
        newList.push(newObj);
    });
    return newList;
}

function formatFloat(val) {
    if (val !== null && val !== "" && val !== undefined) {
        return val.toFixed(2).replace(".", Globalize.cultures[Globalize.cultureSelector].numberFormat["."]);
    }
    return "";
}

function AnalisisPorCaracteristica(id, caracteristica, caracteristicaId, valorCalado, valorAnalisis, unidad, rango, esAutomatizable ,min,max) {
    if (id == 0) {
        this.Id = id;
        this.ValorId = "Valor" + caracteristicaId;
        this.Caracteristica = caracteristica;
        this.CaracteristicaId = caracteristicaId;
        this.ValorCalado = ko.observable(valorCalado);

        //if ($("#EsTrigoPan").val() == "True" && esAutomatizable) {
        //    var value = (valorAnalisis.toString() || "").replace(Globalize.culture().numberFormat[","], Globalize.culture().numberFormat["."]);
        //    this.ValorAnalisis = reverseFormat(value);
        //} else {
        //    this.ValorAnalisis = reverseFormat(valorAnalisis.toString());
        //}
        this.newValorAnalisis = ko.observable(formatFloat(valorAnalisis)); 
        this.Unidad = unidad;
        this.Rango = rango;
        this.Eliminable = true;
        this.Min = min;
        this.Max = max;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        
        this.ValorId = "Valor" + id.CaracteristicaId;
        this.Caracteristica = id.Caracteristica;
        this.CaracteristicaId = id.CaracteristicaId;
        this.ValorCalado = ko.observable(id.ValorCalado);
        if ($("#EsTrigoPan").val() == "True" && id.EsAutomatizable) {
            this.newValorAnalisis = ko.observable(formatFloat(id.ValorAnalisis));

            //var value2 = (id.ValorAnalisis.toString() || "").replace(Globalize.culture().numberFormat[","], Globalize.culture().numberFormat["."]);
            //this.ValorAnalisis = reverseFormat(value2);
        } else {
            this.newValorAnalisis = ko.observable(id.newValorAnalisis);
            //this.ValorAnalisis = reverseFormat(id.ValorAnalisis || "");
        }
        this.Unidad = id.Unidad;
        this.Rango = id.Rango;
        this.Eliminable = id.Eliminable;
        this.Min = min;
        this.Max = max;
    }
}






