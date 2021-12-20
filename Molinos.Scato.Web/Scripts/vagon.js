jQuery(document).ready(function ($) {
    
    /*Agrego clase de validación para Bootstrap*/
    $('span.field-validation-valid, span.field-validation-error').each(function () {
        $(this).addClass('help-block');
    });
    
    //  create the model
    var model = new VagonViewModel();

    //  bind model to the html
    ko.applyBindings(model);

    if ($('#esIngreso').val() == "False") {
        $('#TarifaReferencia').attr("readonly", "readonly");
        $('#TarifaReferencia').attr('tabindex', -1); 
    }
    $.validator.addMethod("KmRecorrerRequerido", function (value, element) {
        return value.length > 0;
    }, $('#KmRecorrer').data().errorRequerido);
    $.validator.addMethod("KmRecorrerNumerico", function (value, element) {
        return $.isNumeric(Globalize.parseFloat(value));
    }, $('#KmRecorrer').data().errorNumerico);

    //Corrige la funcionalidad de la validación
    var arr = ["KmRecorrer"];

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

    if ($('#esIngreso').val() == "True" && $('#cartaPorteId').val() == 0) {
        $('.image-ctg').show();
    } else {
        $('.image-ctg').hide();
    }
});

/*----------------------------------------------------------------------*/
/* Item Model
/*----------------------------------------------------------------------*/
var i = 0;
function Vagon(id, patente, bruto, tara, neto, primero, nroCtg, sucursal, numOrden) {
    var self = this;
    self.Id = id;
    self.Patente = ko.observable(patente).extend({ required: true });
    self.PesoBrutoOrigen = ko.observable(bruto);
    self.PesoTaraOrigen = ko.observable(tara);
    self.PesoNetoOrigen = ko.observable(neto);
    self.NumCTG = ko.observable(nroCtg);
    self.Sucural = ko.observable(sucursal);
    self.NumOrden = ko.observable(numOrden);
    self.Primero = ko.observable(primero);
    self.Indice1 = ko.observable(i++);
    self.Indice2 = ko.observable(i++);
    self.Indice3 = ko.observable(i++);
    self.Indice4 = ko.observable(i++);
    self.Indice5 = ko.observable(i++);
    self.Indice6 = ko.observable(i++);
    self.Indice7 = ko.observable(i++);
};

/*----------------------------------------------------------------------*/
/* View Model
/*----------------------------------------------------------------------*/
function VagonViewModel() {
    var self = this;

    //  data
    self.vagones = ko.observableArray([]);
    var vagonesJson = $("#VehiculoJson").val();
    if (vagonesJson != "[]" && vagonesJson != "" && vagonesJson != null) {
        var vagonesAux = jQuery.parseJSON(vagonesJson);
        var primero = true;
        for (var i = 0; i < vagonesAux.length; i++) {
            var vagonObj = new Vagon(vagonesAux[i].Id, vagonesAux[i].Patente, vagonesAux[i].PesoBrutoOrigen, vagonesAux[i].PesoTaraOrigen, vagonesAux[i].PesoNetoOrigen, primero, vagonesAux[i].NumCTG, vagonesAux[i].Sucural, vagonesAux[i].NumOrden);
            self.vagones.push(vagonObj);
            if (primero) {
                ObtenerImagenCPEFerroviaria(vagonObj);
            }
            primero = false;
        }
        validateFerroviarioCPE();
        $('.PatenteUnica').valid()
    }


    //  edit helpers
    self.availableColours = [];
    self.editingItem = ko.observable();
    self.isItemEditing = function (itemToTest) {
        return itemToTest == self.editingItem();
    };

    //  edit behaviours
    self.addVagon = function () {
        var esPrimero = false;
        if (self.vagones().length == 0) esPrimero = true;
        var vagon = new Vagon(0, null, null, null, null, esPrimero, null, null, null);
        self.vagones.push(vagon);
        
        $(".patente-internacional").mask("?9999999");
        if ($('#esIngreso').val() == "False") {
            $('.pesoBrutoRequerido').attr('disabled', 'disabled');
            $('.pesoTaraRequerido').attr('disabled', 'disabled');
            
        }
        validateFerroviarioCPE();
    };

    self.removeVagon = function (vagon) {
        if (self.editingItem() == null) {
            self.vagones.remove(vagon);
        }
    };

    self.actualizarPesoNeto = function (e) {
        var resta = e.PesoBrutoOrigen() - e.PesoTaraOrigen();
        if (isNaN(resta)) {
            e.PesoNetoOrigen(null);
        } else {
            e.PesoNetoOrigen(resta);
        }
    };

    function validateFerroviarioCPE() {
        if ($('#Cpe').is(':checked')) {
            $('.row-tren-ferroviario-hidden').removeClass('row-tren-ferroviario-hidden').addClass('row-tren-ferroviario-visibility');
        } else {
            $('.row-tren-ferroviario-visibility').removeClass('row-tren-ferroviario-visibility').addClass('row-tren-ferroviario-hidden');
        }
    }

    self.changeImagenCPE = function (vagon) {
        ObtenerImagenCPEFerroviaria(vagon);
    };

    function ObtenerImagenCPEFerroviaria(vagon) {
        if ($('#esIngreso').val() == "True" && $('#cartaPorteId').val() == 0) {
            var index = self.vagones.indexOf(vagon);
            if (index != -1) {
                var nroCTG = self.vagones()[index].NumCTG() == null ? '' : self.vagones()[index].NumCTG() == undefined ? '' : self.vagones()[index].NumCTG();
                obtenerFotoCartaPorteElectronica(nroCTG);
            }
        }
    }

    function obtenerFotoCartaPorteElectronica(nroCTG) {
        try {
            if (nroCTG != '') {
                //Clear
                $('#foto1').attr("src", '');
                $('#fotoDiv').css('display', 'inline');
                $('.tomarFoto1').css('display', 'inline');
                $('.tomarFoto2').css('display', 'inline');
                $('#FotoMesaDigitalizacion1').val('');
                $('.ocultar').hide();
                $('#tabFotos a[href="#fotoDiv1"]').tab('show');
                $('#foto1').elevateZoom({
                    zoomType: "inner",
                    cursor: "crosshair"
                });

                BlockUI("Obteniendo Imagen...");
                $.getJSON($("#links").data().urlObtenerImagenCpe, { nroCtg: nroCTG }, function (data) {
                    if (data.CodigoDeError == 1) {
                        MostrarAlertaInfo(data.Error);
                    } else if (data.CodigoDeError == 0) {
                        $('#foto1').attr("src", 'data:image/jpeg;base64,' + data.PdfImageBase64);
                        $('#fotoDiv').css('display', 'inline');
                        $('.tomarFoto1').css('display', 'inline');
                        $('.tomarFoto2').css('display', 'inline');
                        $('#FotoMesaDigitalizacion1').val(data.PdfImageBase64);
                        $('.ocultar').hide();
                        $('#tabFotos a[href="#fotoDiv1"]').tab('show');
                        $('#foto1').elevateZoom({
                            zoomType: "inner",
                            cursor: "crosshair"
                        });
                    } else {
                        MostrarAlertaError(data.Error);
                    }
                }).complete(function () {
                    $.unblockUI();
                });
            }
        } catch (e) {
            $.unblockUI();
        }
    }

}