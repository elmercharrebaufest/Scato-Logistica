$(document).ready(function () {
    //Foco en primer elemento
    $("#orden-form").find(':input:not([readonly]):enabled:visible:first').focus();

    $(".patente-internacional").mask("?*******", {placeholder: ""});
    $("#NroRemito").mask("9999-99999999");
    $("#ModeloCamion").mask("9999");

    DefinirAutocompletarChofer();

    var listarProveedores = $('#links').data().urlBuscarProveedores;
    var obtenerProveedor = $('#links').data().urlBuscarProveedor;
    var obtenerProveedorSap = $('#links').data().urlObtenerProveedoresSap;

    DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, false, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    $('#TipoComercialId').change(function () {
        DefinirAutocompletarTransportista('#Transportista', '#TransportistaId', '#autocompleteTran', listarProveedores, obtenerProveedor, obtenerProveedorSap, $('#links').data().urlBuscarTransportistas, $('#links').data().urlBuscarTransportistaUnico, true, '#TipoComercialId', $('#tiposComerciales').data().altaRapida, onSelectProveedor, onSelectTransportista, true, false, false);
    });
    ko.applyBindings(new DescargaDeBinesListViewModel(), document.getElementById('permisosViewModel'));

    FiltrarMaterialesPorVinedo();
    if ($('#soloLectura').val() != "True") {
        FiltrarCuartelesPorVinedo();
    }

    $('#VinedoId').change(function () {
        FiltrarMaterialesPorVinedo();
        FiltrarCuartelesPorVinedo();
    });
    
    if ($('#tiposDeBines option:selected').val() > 0) {
        var clase = ObtenerClasePorMaterial($('#tiposDeBines option:selected').val());
        if (clase == 0) {
            $('.labelCantidad').text($('#textoCantidad').val());
        } else {
            $('.labelCantidad').text($('#textoPorcentaje').val());
        }
    }

    $('#tiposDeBines').change(function () {
        if ($('#soloLectura').val() != "True") {
            var clase = ObtenerClasePorMaterial($('#tiposDeBines option:selected').val());
            if (clase == 0) {
                $('.labelCantidad').text($('#textoCantidad').val());
            } else {
                $('.labelCantidad').text($('#textoPorcentaje').val());
            }
        }
    });

    $('#PesoBrutoOrigen').change(function () {
        CalcularPesoNetoOrigen();
    });
    
    $('#PesoTaraOrigen').change(function () {
        CalcularPesoNetoOrigen();
    });

    
    $('.form-horizontal').submit(function () {

    });
    
    $.validator.addMethod("porcentajeDeUvas", function (value, element) {
        var porcentajeUvas = 100;
        var vm = ko.dataFor(document.getElementById('descargaDeBinesViewModel'));
        if (vm.descargasDeBines().length > 0 && vm.descargasDeBines()[0].Clase == 1) {
            porcentajeUvas = 0;
            $.each(vm.descargasDeBines(), function(index, val) {
                porcentajeUvas = parseInt(porcentajeUvas) + parseInt(val.CantidadBines);
            });
        }
        return porcentajeUvas == 100;
    }, $("#cantidad").data().error);
});

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function DescargaDeBines(id, tipoId, tipo, cuartelId, cuartel, cantidad, clase) {
    if (id == 0) {
        this.Id = id;
        this.TipoId = tipoId;
        this.Tipo = tipo;
        this.CuartelId = cuartelId;
        this.Cuartel = cuartel;
        this.CantidadBines = cantidad;
        this.Clase = clase;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.TipoId = id.TipoId;
        this.Tipo = id.Tipo;
        this.CuartelId = id.CuartelId;
        this.Cuartel = id.Cuartel;
        this.CantidadBines = id.CantidadBines;
        this.Clase = ObtenerClasePorMaterial(id.TipoId);
    }
}

function DescargaDeBinesListViewModel() {
    var self = this;
    self.descargasDeBines = ko.observableArray([]);
    self.newTipoDeBin = ko.observable();
    self.newCodigoCuartelId = ko.observable();
    self.newCantidad = ko.observable();

    $.get($('#links').data().urlObtenerDescargas, { id: $('#Id').val() },
        function (allData) {
            var mappeddescargas = $.map(allData, function (item) {
                return new DescargaDeBines(item);
            });
            self.descargasDeBines(mappeddescargas);
            $('#descargasFinales').val(ko.toJSON(self.descargasDeBines));
        }
    );

    // Operations
    self.addDescarga = function () {
        if ($('#tiposDeBines option:selected').val() > '') {
            var nuevaClase = ObtenerClasePorMaterial($('#tiposDeBines option:selected').val());
            if (self.descargasDeBines().length == 0 || nuevaClase == self.descargasDeBines()[0].Clase) {
                if ($('#tiposDeBines option:selected').text() > "" && $('#codigosCuartel option:selected').text() > "" && $('#cantidad').val() > "" && $.isNumeric($('#cantidad').val())) {
                    self.descargasDeBines.push(new DescargaDeBines(0, $('#tiposDeBines option:selected').val(), $('#tiposDeBines option:selected').text(), $('#codigosCuartel option:selected').val(), $('#codigosCuartel option:selected').text(), $('#cantidad').val(), nuevaClase));
                    //inhabilito el dropdown
                    //$("#tiposDeBines").attr('disabled', 'disabled');
                    //inhabilito la opcion
                    $("#codigosCuartel option:selected").attr('disabled', 'disabled');
                    setearDropDownSelected();
                    self.newCantidad("");
                }
                $('#descargasFinales').val(ko.toJSON(self.descargasDeBines));
            }
        }
    };

    self.removeDescarga = function (descarga) {
        if ($('#soloLectura').val() != "True") {
            //habilito la opcion
            $("#codigosCuartel option[value=" + descarga.CuartelId + "]").removeAttr('disabled');
            self.descargasDeBines.remove(descarga);
            if (self.descargasDeBines.count == 0) {
                //habilito el dropdown
                $("#tiposDeBines").removeAttr('disabled');
            }
            $('#descargasFinales').val(ko.toJSON(self.descargasDeBines));
        }
    };

}

function setearDropDownSelected() {
    var seleccionado = false;
    $.each($("#codigosCuartel option"), function (index, value) {
        if (value.disabled) {
            value.selected = false;
        } else {
            if (!seleccionado) {
                value.selected = true;
                seleccionado = true;
            }
        }
    });
}

function FiltrarMaterialesPorVinedo() {
    if ($('#VinedoId option:selected').val() > 0) {
        $.getJSON($('#links').data().urlFiltrarMateriales, { workflow: $('#workflow').val(), centroId: $('#centroId').val(), vinedoId: $('#VinedoId option:selected').val() },
            function (allData) {

                var options = '';
                for (var j = 0; j < allData.length; j++) {
                    options += "<option value='" + allData[j].Value + "'>"
                            + allData[j].Text + "</option>";
                }
                $('#MaterialIdYPosicion').html(options);
                
                if (allData.length == 0) {
                    $('.materialMensaje').html($(".materialdiv").data().materialmensaje);

                    $('.materialMensaje').addClass('field-validation-error');
                    $('.materialMensaje').removeClass('field-validation-valid');

                } else {
                    $('.materialMensaje').html("");

                    $('.materialMensaje').addClass('field-validation-valid');
                    $('.materialMensaje').removeClass('field-validation-error');

                }
            }
        );
    }
}

function FiltrarCuartelesPorVinedo() {
    if ($('#VinedoId option:selected').val() > 0) {
        $.getJSON($('#links').data().urlFiltrarCuarteles, { vinedoId: $('#VinedoId option:selected').val() },
            function (allData) {

                var options = '';
                for (var j = 0; j < allData.length; j++) {
                    options += "<option value='" + allData[j].Value + "'>"
                            + allData[j].Text + "</option>";
                }
                $('#codigosCuartel').html(options);
                //inhabilito los que ya fueron elegidos
                var vm = ko.dataFor(document.getElementById('descargaDeBinesViewModel'));
                $.each(vm.descargasDeBines(), function (index, value) {
                    $("#codigosCuartel option[value=" + value.Id + "]").attr('disabled', 'disabled');
                });

                setearDropDownSelected();
            }
        );
    }
}

function ObtenerClasePorMaterial(materialId) {
    var clase;
    $.ajax({
        url: $('#links').data().urlObtenerClase,
        dataType: 'json',
        async: false,
        data: { materialId: materialId },
        success: function (data) {
            clase = data.resultado;
        },
    });
    return clase;
}

function CalcularPesoNetoOrigen() {
    var pesoBrutoOrigen = 0;
    if ($.isNumeric(parseInt($('#PesoBrutoOrigen').val()))) {
        pesoBrutoOrigen = $('#PesoBrutoOrigen').val();
    }
    var pesoTaraOrigen = 0;
    if ($.isNumeric(parseInt($('#PesoTaraOrigen').val()))) {
        pesoTaraOrigen = $('#PesoTaraOrigen').val();
    }
    $('#PesoNetoOrigen').val(pesoBrutoOrigen - pesoTaraOrigen);
}