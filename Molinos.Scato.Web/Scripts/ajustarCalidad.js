function CaladoPorCaracteristica(id, material, caracteristica, caracteristicaId, valorCalado, rango, esModificable, esAnalisis) {
    if (id == 0) {
        this.Id = id;
        this.Material = material;
        this.Caracteristica = caracteristica;
        this.CaracteristicaId = caracteristicaId;
        this.ValorOriginal = ko.observable(valorCalado);
        this.ValorNuevo = ko.observable(valorCalado);
        this.Rango = rango;
        this.EsModificable = esModificable;
        this.EsAnalisis = esAnalisis;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Material = id.Material;
        this.Caracteristica = id.Caracteristica;
        this.CaracteristicaId = id.CaracteristicaId;
        this.ValorOriginal = ko.observable(id.ValorOriginal);
        this.ValorNuevo = ko.observable(id.ValorNuevo);
        this.Rango = id.Rango;
        this.EsModificable = id.EsModificable;
        this.EsAnalisis = id.EsAnalisis;
    }
    
    //self.getCaracteristicaById = function (id) {
    //    ko.utils.filterArray(self.caladosPorCaracteristica(), function (item) {
    //        return item.id == id;
    //    });
    //};
}

function CaladoPorCaracteristicaListViewModel() {
    // Data
    var self = this;
    self.caladosPorCaracteristica = ko.observableArray([]);
    self.newId = ko.observable();
    self.newRango = ko.observable();
    self.newValorNuevo = ko.observable();

    $.getJSON($("#modificar").data().cargarUrl, { caladoId: $('#hdnCaladoId').val() },
            function (data) {
                cargarCaracteristicas(data.caladosPorCaracteristica);
            }
        );
    

    self.editCaladoPorCaracteristica = function(caladoPorCaracteristica) {
        this.newId = caladoPorCaracteristica.Id;
        this.newRango = caladoPorCaracteristica.Rango;
        $("#rango").text(this.newRango);

        $("#dialogo-editar-guardar").attr("disabled", false);
        $('#dialogo-editar').modal({
            backdrop: 'static',
            keyboard: false
        }).css({
            'top': '40%',
            'left': '50%'
        });
        $('.validation-summary-errors').hide();
        $('.newValorNuevo').val("");
        $('.newValorNuevo').focus();
        $.each(self.caladosPorCaracteristica(), function(index, value) {
            if (caladoPorCaracteristica.Id != value.newId)
            {
                value.newId = null;
            }
        });
    };

    self.confirmCaladoPorCaracteristica = function () {
        //Solo ejecutar confirmar si no hay errores en nuevo valor
        if (!$("#divNuevoValor").hasClass("error")) {
            var valorNuevo = Globalize.parseFloat(this.newValorNuevo());
            $.each(self.caladosPorCaracteristica(), function (index, value) {
                if (value.Id == this.newId) {
                    value.ValorNuevo = valorNuevo;
                    var json = ko.toJSON(value);
                    $.getJSON($("#modificar").data().modificarUrl, { calaldoPorCaracteristica: json, tipoDoc: $("#hdnTipoDoc").val(), numeroDoc: $("#hdnNumeroDoc").val(), caladoId: $("#hdnCaladoId").val() }, function (data) {
                        if (data.resultado == "error") {
                            $.get(this.href, cargarDialogoVer($('#errorModificar')));
                        }
                        $('.newValorNuevo').val('');
                        self.caladosPorCaracteristica.removeAll();
                        cargarCaracteristicas(data.caladosPorCaracteristica);
                    });
                }
            });
            $('#aceptar').removeAttr('disabled');
            $('#dialogo-editar').modal('hide');
        }
    };
    
    function cargarCaracteristicasJson(caracteristicasJson) {
        var caracteristicas = $.map(JSON.parse(caracteristicasJson), function (item) { return new CaladoPorCaracteristica(item); });
        cargarCaracteristicas(caracteristicas);
    }
    function cargarCaracteristicas(caracteristicas) {
        $.each(caracteristicas, function (index, value) {
            self.caladosPorCaracteristica.push(new CaladoPorCaracteristica(value));
        });
    }
}

function reverseFormat(x) {
    return Globalize.parseFloat(x);
};

function formatBoolean(val) {
    return val ? $("#ValorSi").val() : $("#ValorNo").val();
}

function formatFloat(val) {
    if (val != null) {
        return val.toFixed(2).replace(".", Globalize.cultures[Globalize.cultureSelector].numberFormat["."]);
    }
    return "";
}

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

$(document).ready(function () {
    ko.applyBindings(new CaladoPorCaracteristicaListViewModel(), document.getElementById('calado'));

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
    
    ko.bindingHandlers.commaDecimalFormatter = {
        init: function (element, valueAccessor) {
            var observable = valueAccessor();
            var interceptor = ko.computed({
                read: function () {
                    return formatWithComma(observable());
                },
                write: function (newValue) {
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
            var parsedValue = Globalize.parseFloat(value);
            var rangoValores = $('#rango').text().split("-");
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
            element.parentElement.parentElement.className = "control-group";
            return true;
        }
    }, $("#ErrorRangoValor").val());
    


    $("#documentoOrigenHref").click(function() {
        var url = $("#documentoOrigenHref").data().documentoUrl;
        url = UpdateQueryString("instanceId", $("#hdnInstanceId").val(), url);
        url = UpdateQueryString("TipoDocumentoIngreso", $("#hdnTipoDoc").val(), url);
        url = UpdateQueryString("Actividad", "AjustarCalidad", url);
        url = UpdateQueryString("ActividadTitulo", "Ajustar Calidad Ingresada", url);
        url = UpdateQueryString("RecorridoId", $("#hdnRecorridoId").val(), url);
        url = UpdateQueryString("NroDocumento", $("#hdnNumeroDoc").val(), url);
        url = UpdateQueryString("Patente", $("#hdnPatente").val(), url);
        url = UpdateQueryString("EsActividad", false, url);
        window.location = url;
    });
    
    $(window).keydown(function (e) {
        if (e.keyCode == 13)
        e.preventDefault();
    });

});

function UpdateQueryString(key, value, url) {
    if (!url) url = window.location.href;
    var re = new RegExp("([?|&])" + key + "=.*?(&|#|$)(.*)", "gi");

    if (re.test(url)) {
        if (typeof value !== 'undefined' && value !== null)
            return url.replace(re, '$1' + key + "=" + value + '$2$3');
        else {
            var hash = url.split('#');
            url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');
            if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                url += '#' + hash[1];
            return url;
        }
    }
    else {
        if (typeof value !== 'undefined' && value !== null) {
            var separator = url.indexOf('?') !== -1 ? '&' : '?', hash2 = url.split('#');
            url = hash2[0] + separator + key + '=' + value;
            if (typeof hash2[1] !== 'undefined' && hash2[1] !== null)
                url += '#' + hash2[1];
            return url;
        }
        else
            return url;
    }
}