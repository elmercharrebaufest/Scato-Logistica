function Descuento(id, valorDesde, valorHasta, porcentajeDescuento, porcentajeEnvioCamaraAuditoria, mercadoATermino) {
    if (id == 0) {
        this.Id = id;
        this.ValorDesde = ko.observable(valorDesde);
        this.ValorHasta = valorHasta;
        this.PorcentajeDescuento = porcentajeDescuento;
        this.PorcentajeEnvioCamaraAuditoria = porcentajeEnvioCamaraAuditoria;
        this.MercadoATermino = mercadoATermino;
        this.CaracteristicaDeCalidadId = $("#CaracteristicaId").val();
        this.EsNuevo = true;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.ValorDesde = ko.observable("0");
        this.ValorHasta = id.ValorHasta.toString();
        this.PorcentajeDescuento = id.PorcentajeDescuento.toString();
        this.PorcentajeEnvioCamaraAuditoria = id.PorcentajeEnvioCamaraAuditoria.toString();
        this.MercadoATermino = id.MercadoATermino.toString() == "true" ? true : false;
        this.CaracteristicaDeCalidadId = id.CaracteristicaDeCalidadId;
        this.EsNuevo = id.EsNuevo;
        this._destroy = id._destroy;
    }
}
function comparaDescuentos(d1, d2) {
    if (d1.ValorHasta == d2.ValorHasta) return 0;
    return Globalize.parseFloat(d1.ValorHasta) > Globalize.parseFloat(d2.ValorHasta) ? 1 : -1;
}

function actualizarValoresDesde(descuentos) {
    //Actualiza la columna de Valor Desde (los descuentos deben estar ordenados!)
    var anterior = null;
    $.each(descuentos, function (index, value) {
        if (value._destroy != true) {
            if (anterior == null) value.ValorDesde($("#CaladoMinimo").val());
            else {
                value.ValorDesde(anterior.ValorHasta);
            }
            anterior = value;
        }
    });
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

function reverseFormat(x) {
    return Globalize.parseFloat(x);
};

function DescuentoListViewModel() {
    // Inicializo observers
    var self = this;
    self.descuentos = ko.observableArray([]);
    self.newPorcentajeDescuento = ko.observable();
    self.newPorcentajeEnvioCamaraAuditoria = ko.observable();
    self.newMercadoATermino = ko.observable();
    self.newValorHasta = ko.observable();
    self.newValorDesde = ko.observable();

    // Obtengo objetos Descuento del dominio
    if ($("#descuentosPostBack").val() != "") {
        //Si es un postback del controller: (modelo invalido)
        var mappedDescuentos = $.map(JSON.parse($("#descuentosPostBack").val()), function (item) { return new Descuento(item); });
        $.each(mappedDescuentos, function (index, value) {
            value.ValorHasta = value.ValorHasta.toString().replace(".", Globalize.culture().numberFormat["."]);
            value.PorcentajeDescuento = value.PorcentajeDescuento.toString().replace(".", Globalize.culture().numberFormat["."]);
            value.PorcentajeEnvioCamaraAuditoria = value.PorcentajeEnvioCamaraAuditoria.toString().replace(".", Globalize.culture().numberFormat["."]);
        });

        self.descuentos(mappedDescuentos);
        self.descuentos.sort(comparaDescuentos);
        actualizarValoresDesde(mappedDescuentos);
    } else {
        //Si es la primera vez que entramos:
        $.getJSON($("#botonCrearDescuento").data().obtenerDescuentos, { caracteristicaId: $('#CaracteristicaId').val() },
            function (allData) {
                var mappedDescuentos = $.map(allData, function (item) { return new Descuento(item); });
                $.each(mappedDescuentos, function (index, value) {
                    value.ValorHasta = value.ValorHasta.toString().replace(".", Globalize.culture().numberFormat["."]);
                    value.PorcentajeDescuento = value.PorcentajeDescuento.toString().replace(".", Globalize.culture().numberFormat["."]);
                    value.PorcentajeEnvioCamaraAuditoria = value.PorcentajeEnvioCamaraAuditoria.toString().replace(".", Globalize.culture().numberFormat["."]);
                });

                self.descuentos(mappedDescuentos);
                self.descuentos.sort(comparaDescuentos);
                actualizarValoresDesde(mappedDescuentos);
            }
        );
    }
    
    // Operations
    self.botonCrearDescuento = function () {
        // Validaciones
        if (!self.newPorcentajeEnvioCamaraAuditoria() == "") {
            $('#porcentajeEnvioCamaraAuditoria').val(0);
        }

        if ($("#CaladoMinimo").val() == "" || $("#CaladoMaximo").val() == "") $("#orden-form").submit();
        else if (!self.newValorHasta() > "") {
            $('#valorHasta').focus();
        }
        else if (!self.newPorcentajeDescuento() > "") {
            $('#porcentajeDescuento').focus();
        }
        else {
            var invalido = false;
            if (!$.isNumeric(Globalize.parseFloat($("#CaladoMaximo").val()))) {
                $('#CaladoMaximo').focus();
            }
            else if (!$.isNumeric(Globalize.parseFloat($("#CaladoMinimo").val()))) {
                $('#CaladoMinimo').focus();
            }
            else if (!$.isNumeric(Globalize.parseFloat(self.newValorHasta()))) {
                $('#valorHasta').focus();
            }
            else if (!$.isNumeric(Globalize.parseFloat(self.newPorcentajeDescuento()))) {
                $('#porcentajeDescuento').focus();
            }
            else if (Globalize.parseFloat(self.newPorcentajeDescuento()) > 100) {
                $('#porcentajeDescuento').focus();
            }
            else if (Globalize.parseFloat(self.newPorcentajeEnvioCamaraAuditoria()) > 100) {
                $('#porcentajeEnvioCamaraAuditoria').focus();
            }
            else if (Globalize.parseFloat($("#CaladoMaximo").val()) < Globalize.parseFloat(self.newValorHasta())) {
                $('#valorHasta').focus();
            }
            else if (Globalize.parseFloat($("#CaladoMinimo").val()) > Globalize.parseFloat(self.newValorHasta())) {
                $('#valorHasta').focus();
            }
            else {
                $.each(self.descuentos(), function (index, value) {
                    if (value.ValorHasta == self.newValorHasta() && value._destroy != true) {
                        invalido = true;
                    }
                });
                // Si es valido, cargo el descuento
                if (!invalido) {
               
                    self.descuentos.push(new Descuento(0, self.newValorDesde(), self.newValorHasta(), self.newPorcentajeDescuento(), self.newPorcentajeEnvioCamaraAuditoria(), self.newMercadoATermino()));
                    self.descuentos.sort(comparaDescuentos);

                    actualizarValoresDesde(self.descuentos());
                
                    self.newValorHasta("");
                    self.newPorcentajeDescuento("");
                    self.newPorcentajeEnvioCamaraAuditoria("");
                }

                $('#valorHasta').focus();
            }
        }
    };
    
    self.removeDescuento = function (descuento) {
        self.descuentos.destroy(descuento);
        actualizarValoresDesde(self.descuentos());
    };

    self.actualizarNewValorDesde = function () {
        //Actualiza el textbox Valor Desde a medida que uno escribe Valor Hasta
        var estado = false;
        var ultimoHasta = $("#CaladoMinimo").val();
        $.each(self.descuentos(), function (index, value) {
            if (value._destroy != true) {
                if (estado == false && Globalize.parseFloat($('#valorHasta').val()) < Globalize.parseFloat(value.ValorHasta)) {
                    estado = true;
                    self.newValorDesde(value.ValorDesde());
                } else {
                    ultimoHasta = value.ValorHasta;
                }
            }
        });
        
        if (estado == false) {
            self.newValorDesde(ultimoHasta);
        }
    };

}

$(document).ready(function () {
    
    ko.applyBindings(new DescuentoListViewModel(), document.getElementById('grilla'));
    //ko.applyBindings(new DescuentoListViewModel());

    //ko.bindingHandlers.commaDecimalFormatter = {
    //    init: function (element, valueAccessor) {
    //        var observable = valueAccessor();
    //        var interceptor = ko.computed({
    //            read: function () {
    //                return formatWithComma(observable);
    //            },
    //            write: function (newValue) {
    //                observable(reverseFormat(newValue));
    //            }
    //        });
    //        if (element.tagName == 'INPUT')
    //            ko.applyBindingsToNode(element, {
    //                value: interceptor
    //            });
    //        else
    //            ko.applyBindingsToNode(element, {
    //                text: interceptor
    //            });
    //    }
    //};  
    $('#NoAceptarSiSeDefineUnValor').change(function () {
        setearTextboxTolerancia();
    });

    setearTextboxValor();
    $('#SituacionEnvioACamara').change(function () {
        setearTextboxValor();
    });

    $('#DescuentoEnPorcentaje').change(function () {
        if ($('#DescuentoEnPorcentaje option:selected').val() == 'SinDescuento')
            $('#grilla').hide();
        else {
            $('#grilla').show();
        }
    });
    
    $('#valorDesde').val($("#CaladoMinimo").val());
    $("#CaladoMinimo").change(function() {
        $('#valorDesde').val($("#CaladoMinimo").val());
    });

    $(document).on('click', '#cargaEnCalado', (function () {
        if ($('#obligatorio').prop('checked')) {
            return false;
        }
    }));

    if ($('#obligatorio').prop('checked')) {
        $('#cargaEnCalado').prop('checked', true);
        $('#cargaEnCalado').attr('readonly', 'readonly');
    } else {
        $('#cargaEnCalado').removeAttr('readonly');
    }

    $(document).on('change', '#obligatorio', (function () {
        if ($('#obligatorio').prop('checked')) {
            $('#cargaEnCalado').prop('checked', true);
            $('#cargaEnCalado').attr('readonly', 'readonly');
        } else {
            $('#cargaEnCalado').removeAttr('readonly');
        }
    }));
    
    $('.form-horizontal').submit(function () {

        var vm = ko.dataFor(document.getElementById('grilla'));
        var descuentos = vm.descuentos();
        $.each(descuentos, function (index, value) {
            value.ValorHasta = Globalize.parseFloat(value.ValorHasta.toString());
            value.PorcentajeDescuento = Globalize.parseFloat(value.PorcentajeDescuento.toString());
            value.PorcentajeEnvioCamaraAuditoria = Globalize.parseFloat(value.PorcentajeEnvioCamaraAuditoria.toString());
        });

        $('#descuentos').val(ko.toJSON(descuentos));
        
        /*$('#dialogo-editar form').submit();
        if ($('#dialogo-editar form').valid())
            $("#dialogo-editar-guardar").attr("disabled", true);*/
    });
    
    DefinirAutocompletar('#MaterialDescripcion', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    setearTextboxTolerancia();
    valoresEspecialesRequeridos();
});

function setearTextboxValor() {
    if ($('#SituacionEnvioACamara option:selected').val() == 'SiempreSiSuperaValorCamara') {
        $('#valorCamara').show();
    }
    else {
        $('#valorCamara').hide();
        $("#SiSuperaValorCamara").val('');
    }
}

function setearTextboxTolerancia() {
        //Ahora si se activa el check y tiene un valor en tolerancia, el vehículo no es aceptable solo si supera la tolerancia
        //if ($('#NoAceptarSiSeDefineUnValor').is(':checked')) {
        //    $('#ToleranciaSinAnalisis').attr('disabled', true);
        //    $('#ToleranciaSinAnalisis').val("");
        //}
        //else {
        //    $('#ToleranciaSinAnalisis').removeAttr('disabled');
        //}
};

$(document).on('change', '#valorEspecialMin', function () {
    valoresEspecialesRequeridos();
});

$(document).on('change', '#valorEspecialMax', function () {
    valoresEspecialesRequeridos();
});

function valoresEspecialesRequeridos() {
    if ($('#valorEspecialMin').val() || $('#valorEspecialMax').val()) {

        $('#valorEspecialMin').prop('required', true);
        $('#valorEspecialMax').prop('required', true);
    }
}

jQuery.extend(jQuery.validator.messages, {
    required: "Este campo es requerido."
});