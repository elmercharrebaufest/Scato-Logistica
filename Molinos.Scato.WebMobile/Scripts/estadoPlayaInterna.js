function EstadoPlayaInternaDeCallesVM(config) {
    this.containerId = config.containerId;
    this.vmData = config.vmData;
    this.tipoCalle = config.tipo;
}

EstadoPlayaInternaDeCallesVM.prototype = {
    onReady: function () {
        let self = this;
        self.vm = {
            mainModule: {},
        };

        self.init();
    },
    init: function () {
        let self = this;
        ko.applyBindings(new EstadoPlayaInternaDeCallesViewModel(self.vmData, self.tipoCalle), $("#" + self.containerId)[0]);
    }
}

function EstadoPlayaInternaDeCallesViewModel(tiposCallesPlanta, tipoCalle) {
    var self = this;
    self.PatenteBuscada = ko.observable('');
    self.Calles = ko.observableArray([]);
    let calles = [];
    // Agregar calles al array
    $.each(tiposCallesPlanta, function (indexTipoCallePlanta, tipoCallePlanta) {
        $.each(tipoCallePlanta.Calles, function (indexCalle, calle) {
            calles.push(calle);
        })
    })

    self.Calles(calles);

    self.TiempoEnCola = function (item) {
        if (item.Camiones.length > 0) {
            return calcularTiempoEnCola(item.Camiones[0].FechaIngreso)
        }
        return "";
    };

    setInterval(() => {
        if ($(".tabPanelEstadoPlayaInterna.active").data().calle == tipoCalle) {
            let tiposCallesNuevasPlanta = actualizarCalles(tipoCalle);
            let callesNuevas = [];
            // Agregar calles al array del observable
            $.each(tiposCallesNuevasPlanta, function (index, tipoCalleNuevaPlanta) {
                $.each(tipoCalleNuevaPlanta.Calles, function (index2, calleNueva) {
                    if (self.PatenteBuscada()) {
                        let patenteBuscada = self.PatenteBuscada();
                        if (calleNueva.Camiones.filter(camion => camion.Patente.includes(patenteBuscada)).length > 0) {
                            callesNuevas.push(calleNueva);
                        }
                    } else {
                        callesNuevas.push(calleNueva);
                    }
                })
            })
            self.Calles(callesNuevas);
        }
    }, 4000)
}

function obtenerClaseIcono(rechazado, calidad) {
    let icon = rechazado ? "fas fa-times-circle" : (calidad == 2 ? "fas fa-tint" : calidad == 3 ? "fas fa-vial" : calidad == 1 ? "fas fa-clipboard-check" : "");
    return icon;
}

function actualizarCalles(tipo) {
    let calles;
    $.ajax({
        url: urlEstadoDeCalles+"?tiposCalleStr="+tipo,
        type: 'GET',
        contentType: 'application/json;',
        dataType: 'json',
        async: false,
        success: function (allData) {
            calles = allData;
        },
        error: function (data) {
        },
        complete: function (data) {
        }
    });
    return calles;
}

function abrirModal() {
    self = this;
    $.ajax({
        url: urlMostrarDetalleCamion,
        data: {
            patente: self.Patente,
            calleId: self.CalleId
        },
        type: "POST",
        success: function (result) {
            $("#div-rechazo-mover").html(result);
            $("#modal-rechazo-mover").modal("show");
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function calcularTiempoEnCola(fechaIngeso) {
    var fechaActual = Date.now();
    var fechaInicioDeCola = new Date(parseInt(fechaIngeso.substr(6)));

    let diffMilli = fechaActual - fechaInicioDeCola;
    let secondsInMilli = 1000;
    let minutesInMilli = secondsInMilli * 60;
    let hoursInMilli = minutesInMilli * 60;

    let diffHrs = Math.floor(diffMilli / hoursInMilli);
    diffMilli = diffMilli % hoursInMilli;

    let diffMins = Math.floor(diffMilli / minutesInMilli);
    diffMilli = diffMilli % minutesInMilli;

    diffHrs = (diffHrs < 10) ? "0" + diffHrs : diffHrs;
    diffMins = (diffMins < 10) ? "0" + diffMins : diffMins;

    return diffHrs < 01 && diffMins < 60 ? diffMins + 'm' : diffHrs + "h " + diffMins + 'm';
}

