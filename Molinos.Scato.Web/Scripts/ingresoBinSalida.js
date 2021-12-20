
function CargaDeBinesDto(id, cantidad, tipoBinId, tipoBin, remitoId, pesoBin) {
    if (id == 0) {
        this.Id = id;
        this.CantidadBines = cantidad;
        this.TipoId = tipoBinId;
        this.Tipo = tipoBin;
        this.RemitoBodegaUvaId = remitoId;
        this.Peso = pesoBin;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.CantidadBines = id.CantidadBines;
        this.TipoId = id.TipoId;
        this.Tipo = id.Tipo;
        this.RemitoBodegaUvaId = id.RemitoBodegaUvaId;
        this.Peso = id.Peso;
    }
}

function EgresoListViewModel() {
    // Data
    var self = this;
    self.tiposBines = ko.observableArray(tiposBines);
    self.egresos = ko.observableArray([]);
    self.observacion = ko.observable();
    self.newTipoBin = ko.observable();
    self.newCantidad = ko.observable(),
    self.kgBinesTara = ko.computed(function() {
        return self.egresos().reduce(function(suma, item) {
            return suma + (item.CantidadBines * item.Peso);
        }, 0);
    });

    self.netoBines = ko.computed(function() {
        return kgBinesIngreso - self.kgBinesTara();
    });

    // Operations
    self.addBin = function() {
        if (!this.newCantidad() || this.newCantidad() <= 0 || this.newCantidad() > 999 || (this.newCantidad() % 1) != 0) {
            $.get(this.href, cargarDialogoVer($('#errorNumerico')));
        } else {

            var noExiste = self.egresos().every(function(value, index, array) {
                return value.TipoId != self.newTipoBin().Id;
            });

            if (noExiste) {
                self.egresos.push(new CargaDeBinesDto(0, this.newCantidad(), this.newTipoBin().Id, this.newTipoBin().Descripcion, remitoId, this.newTipoBin().Peso));
                this.newCantidad("");
            } else {
                $.get(this.href, cargarDialogoVer($('#errorTipoBinExistente')));
            }
        }
    };

    self.removeEgreso = function(bin) {
        self.egresos.remove(bin);
    };

}

$(document).ready(function () {

    var viewModelEgreso = new EgresoListViewModel();
    ko.applyBindings(viewModelEgreso);

    ko.extenders.formatted = function (target, callback) {
        target.formatted = ko.dependentObservable(function () {
            return callback(ko.utils.unwrapObservable(target));
        });
        return target;
    };

});
