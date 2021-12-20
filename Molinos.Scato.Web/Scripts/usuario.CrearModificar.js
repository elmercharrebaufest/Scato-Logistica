function Rol(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}

function Centro(id, descripcion) {
    if ($.isNumeric(parseInt(id))) {
        this.Id = id;
        this.Descripcion = descripcion;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.Descripcion = id.Descripcion;
    }
}

function UsuarioListViewModel() {
    var self = this;
    self.roles = ko.observableArray([]);
    self.centros = ko.observableArray([]);

    $.getJSON("Usuario/ObtenerRoles", { id: $('#Id').val() },
        function (allData) {
            var mappedRoles = $.map(allData, function (item) {
                //inhabilito la opcion
                $("#rol option[value=" + item.Id + "]").attr('disabled', 'disabled');
                setearDropDownRolSelected();
                return new Rol(item);
            });
            self.roles(mappedRoles);
            $('#rolesFinales').val(ko.toJSON(mappedRoles));
        }
    );

    $.getJSON("Usuario/ObtenerCentros", { id: $('#Id').val() },
        function (allData) {
            var mappedCentros = $.map(allData, function (item) {
                //inhabilito la opcion
                $("#centro option[value=" + item.Id + "]").attr('disabled', 'disabled');
                setearDropDownCentroSelected();
                return new Centro(item);
            });
            self.centros(mappedCentros);
            $('#centrosFinales').val(ko.toJSON(mappedCentros));
        }
    );

    // Operations
    self.addRol = function () {
        if ($('#rol option:selected').text() > "") {
            self.roles.push(new Rol($('#rol option:selected').val(), $('#rol option:selected').text()));
            //inhabilito la opcion
            $("#rol option:selected").attr('disabled', 'disabled');
            setearDropDownRolSelected();
        }
        $('#rolesFinales').val(ko.toJSON(self.roles));

    };
    
    self.addCentro = function () {
        if ($('#centro option:selected').text() > "") {
            self.centros.push(new Centro($('#centro option:selected').val(), $('#centros option:selected').text()));
            //inhabilito la opcion
            $("#centro option:selected").attr('disabled', 'disabled');
            setearDropDownCentroSelected();
        }
        $('#centrosFinales').val(ko.toJSON(self.centros));

    };

    self.removeRol = function (rol) {
        //habilito la opcion
        $("#rol option[value=" + rol.Id + "]").removeAttr('disabled');
        self.roles.remove(rol);
        $('#rolesFinales').val(ko.toJSON(self.roles));
    };
    
    self.removeCentro = function (centro) {
        //habilito la opcion
        $("#centro option[value=" + centro.Id + "]").removeAttr('disabled');
        self.centros.remove(centro);
        $('#centrosFinales').val(ko.toJSON(self.centros));
    };

}

function setearDropDownRolSelected() {
    var seleccionado = false;
    $.each($("#rol option"), function (index, value) {
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

function setearDropDownCentroSelected() {
    var seleccionado = false;
    $.each($("#centro option"), function (index, value) {
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

    ko.applyBindings(new UsuarioListViewModel());
});
