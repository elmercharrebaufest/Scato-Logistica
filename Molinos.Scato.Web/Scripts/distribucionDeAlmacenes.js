var i = 0;
function DistribucionDeAlmacen(id, almacen, almacenId, litros) {
    this.IdTemporal = i++;
    if (id == 0) {
        this.Id = id;
        this.Almacen = almacen;
        this.AlmacenId = almacenId;
        this.Litros = litros;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Almacen = id.Almacen;
        this.AlmacenId = id.AlmacenId;
        this.Litros = id.Litros;
    }
}
function DistribucionDeAlmacenModel() {
    var self = this;
    self.distribucionesDeAlmacenes = ko.observableArray([]);
    self.litrosRestantes = ko.observable(0);
    if ($("#distribucionesDeAlmacenesJsonPostBack").val() != "") {
        //Si es un postback del controller: (modelo invalido)
        var mapped = $.map(JSON.parse($("#distribucionesDeAlmacenesJsonPostBack").val()), function (item) { return new DistribucionDeAlmacen(item); });
        self.distribucionesDeAlmacenes(mapped);
    }
    if ($("#PesoNetoBodegaEnLitros").val() != "") {
        var total = 0;
        ko.utils.arrayForEach(self.distribucionesDeAlmacenes(), function (f) {
            total += parseInt(f.Litros);
        });
        self.litrosRestantes(parseInt($("#PesoNetoBodegaEnLitros").val()) - total);
    }

    self.botonAgregar = function () {
        ValidarObjeto($("form"), $("#almacenDropDown"));
        ValidarObjeto($("form"), $("#litros"));
        var controlGroup1 = $("#almacenDropDown").closest("div.control-group");
        var controlGroup2 = $("#litros").closest("div.control-group");
        
        if ($("#litros").val().length == 0) {
            controlGroup2.addClass('error');
            var obj = controlGroup2.find('span.field-validation-valid');
            obj.addClass('field-validation-error');
            obj.removeClass('field-validation-valid');
            obj.html($('#litros').data().errorRequerido);
        }

        if (controlGroup1.find('span.field-validation-error').length == 0 && controlGroup2.find('span.field-validation-error').length == 0) {
            self.distribucionesDeAlmacenes.push(new DistribucionDeAlmacen(0, $('#almacenDropDown :selected').text(), $('#almacenDropDown :selected').val(), $("#litros").val()));
            self.litrosRestantes(parseInt(self.litrosRestantes()) - parseInt($("#litros").val()));
            $("#litros").val("");
        }
    };
    
    self.removeDistribucion = function (d) {
        self.distribucionesDeAlmacenes.remove(function (e) {
            return e.IdTemporal == d.IdTemporal;
        });
        
        var total = 0;
        ko.utils.arrayForEach(self.distribucionesDeAlmacenes(), function (f) {
            total += parseInt(f.Litros);
        });
        self.litrosRestantes(parseInt($("#PesoNetoBodegaEnLitros").val()) - total);
    };
    
    $.validator.addMethod("almacenRequerido", function (value, element) {
        return value.length > 0 && parseInt(value) > 0;
    }, $('#almacenDropDown').data().errorRequerido);
    
    $.validator.addMethod("litrosValidos2", function (value, element) {
        return value.length == 0 || parseInt(value) <= parseInt(self.litrosRestantes());
    }, $('#litros').data().errorInvalido);
    $.validator.addMethod("litrosValidos", function (value, element) {
        return value.length == 0 || parseInt(value) > 0;
    }, $('#litros').data().errorRequerido);
}
$(document).ready(function () {
    var model = new DistribucionDeAlmacenModel();
    ko.applyBindings(model);

    $('.campoLitrosInt').keydown(function (event) {
        
        if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 16 || event.keyCode == 39 || event.keyCode == 37 || event.keyCode == 46) {
        } else if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105))) {
                event.preventDefault();
        }
        if (event.keyCode == 13) {
            model.botonAgregar();
            event.preventDefault();
        }
    });
});