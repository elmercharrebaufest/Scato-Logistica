function Calle(item, context) {
    var self = this;
    self.Nombre = item.Nombre;
    self.TipoCalle = item.TipoCalle;
    self.CantidadDeCamiones = item.CantidadDeCamiones;
    self.Id = item.Id;
    self.FechaLLamada = item.FechaLLamada;
    self.Deshabilitada = ko.observable(item.Deshabilitada);
    self.Llamada = ko.observable(item.Bloqueada);
    self.Automatica = ko.observable(item.Automatica); //para calles de calado
    self.MaterialId = ko.observable(item.MaterialId.toString()); //solo trae valor para calles de calado
    self.MaterialDesc = ko.observable(item.MaterialDesc);
    self.CamionesEnCalle = ko.observable(0);
    var posiciones = [];
    for (var i = 0; i < self.CantidadDeCamiones; i++) {
        posiciones.push(new Camion({ Id: 0, Patente: '', MaterialId: null }, self));
    }
    self.Posiciones = ko.observableArray(posiciones);
    self.Bloqueada = ko.computed(function () {
        return self.Llamada() && self.CamionesEnCalle() > 0;
    });

    self.TiempoEnCola = ko.computed(function () {
        return self.Posiciones().length > 0 ? self.Posiciones()[0].TiempoEnCola : "";
    });
    self.CargarCamiones = function (camiones) {
        if (self.Llamada() && camiones.length == 0) {
            self.Llamada(false);
        }
        var posiciones = [];
        $.each(camiones, function (key, value) {
            posiciones.push(new Camion(value, self));
            self.MaterialId(value.MaterialId);
        });
        posiciones = posiciones.sort(function (a, b) { return a.Id - b.Id; });
        self.CamionesEnCalle(camiones.length);
        if (camiones.length < self.CantidadDeCamiones) {
            for (var i = camiones.length; i < self.CantidadDeCamiones; i++) {
                posiciones.push(new Camion({ Id: 0, Patente: '', MaterialId: null }, self));
            }
        }
        self.Posiciones(posiciones);
    };
    self.Actualizar = function (itemActualizado) {
        if (itemActualizado && itemActualizado.length > 0) {
            self.Deshabilitada(itemActualizado[0].Deshabilitada);
            self.FechaLLamada = itemActualizado[0].FechaLLamada;
            self.Llamada(itemActualizado[0].Bloqueada);
            if (self.TipoCalle == 4) { //Si no es calle calado, el material se actualiza al traer los camiones
                self.Automatica(itemActualizado[0].Automatica);
            }
        }
    }
    self.Color = item.MaterialId == 4 ? "bg-soja" : item.MaterialId == 386 ? "bg-maiz" : item.MaterialId == 13 ? "bg-naranja" : item.MaterialId == 5 ? "bg-warning" : item.MaterialId > 0 ? "bg-dark" : (self.TipoCalle == 1 ? 'bg-vacio' : "");
    self.Icon = (item.TipoCalidad == 2 ? "fas fa-tint" : item.TipoCalidad == 3 ? "fas fa-vial" : item.TipoCalidad == 1 ? "fas fa-clipboard-check" : "");


}

function Camion(item, calle) {
    var self = this;
    self.Id = item.Id;
    self.Patente = item.Patente;
    self.MaterialId = item.MaterialId;
    self.Calidad = item.Calidad;
    self.CalleId = item.CalleId;
    self.UltimoDeLaFila = item.UltimoDeLaFila;
    self.AsignadoEnPuestoComando = item.AsignadoEnPuestoComando;
    self.Calle = calle;
    //
    self.TiempoEnCola = null;
    if (item.FechaIngeso) {

        var fechaActual = Date.now();
        var fechaInicioDeCola = new Date(parseInt(item.FechaIngeso.substr(6)));

        let diffMilli = fechaActual - fechaInicioDeCola;
        let secondsInMilli = 1000;
        let minutesInMilli = secondsInMilli * 60;
        let hoursInMilli = minutesInMilli * 60;
        //let daysInMilli = hoursInMilli * 24;

        //let diffDays = Math.floor(different / daysInMilli);
        //diffMilli = diffMilli % daysInMilli;

        let diffHrs = Math.floor(diffMilli / hoursInMilli);
        diffMilli = diffMilli % hoursInMilli;

        let diffMins = Math.floor(diffMilli / minutesInMilli);
        diffMilli = diffMilli % minutesInMilli;

        diffHrs = (diffHrs < 10) ? "0" + diffHrs : diffHrs;
        diffMins = (diffMins < 10) ? "0" + diffMins : diffMins;

        self.TiempoEnCola = diffHrs < 01 && diffMins < 60 ? diffMins + 'm' : diffHrs + "h " + diffMins + 'm';
    }

    self.Icon = item.Rechazado ? "fas fa-times-circle" : (item.Calidad == 2 ? "fas fa-tint" : item.Calidad == 3 ? "fas fa-vial" : item.Calidad == 1 ? "fas fa-clipboard-check" : "");
    self.Color = item.MaterialId == 4 ? "bg-soja" : item.MaterialId == 386 ? "bg-maiz" : item.MaterialId == 13 ? "bg-naranja" : item.MaterialId == 5 ? "bg-warning" : item.MaterialId > 0 ? "bg-dark" : 'bg-vacio';
}

function EstadoDeCallesViewModel() {
    var self = this;
    self.Calles = ko.observableArray([]);
    self.Materiales = ko.observableArray([]);
    self.PatenteBuscada = ko.observable('');
    self.PatenteBuscada = ko.observable('');
    self.dummy = ko.observable();

    var calles = jQuery.parseJSON(callesJson);
    var mappedcalles = $.map(calles, function (item) {
        return new Calle(item, self);
    });
    self.Calles(mappedcalles);
    self.CallesFiltradas = function (tipoCalle, tipoCalle2) {
        return ko.utils.arrayFilter(self.Calles(), function (customer) {
            if (self.PatenteBuscada()) {
                var patenteBuscada = self.PatenteBuscada();
                return (customer.TipoCalle == tipoCalle || customer.TipoCalle == tipoCalle2) && customer.Posiciones().filter(function (obj) { return obj.Patente.includes(patenteBuscada); }).length > 0;
            }
            return (customer.TipoCalle == tipoCalle || customer.TipoCalle == tipoCalle2);
        });
    };
    self.CallesLLamadas = function (tipoCalle, tipoCalle2, materialId) {
        self.dummy();
        var array = ko.utils.arrayFilter(self.Calles(), function (customer) {
            return (customer.TipoCalle == tipoCalle || customer.TipoCalle == tipoCalle2) && (!materialId || materialId == customer.MaterialId()) && customer.Llamada();
        });
        return array.sort(function (a, b) { return a.FechaLLamada != null && b.FechaLLamada != null ? new Date(parseInt(a.FechaLLamada.substr(6))) - new Date(parseInt(b.FechaLLamada.substr(6))) : 1; });
    };
    self.UltimosCamiones = function (tipoCalle, tipoCalle2) {
        var camiones = [];
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            if (calle.TipoCalle == tipoCalle || calle.TipoCalle == tipoCalle2) {
                camiones = camiones.concat(calle.Posiciones());
            }
        });

        return camiones
            .sort(function (a, b) { return b.Id - a.Id; })
            .slice(0, 5);
    };
    self.sumarCamiones = function (materialId) {
        var count = 0;
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            if (calle.MaterialId() == materialId) {
                count += calle.CamionesEnCalle();
            }
        });
        return count;
    };
    self.Recalcular = function () {
        self.dummy.notifySubscribers();
    }; 
    self.CantidadSoja = ko.computed(function () { return self.sumarCamiones(4); });
    self.CantidadMaiz = ko.computed(function () { return self.sumarCamiones(386); });
    self.CantidadTrigo = ko.computed(function () { return self.sumarCamiones(13); });
    self.CantidadGirasol = ko.computed(function () { return self.sumarCamiones(5); });

    self.ListarCamiones = function () {
        $.ajax({
            url: urlEstadoDeCalles,
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            success: function (allData) {
                ko.utils.arrayForEach(self.Calles(), function (calle) {
                    calle.CargarCamiones(allData.estado.filter(function (obj) { return obj.CalleId == calle.Id; }));
                    calle.Actualizar(allData.calles.filter(function (obj) { return obj.Id == calle.Id; }));
                });
                self.Materiales(allData.materiales);
                
            },
            error: function (data) {
                setTimeout(recargar, 2000);
            },
            complete: function (data) {
                self.Recalcular();
                setTimeout(self.ListarCamiones, 4000);
            }
        });
    };
    self.ListarCamiones();
}

var recargar = function () {
    window.location.reload(true);
}

$(document).ready(function () {
    ko.applyBindings(new EstadoDeCallesViewModel());
 
});

function abrirModal() {
    self = this;
    $.ajax({
        url: urlMoverRechazado,
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

function ConfirmarEnviarAFilaRechazado() {
    $.ajax({
        url: urlConfirmarRechazado,
        data: {
            instanciaWorflow: $('#InstanciaWorflow').val()
           
        },
        type: "POST",
        success: function (result) {
            $("#modal-rechazo-mover").modal("hide");
        },
        error: function (error) {
            console.log(error);
        }
    });
}


