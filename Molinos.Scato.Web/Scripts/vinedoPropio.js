function Cuartel(id, codigo, activo) {
    if (id == 0) {
        this.Id = id;
        this.Codigo = codigo;
        this.Activo = activo;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Codigo = id.Codigo;
        this.Activo = id.Activo;

    }
}
function CuartelModel() {
    var self = this;
    self.cuarteles = ko.observableArray([]);
    if ($("#cuartelesJsonPostBack").val() != "") {
        //Si es un postback del controller: (modelo invalido)
        var mappedcuarteles = $.map(JSON.parse($("#cuartelesJsonPostBack").val()), function (item) { return new Cuartel(item); });
        self.cuarteles(mappedcuarteles);
    }
    
    self.botonCrearCuartel = function() {
        if ($("#codigoCuartel").val() != "") {
            var match = ko.utils.arrayFirst(self.cuarteles(), function (item) {
                return $("#codigoCuartel").val() === item.Codigo;
            });
            if (!match) {
                self.cuarteles.push(new Cuartel(0, $("#codigoCuartel").val(),true));
                $("#validation-message").hide();
            }
            else {               
                MostrarAlertaError("Ya existe un cuartel con el mismo código");
            }
            $("#codigoCuartel").val("");
            $("#codigoCuartel").focus();
        }
    };
    
    self.removeCuartel = function (cuartel) {
        self.cuarteles.remove(function (d) {
            return d.Codigo == cuartel.Codigo;
        });
    };
    
    self.setCheckbox = function () {
        $("#cuartelesJson").val(ko.toJSON(self.cuarteles));
        return true;
    };
}

function MostrarAlertaError(data) {
    if (data != null) {
        $("#alertaError span").text(data);
    } else {
        $("#alertaError span").text($("#alertaError").data().mensaje);
    }
    $("#alertaError").show();
    $("#alertaError").delay(500).addClass("in");
}

$(document).ready(function () {
    ko.applyBindings(new CuartelModel(), document.getElementById('grilla'));

});