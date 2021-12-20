$(document).ready(function () {
    var viewModel = new StockEstablecimientoViewModel();
    ko.applyBindings(viewModel);
    
    $("#Cosecha").mask("99-99");
    $.validator.addMethod("cosechaValidacion", function (value, element) {
        var cosecha = $("#Cosecha").val().split('-');
        if (parseInt(cosecha[0]) > parseInt(cosecha[1]))
            return false;
        return true;
    }, $("#Cosecha").data().error);

    $.validator.addMethod("cosechaValidacionDiferencia", function (value, element) {
        var cosecha = $('#Cosecha').val().split('-');
        if (parseInt(cosecha[1]) - parseInt(cosecha[0]) !== 1)
            return false;
        return true;
    }, $("#Cosecha").data().errordiferencia);
});

function StockEstablecimientoViewModel() {
    var self = this;

    if ($("#StockDeclarado").val() != null) {
        self.stockDeclarado = ko.observable($("#StockDeclarado").val());
    } else {
        self.stockDeclarado = ko.observable(0);
    }

    if ($("#StockUtilizado").val() != null) {
        self.stockUtilizado = ko.observable($("#StockUtilizado").val());
    } else {
        self.stockUtilizado = ko.observable(0);
    }

    self.stockDisponible = ko.computed(function () {
        var resultado = parseFloat(reverseFormat(self.stockDeclarado()) - reverseFormat(self.stockUtilizado()));
        return isNaN(resultado) ? "" : decimalGlobal(resultado);
    });
}

function reverseFormat(x) {
    return Globalize.parseFloat(x.toString());
};


function decimalGlobal(x) {
    x = x.toString();
    var decimal = Globalize.culture().numberFormat["."];
    var parametro = x.replace(",", decimal);
    parametro = parametro.replace(".", decimal);
    return parametro;
};
