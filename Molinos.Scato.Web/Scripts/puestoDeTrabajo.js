$(document).ready(function () {
    //Inicializo el id que es obligatorio
    if ($('#Id').val() == "") {
        $('#Id').val(0);
    }
    ko.applyBindings(new DispositivoListViewModel());
});


function VideoCamara(id, codigo, descripcion, directorio) {
    this.Id = id;
    this.Codigo = codigo;
    this.Descripcion = descripcion;
    this.Directorio = directorio;
}

function Barrera(codigo, descripcion) {
    this.Codigo = codigo;
    this.Descripcion = descripcion;
}


function DispositivoListViewModel() {
    var self = this;
    self.barreras = ko.observableArray([]);
    self.barrerasEntrada = ko.observableArray([]);
    self.barrerasSalida = ko.observableArray([]);
    self.barrerasCierreSupervisor = ko.observableArray([]);
    self.videoCamaras = ko.observableArray([]);

    if ($('#BarrerasEntradaSupervisorGet').val() != "[]" && $('#BarrerasEntradaSupervisorGet').val() != "" && $('#BarrerasEntradaSupervisorGet').val() != null) {
        var barrerasEntradaSup = jQuery.parseJSON($('#BarrerasEntradaSupervisorGet').val());
        var mappedBarreras = $.map(barrerasEntradaSup, function (item) {
            //inhabilito la opcion
            $("#barrera option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barrera");
            return new Barrera(item.Codigo, item.Descripcion);
        });
        self.barreras(mappedBarreras);
    }

    if ($('#BarrerasCierreSupervisorGet').val() != "[]" && $('#BarrerasCierreSupervisorGet').val() != "" && $('#BarrerasCierreSupervisorGet').val() != null) {
        var barrerasCierreSup = jQuery.parseJSON($('#BarrerasCierreSupervisorGet').val());
        var mappedBarrerasCierre = $.map(barrerasCierreSup, function (item) {
            //inhabilito la opcion
            $("#barrera option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barrera");
            return new Barrera(item.Codigo, item.Descripcion);
        });
        self.barrerasCierreSupervisor(mappedBarrerasCierre);
    }

    if ($('#BarrerasEntradaGet').val() != "[]" && $('#BarrerasEntradaGet').val() != "" && $('#BarrerasEntradaGet').val() != null) {
        var barrerasEntrada = jQuery.parseJSON($('#BarrerasEntradaGet').val());
        var mappedBarrerasEntrada = $.map(barrerasEntrada, function (item) {
            //inhabilito la opcion
            $("#barreraEntrada option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barreraEntrada");
            return new Barrera(item.Codigo, item.Descripcion);
        });
        self.barrerasEntrada(mappedBarrerasEntrada);
    }

    if ($('#BarrerasSalidaGet').val() != "[]" && $('#BarrerasSalidaGet').val() != "" && $('#BarrerasSalidaGet').val() != null) {
        var barrerasSalida = jQuery.parseJSON($('#BarrerasSalidaGet').val());
        var mappedBarrerasSalida = $.map(barrerasSalida, function (item) {
            //inhabilito la opcion
            $("#barreraSalida option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barreraSalida");
            return new Barrera(item.Codigo, item.Descripcion);
        });
        self.barrerasSalida(mappedBarrerasSalida);
    }

    if ($('#videoCamarasFinales').val() !== undefined) {
        var camaras = jQuery.parseJSON($('#videoCamarasFinales').val());
        var mappedVideoCamaras = $.map(camaras, function (item) {
            //inhabilito la opcion
            $("#videoCamara option[value=" + item.Codigo + "]").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("videoCamara");
            return new VideoCamara(item.Id, item.Codigo, item.Descripcion, item.Directorio);
        });
        self.videoCamaras(mappedVideoCamaras);
        $('#videoCamarasFinales').val(ko.toJSON(mappedVideoCamaras));
    }

    self.addVideoCamara = function () {
        if ($('#videoCamara option:selected').text() > "" && $("#videoCamaraDirectorio").val() > "") {
            self.videoCamaras.push(new VideoCamara(0, $('#videoCamara option:selected').val(), $('#videoCamara option:selected').text(), $('#videoCamaraDirectorio').val()));
            //inhabilito la opcion
            $("#videoCamara option:selected").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("videoCamara");
            $('#videoCamaraDirectorio').val("");
        }
        $('#videoCamarasFinales').val(ko.toJSON(self.videoCamaras));
    };

    self.removeVideoCamara = function (videoCamara) {
        //habilito la opcion
        $("#videoCamara option[value=" + videoCamara.Codigo + "]").removeAttr('disabled');
        self.videoCamaras.remove(videoCamara);
        $('#videoCamarasFinales').val(ko.toJSON(self.videoCamaras));
    };

    // Operations
    self.addEntradaSupervisor = function () {
        if ($('#barrera option:selected').text() > "") {
            self.barreras.push(new Barrera($('#barrera option:selected').val(), $('#barrera option:selected').text()));
            //inhabilito la opcion
            $("#barrera option:selected").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barrera");
        }
    };
    self.addCierreSupervisor = function () {
        if ($('#barrera option:selected').text() > "") {
            self.barrerasCierreSupervisor.push(new Barrera($('#barreraCierreSup option:selected').val(), $('#barreraCierreSup option:selected').text()));
            //inhabilito la opcion
            $("#barrera option:selected").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barreraCierreSup");
        }
    };

    self.addEntrada = function () {
        if ($('#barreraEntrada option:selected').text() > "") {
            self.barrerasEntrada.push(new Barrera($('#barreraEntrada option:selected').val(), $('#barreraEntrada option:selected').text()));
            //inhabilito la opcion
            $("#barreraEntrada option:selected").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barreraEntrada");
        }
    };

    self.addSalida = function () {
        if ($('#barreraSalida option:selected').text() > "") {
            self.barrerasSalida.push(new Barrera($('#barreraSalida option:selected').val(), $('#barreraSalida option:selected').text()));
            //inhabilito la opcion
            $("#barreraSalida option:selected").attr('disabled', 'disabled');
            setearDropDownBarreraSelected("barreraSalida");
        }
    };


    self.removeCierreSupervisor = function (barrera) {
        //habilito la opcion
        $("#barreraCierreSup option[value=" + barrera.Codigo + "]").removeAttr('disabled');
        self.barrerasCierreSupervisor.remove(barrera);
    };

    self.removeEntradaSupervisor = function (barrera) {
        //habilito la opcion
        $("#barrera option[value=" + barrera.Codigo + "]").removeAttr('disabled');
        self.barreras.remove(barrera);
    };

    self.removeEntrada = function (barrera) {
        //habilito la opcion
        $("#barreraEntrada option[value=" + barrera.Codigo + "]").removeAttr('disabled');
        self.barrerasEntrada.remove(barrera);
    };

    self.removeSalida = function (barrera) {
        //habilito la opcion
        $("#barreraSalida option[value=" + barrera.Codigo + "]").removeAttr('disabled');
        self.barrerasSalida.remove(barrera);
    };


    function setearDropDownBarreraSelected(id) {
        var seleccionado = false;
        $.each($("#" + id + " option"), function (index, value) {
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

}