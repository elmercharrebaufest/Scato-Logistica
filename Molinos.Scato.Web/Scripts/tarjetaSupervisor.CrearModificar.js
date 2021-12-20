function PuestoDeTrabajo(id, NombrePuesto) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.NombrePuesto = NombrePuesto;
    } else {
        this.Id = id.Id;
        this.NombrePuesto = id.NombrePuesto;
    }
}

function TarjetaListViewModel() {
    var self = this;
    self.puestosDeTrabajo = ko.observableArray([]);
    self.newPuestoId = ko.observable();

    $.getJSON("TarjetaSupervisor/ObtenerPuestosDeTrabajo", { id: $('#Id').val() },
       function (allData) {
            var mappedpuestosDeTrabajo = $.map(allData, function (item) {
                //inhabilito la opcion
                $("#puesto option[value=" + item.Id + "]").attr('disabled', 'disabled');
                setearDropDownPuestoSelected();
                return new PuestoDeTrabajo(item);
            });
            self.puestosDeTrabajo(mappedpuestosDeTrabajo);
            $('#puestosFinales').val(ko.toJSON(mappedpuestosDeTrabajo));
        }
    );


    // Operations
    self.addPuesto = function () {
        if ($('#puesto option:selected').text() > "") {
            self.puestosDeTrabajo.push(new PuestoDeTrabajo($('#puesto option:selected').val(), $('#puesto option:selected').text()));
            //inhabilito la opcion
            $("#puesto option:selected").attr('disabled', 'disabled');
            setearDropDownPuestoSelected();
        }
        $('#puestosFinales').val(ko.toJSON(self.puestosDeTrabajo));
    };




    self.removePuesto = function (puesto) {
       //habilito la opcion
        $("#puesto option[value=" + puesto.Id + "]").removeAttr('disabled');
        self.puestosDeTrabajo.remove(puesto);
        $('#puestosFinales').val(ko.toJSON(self.puestosDeTrabajo));
    };
}


function setearDropDownPuestoSelected() {
    var seleccionado = false;
    $.each($("#puesto option"), function (index, value) {
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


$(document).ready(function () {
    $('#puestosFinales').val("");
    ko.applyBindings(new TarjetaListViewModel());
});