function Sensor(id, CodigoDispositivoSensorArriba, CodigoDispositivoSensorAbajo, CodigoDispositivoSensorQuiebre, Barrera, BarreraBajar, NombreBarrera) {
    if (id == 0) {
        this.Id = id;
        this.NombreBarrera = NombreBarrera;
        this.CodigoDispositivoSensorArriba = CodigoDispositivoSensorArriba;
        this.CodigoDispositivoSensorAbajo = CodigoDispositivoSensorAbajo;
        this.CodigoDispositivoSensorQuiebre = CodigoDispositivoSensorQuiebre;
        this.Barrera = Barrera;
        this.BarreraBajar = BarreraBajar;
        this.VisualizacionBarreraId = $("#VisualizacionBarreraId").val();
        this.EsNuevo = true;
    } else {
        //id trae el objeto que ya existia
        this.Id = id.Id;
        this.NombreBarrera = id.NombreBarrera;
        this.CodigoDispositivoSensorArriba = id.CodigoDispositivoSensorArriba.toString();
        this.CodigoDispositivoSensorAbajo = id.CodigoDispositivoSensorAbajo.toString();
        this.CodigoDispositivoSensorQuiebre = id.CodigoDispositivoSensorQuiebre.toString();
        this.Barrera = id.Barrera.toString();
        this.BarreraBajar = id.BarreraBajar.toString();
        this.VisualizacionBarreraId = id.CaracteristicaDeCalidadId;
        this.EsNuevo = id.EsNuevo;
        this._destroy = id._destroy;
    }
}

function SensoresListViewModel() {
    var self = this;
    self.sensoresBarrera = ko.observableArray([]);

    self.newNombreBarrera = ko.observable();
    self.newCodigoDispositivoSensorArriba = ko.observable();
    self.newCodigoDispositivoSensorAbajo = ko.observable();
    self.newCodigoDispositivoSensorQuiebre = ko.observable();
    self.newBarrera = ko.observable();
    self.newBarreraBajar = ko.observable();

    if ($("#sensoresBarreraPostBack").val() != "") {

        var mappedSensores = $.map(JSON.parse($("#sensoresBarreraPostBack").val()), function (item) { return new Sensor(item); });

        $.each(mappedSensores, function (index, value) {
            value.NombreBarrera = value.NombreBarrera;
            value.CodigoDispositivoSensorArriba = value.CodigoDispositivoSensorArriba;
            value.CodigoDispositivoSensorAbajo = value.CodigoDispositivoSensorAbajo;
            value.CodigoDispositivoSensorQuiebre = value.CodigoDispositivoSensorQuiebre;
            value.Barrera = value.Barrera;
            value.BarreraBajar = value.BarreraBajar;
        });

        self.sensoresBarrera(mappedSensores);

    } else {
        $.getJSON($("#botonCrearSensor").data().obtenerSensores, { grupoId: $('#VisualizacionBarreraId').val() },
            function (allData) {
                var mappedSensores = $.map(allData, function (item) { return new Sensor(item); });
                $.each(mappedSensores, function (index, value) {
                    value.NombreBarrera = value.NombreBarrera;
                    value.CodigoDispositivoSensorArriba = value.CodigoDispositivoSensorArriba;
                    value.CodigoDispositivoSensorAbajo = value.CodigoDispositivoSensorAbajo;
                    value.CodigoDispositivoSensorQuiebre = value.CodigoDispositivoSensorQuiebre;
                    value.Barrera = value.Barrera;
                    value.BarreraBajar = value.BarreraBajar;
                });

                self.sensoresBarrera(mappedSensores);
            }
        );
    }

    // Operations
    self.botonCrearSensor = function () {
        self.sensoresBarrera.push(new Sensor(0, self.newCodigoDispositivoSensorArriba(), self.newCodigoDispositivoSensorAbajo(), self.newCodigoDispositivoSensorQuiebre(), self.newBarrera(), self.newBarreraBajar(), self.newNombreBarrera()));
    };

    self.removeSensorBarrera = function (sensor) {
        self.sensoresBarrera.destroy(sensor);
    };
}

