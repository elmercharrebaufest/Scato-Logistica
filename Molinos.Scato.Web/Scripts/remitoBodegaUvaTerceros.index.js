$(document).ready(function () {
    //Foco en primer elemento
    $("#orden-form").find(':input:not([readonly]):enabled:visible:first').focus();

    $(".patente-internacional").mask("?*******", {placeholder: ""});
    $("#NroRemito").mask("9999-99999999");
    $("#OrdenDeCompra").mask("9999999999");
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

    //inhabilitar campos hasta busqueda en SAP
    InhabilitarCampos();

    //$('#mensajeError').hide();

    $('#aplicar').click(function() {
        buscarEnSap();
    });
    
    $('#OrdenDeCompra').on('change',function () {
        $('.OrdenDeCompraMensaje').html("");

        $('.OrdenDeCompraMensaje').addClass('field-validation-valid');
        $('.OrdenDeCompraMensaje').removeClass('field-validation-error');
    });
    
    $('.buscarEnSap').keypress(function (event) {
        if (event.keyCode == 13) {
            buscarEnSap();
            return false;
        }
    });
    
    $('#VinedoId').change(function () {
        FiltrarMaterialesPorVinedo();
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

    var porcentajeUvas = 100;
    $('.form-horizontal').submit(function () {
        var vm = ko.dataFor(document.getElementById('descargaDeBinesViewModel'));
        if (vm.descargasDeBines().length > 0 && vm.descargasDeBines()[0].Clase == 1) {
            porcentajeUvas = 0;
            $.each(vm.descargasDeBines(), function (index, value) {
                porcentajeUvas = parseInt(porcentajeUvas) + parseInt(value.CantidadBines);
            });
        }
    });

    $.validator.addMethod("porcentajeDeUvas", function (value, element) {
        if (porcentajeUvas != 100) {
            return false;
        }
        return true;
    }, $("#cantidad").data().error);
});

function buscarEnSap() {
    if ($('#OrdenDeCompra').val().length > 0) {
        $('.OrdenDeCompraMensaje').html("");

        $('.OrdenDeCompraMensaje').addClass('field-validation-valid');
        $('.OrdenDeCompraMensaje').removeClass('field-validation-error');

        BlockUI();
        $.getJSON($('#links').data().urlBuscarSap, { ordenDeCompra: $('#OrdenDeCompra').val() },
            function (data) {
                if (data.mensajeError > "") {
                    MostrarAlertaError(data.mensajeError);
                } else {
                    $('#ProveedorId').val(data.proveedorId);
                    $('#Proveedor').val(data.proveedorDesc);
                    $('#Proveedor').attr('disabled', 'disabled');
                    $('#materialesCodigoSap').val(data.materialesCodigoSap);
                    
                    var options = '';
                    for (var j = 0; j < data.vinedos.length; j++) {
                        options += "<option value='" + data.vinedos[j].Value + "'>"
                                + data.vinedos[j].Text + "</option>";
                    }
                    $('#VinedoId').html(options);

                    if (data.vinedos.length == 0) {
                        $('.vinedoMensaje').html($(".vinedodiv").data().vinedomensaje);

                        $('.vinedoMensaje').addClass('field-validation-error');
                        $('.vinedoMensaje').removeClass('field-validation-valid');

                    } else {
                        $('.vinedoMensaje').html("");

                        $('.vinedoMensaje').addClass('field-validation-valid');
                        $('.vinedoMensaje').removeClass('field-validation-error');

                    }
                    

                    HabilitarCampos();
                    FiltrarMaterialesPorVinedo();
                }
            }).always(function () {
                $.unblockUI();
            });
    } else {
        $('.OrdenDeCompraMensaje').html($("#OrdenDeCompra").data().mensajerequerido);

        $('.OrdenDeCompraMensaje').addClass('field-validation-error');
        $('.OrdenDeCompraMensaje').removeClass('field-validation-valid');

    }
}

function onSelectProveedor() {
    $('#EsTransportista').val(false);
}

function onSelectTransportista() {
    $('#EsTransportista').val(true);
}

function DescargaDeBines(id, tipoId, tipo, cantidad, clase) {
    if (id == 0) {
        this.Id = id;
        this.TipoId = tipoId;
        this.Tipo = tipo;
        this.CuartelId = 0;
        this.Cuartel = "";
        this.CantidadBines = cantidad;
        this.Clase = clase;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.MaterialId = id.TipoId;
        this.Material = id.Tipo;
        this.CuartelId = id.CuartelId;
        this.Cuartel = id.Cuartel;
        this.CantidadBines = id.Cantidad;
        this.Clase = ObtenerClasePorMaterial(id.TipoId);
    }
}

function DescargaDeBinesListViewModel() {
    var self = this;
    self.descargasDeBines = ko.observableArray([]);
    self.newTipoDeBin = ko.observable();
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
        var nuevaClase = ObtenerClasePorMaterial($('#tiposDeBines option:selected').val());
        if (self.descargasDeBines().length == 0 || nuevaClase == self.descargasDeBines()[0].Clase) {
            if ($('#tiposDeBines option:selected').text() > "" && $('#cantidad').val() > "" && $.isNumeric($('#cantidad').val())) {
                self.descargasDeBines.push(new DescargaDeBines(0, $('#tiposDeBines option:selected').val(), $('#tiposDeBines option:selected').text(), $('#cantidad').val(), nuevaClase));
                //inhabilito la opcion
                self.newCantidad("");
            }
            $('#descargasFinales').val(ko.toJSON(self.descargasDeBines));
        }
    };

    self.removeDescarga = function (descarga) {
        if ($('#soloLectura').val() != "True") {
            self.descargasDeBines.remove(descarga);
            if (self.descargasDeBines.count == 0) {
                //habilito el dropdown
                $("#tiposDeBines").removeAttr('disabled');
            }
            $('#descargasFinales').val(ko.toJSON(self.descargasDeBines));
        }
    };

}

function FiltrarMaterialesPorVinedo() {
    if ($('#VinedoId option:selected').val() > 0) {
        $.getJSON($('#links').data().urlFiltrarMateriales, {workflow: $('#workflow').val(), centroId: $('#centroId').val(), vinedoId: $('#VinedoId option:selected').val(), materialesCodigoSap: $('#materialesCodigoSap').val()},
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

function InhabilitarCampos() {
    $('select').attr("readonly", "readonly");
    $('input').attr("readonly", "readonly");
    $('.agregar').attr("disabled", "disabled");
    $('.aceptar').attr("disabled", "disabled");
    $('#tiposComerciales').attr("disabled", "disabled");
    $('#VinedoId').attr("disabled", "disabled");
    $('#MaterialId').attr("disabled", "disabled");
    $('#Chofer_TipoDocumentoIdentidadId').attr("disabled", "disabled");
    $('#tiposDeBines').attr("disabled", "disabled");
    $('#TipoVehiculoBodegaId').attr("disabled", "disabled");
    $('#TipoCosecha').attr("disabled", "disabled");

    $('#Patente').removeAttr("readonly");
    $('#NroRemito').removeAttr("readonly");
    $('#OrdenDeCompra').removeAttr("readonly");
}

function HabilitarCampos() {
    $('select').removeAttr("readonly");
    $('input').removeAttr("readonly");
    $('.agregar').removeAttr("disabled");
    $('.aceptar').removeAttr("disabled");
    $('#tiposComerciales').removeAttr("disabled");
    $('#VinedoId').removeAttr("disabled");
    $('#MaterialId').removeAttr("disabled");
    $('#Chofer_TipoDocumentoIdentidadId').removeAttr("disabled");
    $('#tiposDeBines').removeAttr("disabled");
    $('#TipoVehiculoBodegaId').removeAttr("disabled");
    $('#TipoCosecha').removeAttr("disabled");


    $('#PesoNetoOrigen').attr("readonly", "readonly");
}