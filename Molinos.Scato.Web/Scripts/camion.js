
jQuery(document).ready(function ($) {

    /*Agrego clase de validación para Bootstrap*/
    $('span.field-validation-valid, span.field-validation-error').each(function () {
        $(this).addClass('help-block');
    });

    //  create the model
    var model = new CamionViewModel();

    //  bind model to the html
    ko.applyBindings(model);

    if ($('#esIngreso').val() == "True" || $("#habilitarSiemprePeso").val() == "True") {
        $.validator.addMethod("pesoBrutoRequerido", function (value, element) {
            if (vehiculoDemorado && vehiculoDemorado == true) return true;
            return (value.length > 0 && value > 0);
        }, $('#PesoBrutoOrigen').data().errorRequerido);
        $.validator.addMethod("pesoTaraRequerido", function (value, element) {
            if (vehiculoDemorado && vehiculoDemorado == true) return true;
            return (value.length > 0 && value > 0);
        }, $('#PesoTaraOrigen').data().errorRequerido);


        $.validator.addMethod("pesoBrutoNumerico", function (value, element) {
            return $.isNumeric(value);
        }, $('#PesoBrutoOrigen').data().errorNumerico);
        $.validator.addMethod("pesoTaraNumerico", function (value, element) {
            return $.isNumeric(value);
        }, $('#PesoTaraOrigen').data().errorNumerico);

        $.validator.addMethod("pesoNetoPositivo", function (value, element) {
            return $('#PesoNetoOrigen').val() > 0;
        }, $('#PesoNetoOrigen').data().errorPositivo);

        $(".pesoNetoMaximoEjecutar").change(
            function () {
                $('.pesoNetoMaximo').valid();
            }
        );

        $.validator.addMethod("pesoNetoMaximo", function (value, element) {
            if (vehiculoDemorado && vehiculoDemorado == true) return true;
            var pesoNeto = $('#PesoNetoOrigen').val();
            if (pesoNeto > 0 && $('#PesoBrutoOrigen').val() > 0 && $('#PesoTaraOrigen').val() > 0) {
                var netoMax = $('#tipoVehiculoDropdown :selected').data('netomaximo');
                return netoMax == null || pesoNeto <= netoMax;
            } else {
                return true;
            }
        }, $('#PesoNetoOrigen').data().errorPesonetomaximo);

        $(".pesoBrutoMaximoEjecutar").change(
            function () {
                $('.pesoBrutoMaximo').valid();
            }
        );

        $.validator.addMethod("pesoNetoMinimo", function (value, element) {
            if (vehiculoDemorado && vehiculoDemorado == true) return true;
            var pesoNeto = $('#PesoNetoOrigen').val();
            if (pesoNeto > 0 && $('#PesoBrutoOrigen').val() > 0 && $('#PesoTaraOrigen').val() > 0) {
                var netoMin = $('#tipoVehiculoDropdown :selected').data('netominimo');
                return netoMin == null || pesoNeto >= netoMin;
            } else {
                return true;
            }
        }, $('#PesoNetoOrigen').data().errorPesonetominimo);

        $(".pesoBrutoMinimoEjecutar").change(
            function () {
                $('.pesoBrutoMinimo').valid();
            }
        );

        $.validator.addMethod("pesoBrutoMaximo", function (value, element) {
            if (vehiculoDemorado && vehiculoDemorado == true) return true;
            var pesoBruto = $('#PesoBrutoOrigen').val();
            if ($('#PesoBrutoOrigen').val() > 0) {
                var brutoMax = $('#tipoVehiculoDropdown :selected').data('brutomaximo');
                return brutoMax == null || pesoBruto <= brutoMax;
            } else {
                return true;
            }
        }, $('#PesoBrutoOrigen').data().errorPesobrutomaximo);

        //validacion para vagones
        $.validator.addMethod("pesoBrutoPositivo", function (value, element) {
            return $('#' + (parseInt(element.name) + 2)).val() >= 0;
        }, $('#PesoNetoOrigen').data().errorPositivo);
        $.validator.addMethod("pesoTaraPositivo", function (value, element) {
            return $('#' + (parseInt(element.name) + 1)).val() >= 0;
        }, $('#PesoNetoOrigen').data().errorPositivo);

        $.validator.addMethod("pesoNetoMaximoBrutoEjecutarVagon", function (value, element) {
            $('#' + (parseInt(element.name) + 2)).valid();
            return true;
        });
        $.validator.addMethod("pesoNetoMaximoTaraEjecutarVagon", function (value, element) {
            $('#' + (parseInt(element.name) + 1)).valid();
            return true;
        });

        $.validator.addMethod("pesoNetoMaximoVagon", function (value, element) {
            var pesoNeto = $('#' + (parseInt(element.name))).val();
            var pesoTara = $('#' + (parseInt(element.name) - 1)).val();
            var pesoBruto = $('#' + (parseInt(element
                .name) - 2)).val();

            if (pesoNeto > 0 && pesoTara > 0 && pesoBruto > 0) {
                var netoMax = $('#tipoVehiculoDropdown :selected').data('netomaximo');
                return netoMax == null || pesoNeto <= netoMax;
            } else {
                return true;
            }
        }, $('#PesoNetoOrigen').data().errorPesonetomaximo);

        $.validator.addMethod("pesoNetoMinimoVagon", function (value, element) {
            var pesoNeto = $('#' + (parseInt(element.name))).val();
            var pesoTara = $('#' + (parseInt(element.name) - 1)).val();
            var pesoBruto = $('#' + (parseInt(element
                .name) - 2)).val();

            if (pesoNeto > 0 && pesoTara > 0 && pesoBruto > 0) {
                var netoMin = $('#tipoVehiculoDropdown :selected').data('netominimo');
                return netoMin == null || pesoNeto >= netoMin;
            } else {
                return true;
            }
        }, $('#PesoNetoOrigen').data().errorPesonetominimo);
        

        $.validator.addMethod("TarifaReferenciaRequerido", function (value, element) {
            return value.length > 0;
        }, $('#TarifaReferencia').data().errorRequerido);
    } else {
        $("#DvPeso :input").attr("disabled", true);
        $('#TarifaReferencia').attr("readonly", "readonly");
        $('#TarifaReferencia').attr('tabindex', -1);
    }

    //$.validator.addMethod("TarifaToneladaRequerido", function (value, element) {
    //    return value.length > 0;
    //}, $('#TarifaTonelada').data().errorRequerido);
    $.validator.addMethod("KmRecorrerRequerido", function (value, element) {
        return value.length > 0;
    }, $('#KmRecorrer').data().errorRequerido);
    $.validator.addMethod("KmRecorrerNumerico", function (value, element) {
        return $.isNumeric(Globalize.parseFloat(value)) ;
    }, $('#KmRecorrer').data().errorNumerico);
    $.validator.addMethod("TarifaReferenciaNumerico", function (value, element) {
        return $.isNumeric(Globalize.parseFloat(value)) || value == "";
    }, $('#TarifaReferencia').data().errorNumerico);
    $.validator.addMethod("TarifaToneladaNumerico", function (value, element) {
        return $.isNumeric(Globalize.parseFloat(value)) || value == "";
    }, $('#TarifaTonelada').data().errorNumerico);
    $.validator.addMethod("PatenteRequerido", function (value, element) {
        return value.length > 0;
    }, $('#Patente').data().errorRequerido);
    $.validator.addMethod("PatenteUnica", function (value, element) {
        var flag = true;
        $(".PatenteUnica").each(function (index) {
            if (value != "______" && this.value != "______" && this.value.replace(new RegExp('_', 'g'), '') == value.replace(new RegExp('_', 'g'), '') && $(this).attr('id') != $(element).attr('id')) flag = false;
        });
        return flag;
    }, $('#Patente').data().errorDuplicada);
    $(".patente-internacional").mask("?*******", { placeholder: "" });

    $('.campoNumerico').keydown(function (event) {
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 || event.keyCode == 13) {
            var a = "vacio";
        } else {
            if (event.keyCode == 190 || event.keyCode == 188 || event.keyCode == 110) {
                $(this).val($(this).val() + Globalize.culture().numberFormat["."]);
                event.preventDefault();
            }
            else if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });

    $('.pesoBrutoNumerico').keydown(function (event) {
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 || event.keyCode == 13) {
            var a = "vacio";
        } else {
            if (event.keyCode == 190 || event.keyCode == 188 || event.keyCode == 110) {
                //$(this).val($(this).val() + Globalize.culture().numberFormat["."]);
                event.preventDefault();
            }
            else if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });

    $('.pesoTaraNumerico').keydown(function (event) {
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46 || event.keyCode == 13) {
            var a = "vacio";
        } else {
            if (event.keyCode == 190 || event.keyCode == 188 || event.keyCode == 110) {
                //$(this).val($(this).val() + Globalize.culture().numberFormat["."]);
                event.preventDefault();
            }
            else if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
            }
        }
    });

    //Corrige la funcionalidad de la validación
    var arr = ["PesoBrutoOrigen", "PesoTaraOrigen", "Patente", "PatenteAcoplado", "PatenteAcoplado2", "KmRecorrer", "TarifaReferencia", "TarifaTonelada"];

    jQuery.each(arr, function (i, val) {
        $("#" + val).focusout(function () {
            if ($("#" + val).parent().children().children().length == 0) {
                $("#" + val).parent().parent().removeClass("error");
            }
            if ($("#" + val).parent().children().children().length == 1) {
                $("#" + val).parent().parent().addClass("error");
            }
        });
    });
    $('#Patente').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado').change(ValidarPatenteCnrt);
    $('#PatenteAcoplado2').change(ValidarPatenteCnrt);
});
flag = false;
function ValidarPatenteCnrt() {
    if (flag == false) {
        var patente = $("#Patente").val();
        var acoplado = $("#PatenteAcoplado").val();
        var acoplado2 = $("#PatenteAcoplado2").val();
        if (patente != '' || acoplado != '' || acoplado2 != '') {
            flag = true;
            ActualizarTipoVehiculo(patente, acoplado, acoplado2, function () { BlockUI(" consulta de tipo de vehiculo por patente"); }, function () { $.unblockUI(); });
        }
    }
}

/*----------------------------------------------------------------------*/
/* Item Model
/*----------------------------------------------------------------------*/
function Camion(id, patente, patenteacoplado, patenteacoplado2, bruto, tara, neto) {
    var self = this;
    if (bruto == 0) {
        bruto = null;
    }
    if (tara == 0) {
        tara = null;
    }
    if (neto == 0) {
        neto = null;
    }
    self.Id = id;
    self.Patente = ko.observable(patente);
    self.PatenteAcoplado = ko.observable(patenteacoplado);
    self.PatenteAcoplado2 = ko.observable(patenteacoplado2);
    self.PesoBrutoOrigen = ko.observable(bruto);
    self.PesoTaraOrigen = ko.observable(tara);
    self.PesoNetoOrigen = ko.observable(neto);
    self.Primero = true;
};


/*----------------------------------------------------------------------*/
/* View Model
/*----------------------------------------------------------------------*/
function CamionViewModel() {
    var self = this;

    //  data
    self.camion = ko.observableArray([]);
    var camionJson = $("#VehiculoJson").val();
    if (camionJson != "[]" && camionJson != "" && camionJson != null) {
        var camionAux = jQuery.parseJSON(camionJson);
        self.camion.push(new Camion(camionAux[0].Id, camionAux[0].Patente, camionAux[0].PatenteAcoplado, camionAux[0].PatenteAcoplado2, camionAux[0].PesoBrutoOrigen, camionAux[0].PesoTaraOrigen, camionAux[0].PesoNetoOrigen));
    } else {
        var camion = new Camion(0, null, null, null, null, null);
        self.camion.push(camion);
    }


    //  edit helpers
    self.availableColours = [];
    self.editingItem = ko.observable();
    self.isItemEditing = function (itemToTest) {
        return itemToTest == self.editingItem();
    };

    self.actualizarPesoNeto = function (e) {
        var resta = e.PesoBrutoOrigen() - e.PesoTaraOrigen();
        if (isNaN(resta)) {
            e.PesoNetoOrigen(null);
        } else {
            e.PesoNetoOrigen(resta);
        }
        return true;
    };
}

function ActualizarTipoVehiculo(patente, acoplado, acoplado2, before, callback) {
    if (($('#Cpe').is(':checked') ? true : CorrespondeNotificarPorNetoMaximo()) && (patente != "" || acoplado != "")) {
        if (before != null) before();
        $.getJSON($("#links").data().urlObtenerTipoVehiculo, { patente: patente, acoplado: acoplado, acoplado2: acoplado2 }, function (data) {
            if (data.CodigoDeError == 0) {
                flag = false;
                $('#tipoVehiculoDropdown').val(data.vehiculo.TipoVehiculo);
            } else {
                ValidarTipoVehiculoPorCnrt(patente, acoplado, acoplado2, before, callback);
            }
        }).complete(function () {
            if (callback != null) callback();
        });
    } else {
        if (callback != null) callback();
    }
}

function CorrespondeNotificarPorNetoMaximo() {
    var pesoBruto = $('#PesoBrutoOrigen').val();
    var brutoMax = $('#tipoVehiculoDropdown option[value=0]').data('brutomaximo');

    var pesoNeto = $('#PesoNetoOrigen').val();
    var netoMax = $('#tipoVehiculoDropdown option[value=0]').data('netomaximo');
    return !(pesoNeto > 0 && $('#PesoBrutoOrigen').val() > 0 && $('#PesoTaraOrigen').val() > 0 && (netoMax == null || pesoNeto <= netoMax) && (brutoMax == null || pesoBruto <= brutoMax));
}

function CorrespondeNotificarPorNetoMinimo() {
    var pesoBruto = $('#PesoBrutoOrigen').val();
    var brutoMax = $('#tipoVehiculoDropdown option[value=0]').data('brutomaximo');

    var pesoNeto = $('#PesoNetoOrigen').val();
    var netoMin = $('#tipoVehiculoDropdown option[value=0]').data('netominimo');
    return !(pesoNeto > 0 && $('#PesoBrutoOrigen').val() > 0 && $('#PesoTaraOrigen').val() > 0 && (netoMin == null || pesoNeto >= netoMin) && (brutoMax == null || pesoBruto >= brutoMax));
}

function ValidarTipoVehiculoPorCnrt(patente, acoplado, acoplado2, before, callback) {
    $.getJSON($("#links").data().urlObtenerTipovehiculoPorPatente, { patente: patente, acoplado: acoplado, acoplado2: acoplado2, workflow: $('#workflow').val() }, function (data) {
        flag = false;

        if (data.CodigoDeError == 0) {
            if (data.Categoria != null) {
                if ($('#tipoVehiculoDropdown option[value=' + data.Categoria + ']').length == 0) {
                    if (($('#Cpe').is(':checked') ? true : CorrespondeNotificarPorNetoMaximo())) MostrarAlertaError("La categoría del vehículo " + data.CategoriaDesc + " no esta configurada para el centro actual");
                } else {
                    $('#tipoVehiculoDropdown').val(data.Categoria);
                    ValidarObjeto($("form"), $("#PesoBrutoOrigen"));
                    ValidarObjeto($("form"), $("#PesoNetoOrigen"));
                }
            } else {
                if (($('#Cpe').is(':checked') ? true : CorrespondeNotificarPorNetoMaximo())) MostrarAlertaError("El servicio CNRT no devolvió información sobre la categoría del vehículo, debe ingresarla manualmente.");
            }
        } else if (data.CodigoDeError == 1) {
            $('.btn').removeAttr('disabled');
        } else {
            MostrarAlertaAdvertencia(data.Error);
        }
    }).complete(function () {
        if (callback != null) callback();
    });
}