function Almacen(id) {
    this.Id = id.Id;
    this.Descripcion = id.Descripcion;
    this.CodigoSAP = id.CodigoSAP;
    this.EsNuevo = id.EsNuevo;
    this.FueEliminado = id.FueEliminado;
}

function CaladoPorCaracteristicaListViewModel() {
    var self = this;
    self.newAlmacen = ko.observable();
    self.almacenesSinAsignar = ko.observableArray([]);
    self.almacenesEliminados = ko.observableArray([]);
    self.almacenes = ko.observableArray([]);
    self.selectedItem = ko.observable();
    self.defaultAlmacen = ko.observable();

    self.defaultAlmacen($("#dialogo-almat-body").data().defaultAlmacen);
    var mappedalmacenes = $.map($("#dialogo-almat-body").data().almacenes, function (item) { return new Almacen(item); });
    self.almacenes(mappedalmacenes);
    
    var mappedalmacenesTodos = $.map($("#dialogo-almat-body").data().almacenesSinAsignar, function (item) { return new Almacen(item); });
    self.almacenesSinAsignar(mappedalmacenesTodos);

   
    self.botonAgregarAlmacen = function () {
        if (self.almacenes.indexOf(self.selectedItem()) == -1 && jQuery.type(self.selectedItem()) != "undefined") {

            if (self.selectedItem().FueEliminado == true) {
                self.almacenesEliminados.remove(self.selectedItem());
                self.selectedItem().FueEliminado = false;
            }
           
            self.almacenes.push(self.selectedItem());
            self.almacenesSinAsignar.remove(self.selectedItem());
        }
    };
    self.removerAlmacen = function (almacen) {
        if (almacen.Id == $('#almacenPredIdVal').val()) {
            MostrarAlertaError($("#dialogo-almat-body").data().errorPredeterminado);
            return;
        }
        if (almacen.EsNuevo == false) {
            almacen.FueEliminado = true;
            self.almacenesEliminados.push(almacen);
        }
        self.almacenesSinAsignar.push(almacen);
        self.almacenes.remove(almacen);
    };

}



$(document).ready(function () {
    ko.applyBindings(new CaladoPorCaracteristicaListViewModel());
    $('#dialogo-almat').on('show', function () {
        $('#dialogo-editar-guardar').attr("disabled", true);
        $('#dialogo-editar-cancelar').attr("disabled", true);
    });
    $('#dialogo-almat').on('hidden', function () {
        $('#dialogo-editar-guardar').attr("disabled", false);
        $('#dialogo-editar-cancelar').attr("disabled", false);
    });
    $('#dialogo-editar').on('hidden', function () {
        $('#dialogo-almat').modal('hide');
    });

    $("#mostrarAlmat").click(function () {
        $('#dialogo-almat').modal({
            backdrop: 'static', keyboard: false
        }).css({
            width: function () {
                return $('#dialogo-editar').outerWidth()/2;
            }, 'margin-left': function () {
                return -($(this).width() / 2);
            },
            'top': '50%',
            'margin-top': function () {
                return -($(this).height() / 1.5);
            }
        });
    });
    
    $(".dialogo-almat-cancelar").click(function () {
        $('#dialogo-almat').modal('hide');
    });

    if ($('#almacenPredIdVal').val() > 0) {
        var valor = $('#almacenPredIdVal').val();
        $('#AlmacenPredId').val(valor);
    } else {
        $('#almacenPredIdVal').val(0);
    }
    
    $('#AlmacenPredId').change(function () {
        $('#almacenPredIdVal').val($('#AlmacenPredId').val());
    });
});