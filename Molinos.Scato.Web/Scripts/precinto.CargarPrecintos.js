function Precinto(id, precinto, detalle) {
    if (id == 0) {
        this.Id = id;
        this.WorkflowInstanceId = $('#guid').val();
        this.NumeroPrecinto = precinto;
        this.Detalle = detalle;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.WorkflowInstanceId = id.WorkflowInstanceId;
        this.NumeroPrecinto = id.NumeroPrecinto;
        this.Detalle = id.Detalle;
    }
}

function PrecintoListViewModel() {
    // Data
    var self = this;
    self.precintos = ko.observableArray([]);
    self.newPrecintoTexto = ko.observable();
    self.newPrecintoDetalle = ko.observable();

    // Operations
    self.addPrecinto = function () {
        if (self.newPrecintoTexto() > "") {
            var viewModel = this;
            var existe = false;
            var precintosActuales = self.precintos();
            $.each(precintosActuales, function(index, value) {
                if (value.NumeroPrecinto == viewModel.newPrecintoTexto() && value._destroy != true) {
                    existe = true;
                }
            });
            if (!existe) {
                if (!this.newPrecintoDetalle()) {
                    this.newPrecintoDetalle($('#detallePrecinto').val());
                }
                self.precintos.push(new Precinto(0, this.newPrecintoTexto(), this.newPrecintoDetalle()));
                self.newPrecintoTexto("");
                self.newPrecintoDetalle("");
            } else {
                $.get(this.href, cargarDialogoVer($('#error')));
            }
            $('#detallePrecinto').val("");
            $('.validaPrecinto').hide();
        } else {
            $('.validaPrecinto').show();
        }
        $('#numeroPrecinto').focus();
        //$('#precintos').validate().element('#numeroPrecinto');
    };
    
    self.removePrecinto = function (precinto) { self.precintos.destroy(precinto); };

}

$(document).ready(function() {

    ko.applyBindings(new PrecintoListViewModel());
    
    $.validator.addMethod("numeroPrecintoInvalido", function (value, element) {
        return ( ($('#numeroPrecinto').val().length > 0) /*|| ($('#numeroPrecinto').is(":focus"))*/ );
    }, $('#numeroPrecinto').data().errorPrecintoInvalido);

    $('#numeroPrecinto').focus();

    $.getJSON("Precinto/ObtenerPrecintos", { instanceId: $('#guid').val() },
        function (allData) {
            var mappedPrecintos = $.map(allData, function (item) { return new Precinto(item); });
            self.precintos(mappedPrecintos);
        }
    );
});
