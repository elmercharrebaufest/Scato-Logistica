function VinedoListViewModel() {
    // Data
    var self = this;
    
    if ($("#Hectareas").val() != null) {
        self.hectareas = ko.observable($("#Hectareas").val());
    } else {
        self.hectareas = ko.observable(0);
    }
    
    if ($("#TopeHectarea").val() != null) {
        self.topeHectarea = ko.observable($("#TopeHectarea").val());
    } else {
        self.topeHectarea = ko.observable(0);
    }
    
    if ($("#KgRecibidos").val() != null) {
        self.recibidos = ko.observable($("#KgRecibidos").val());
    } else {
        self.recibidos = ko.observable(0);
    }

    self.kgARecibir = ko.computed(function () {
        var resultado = parseInt((reverseFormat(self.hectareas()) * reverseFormat(self.topeHectarea())) - reverseFormat(self.recibidos()));
        return isNaN(resultado) ? "" : resultado;
    });
    self.porcRecibidos = ko.computed(function () {
        return formatWithComma((reverseFormat(self.recibidos()) / (reverseFormat(self.kgARecibir()) + reverseFormat(self.recibidos()))) * 100);
    });
}

$(document).ready(function () {
    var viewModel = new VinedoListViewModel();
    ko.applyBindings(viewModel);

    $(".cosecha").mask("9999");

    $('#VariedadId ,#Cosecha').change(function () {
        if ($('#VinedoId').val() != null && $('#VariedadId').val() != 0 && $('#Cosecha').val().length == 4) {
            BlockUI($("#variedadDesc").data().buscandorecibidos);
            $.getJSON($("#variedadDesc").data().obtenerRecibidos, { vinedoId: $("#VinedoId").val(), variedadId: $('#VariedadId').val(), cosecha: $('#Cosecha').val() },
                function (allData) {
                    viewModel.recibidos(allData);

                    $.unblockUI();
                }
            );
        }
    });
});


// Formatting Functions
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
    return Globalize.parseFloat(x.toString());
};
