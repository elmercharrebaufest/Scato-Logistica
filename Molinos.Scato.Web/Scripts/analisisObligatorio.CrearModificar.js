function PuestoDeTrabajo(id, nombrePuesto) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.NombrePuesto = nombrePuesto;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.NombrePuesto = id.NombrePuesto;
    }
}

function ViewModel() {
    var self = this;
    self.puestos = ko.observableArray([]);

    $.getJSON("AnalisisObligatorio/ObtenerPuestos", { id: $('#Id').val() },
        function (allData) {
            var mappedPuestos = $.map(allData, function (item) {
                //inhabilito la opcion
                $("#puesto option[value=" + item.Id + "]").attr('disabled', 'disabled');
                setearDropDownRolSelected();
                return new PuestoDeTrabajo(item);
            });
            self.puestos(mappedPuestos);
            $('#puestosFinales').val(ko.toJSON(mappedPuestos));
        }
    );
    
    // Operations
    self.addPuesto = function () {
        if ($('#puesto option:selected').text() > "") {
            self.puestos.push(new PuestoDeTrabajo($('#puesto option:selected').val(), $('#puesto option:selected').text()));
            //inhabilito la opcion
            $("#puesto option:selected").attr('disabled', 'disabled');
            setearDropDownRolSelected();
        }
        $('#puestosFinales').val(ko.toJSON(self.puestos));

    };
    
    self.removePuesto = function (rol) {
        //habilito la opcion
        $("#puesto option[value=" + rol.Id + "]").removeAttr('disabled');
        self.puestos.remove(rol);
        $('#puestosFinales').val(ko.toJSON(self.puestos));
    };

}

$(document).ready(function () {
    DefinirAutocompletar('#MaterialDescripcion', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial);
    $("#MaterialDescripcion").autocomplete("option", "appendTo", "#dialogo-editar");
    ko.applyBindings(new ViewModel());
});

function setearDropDownRolSelected() {
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