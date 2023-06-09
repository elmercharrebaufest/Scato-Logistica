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
        ko.applyBindings(new EstadoPlayaInternaDeCallesViewModel(self.vmData, self.tipoCalle, self.containerId), $("#" + self.containerId)[0]);
    }
}

function EstadoPlayaInternaDeCallesViewModel(tiposCallesPlanta, tipoCalleEnUso,containerId) {
    var self = this;
    self.PatenteBuscada = ko.observable('');
    self.Calles = ko.observableArray([]);
    self.dummy = ko.observable();

    let callesOrdenadas = reordernarCalles(tiposCallesPlanta, tipoCalleEnUso, null);
    self.Calles(callesOrdenadas);

    self.TiempoEnCola = function (item) {
        if (item.Camiones.length > 0) {
            return calcularTiempoEnCola(item.Camiones[0].FechaIngreso)
        }
        return "";
    };

    setInterval(() => {
        if ($(".tabPanelEstadoPlayaInterna.active").data().calle == containerId) {
            let tiposCallesNuevasPlanta = actualizarCalles(tipoCalleEnUso);
            let callesNuevas = reordernarCalles(tiposCallesNuevasPlanta, tipoCalleEnUso, self.PatenteBuscada());
            self.Calles(callesNuevas);
        }
    }, 4000)

    self.sumarCamiones = function (materialId) {
        let count = 0;
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            count += calle.Camiones.reduce((total, camion) => camion.MaterialId == materialId ? total + 1 : total, 0);
        });
        return count;
    };

    self.sumarCamionesSoja = function (contarEPA) {
        let count = 0;
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            count += calle.Camiones.reduce((total, camion) => camion.MaterialId == 4 && camion.EsSojaEPA == contarEPA && camion.EsSojaIMPO == false ? total + 1 : total, 0);
        });
        return count;
    };

    self.sumarCamionesSojaIMPO = function (contarIMPO) {
        let count = 0;
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            count += calle.Camiones.reduce((total, camion) => camion.MaterialId == 4 && camion.EsSojaIMPO == contarIMPO ? total + 1 : total, 0);
        });
        return count;
    };

    self.CantidadSoja = ko.computed(function () { return self.sumarCamionesSoja(false); });
    self.CantidadMaiz = ko.computed(function () { return self.sumarCamiones(386); });
    self.CantidadTrigo = ko.computed(function () { return self.sumarCamiones(13); });
    self.CantidadGirasol = ko.computed(function () { return self.sumarCamiones(5); });
    self.CantidadSojaEPA = ko.computed(function () { return self.sumarCamionesSoja(true); });
    self.CantidadSojaIMPO = ko.computed(function () { return self.sumarCamionesSojaIMPO(true); });
}

function reordernarCalles(tiposCallesPlanta, tipoCalleEnUso,patenteBuscada) {
    let calles = [];
    let callesAgrupadas = [];
    let tipoCalleEnUsoArray = tipoCalleEnUso.split(",");

    $.each(tipoCalleEnUsoArray, function (index, data) {
        let callesPorGrupo = tiposCallesPlanta.filter(tipoCalles => tipoCalles.TipoCalle == data);
        if (callesPorGrupo.length > 0 && callesPorGrupo[0].Calles.length > 0) {
            callesPorGrupo[0].Calles[0].EsPrimero = true;
            callesPorGrupo[0].Calles[callesPorGrupo[0].Calles.length - 1].EsUltimo = true;
        }
        callesAgrupadas.push(...callesPorGrupo);
    })

    $.each(callesAgrupadas, function (index, data) {
        $.each(data.Calles, function (indexCalle, calle) {
            if (patenteBuscada) {
                if (calle.Camiones.filter(camion => camion.Patente.includes(patenteBuscada)).length > 0) {
                    calles.push(calle);
                }
            } else {
                calles.push(calle);
            }
        })
    })

    return calles;
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
            $(".modal-backdrop").remove()
            $(".detalleCamionModal").remove()
            $("#detalleCamion").html(result);
            $("#detalleCamionModal").modal("show");
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

function obtenerClaseEscalable(escalable) {
    return escalable ? "fas fa-truck" : "";
}

function ConfirmarCargaDescarga() {
    DeshabilitarBotonVisual("btnConfirmarCargaDescarga")
    $.ajax({
        type: 'POST',
        url: urlConfirmarCargaDescarga,
        dataType: 'json',
        data: {
            workflowInstance: $('#InstanciaWorflow').val(),
            recorridoId: $('#RecorridoId').val()
        },
        success: function (response) {
            if (response.EsValido == true) {
                MostrarAlertaExitosa("Se proceso correctamente.");
            } else {
                MostrarRespuestaMensajes(response);
                HabilitarBotonVisual("btnConfirmarCargaDescarga")
            }
        },
        error: function (error) {
            HabilitarBotonVisual("btnConfirmarCargaDescarga")
        },
        complete: function () {
            document.getElementById('modalConfirmarCargaDescarga').close();
            $.unblockUI();
        }
    });
}

function MostrarRespuestaMensajes(response) {
    response.Mensajes.forEach(function (item, index, array) {
        if (item.TipoDeMensaje === 2) {
            MostrarAlertaError(item.Mensaje);
        } else if (item.TipoDeMensaje === 1) {
            MostrarAlertaAdvertencia(item.Mensaje);
        }
    })
}

function DeshabilitarBotonVisual(id) {
    $("#" + id).attr('disabled', true);
    $("#" + id).removeClass('btn-primary');
    $("#" + id).addClass('btn-secondary');
}

function HabilitarBotonVisual(id) {
    $("#" + id).attr('disabled', false);
    $("#" + id).removeClass('btn-secondary');
    $("#" + id).addClass('btn-primary');
}



$(document).ready(function () {

    $("#pasoDirectoSoja").click(function () { EstadoSwitch('pasoDirectoSoja'); GuardarConfiguracion() })
    $("#pasoDirectoMaiz").click(function () { EstadoSwitch('pasoDirectoMaiz'); GuardarConfiguracion() })
    $("#pasoDirectoTrigo").click(function () { EstadoSwitch('pasoDirectoTrigo'); GuardarConfiguracion() })
    $("#pasoDirectoGirasol").click(function () { EstadoSwitch('pasoDirectoGirasol'); GuardarConfiguracion() })

});

function EstadoSwitch(id) { $("#" + id).val($("#" + id).is(":checked") ? 'True' : 'False') }

function GuardarConfiguracion() {

    $.ajax({
        type: 'POST',
        url: urlGuardarConfiguracion,
        dataType: 'json',
        data: {
            id: $("#idConfiguracion").val(),
            configuracionSoja: $("#pasoDirectoSoja").val(),
            configuracionMaiz: $("#pasoDirectoMaiz").val(),
            configuracionTrigo: $("#pasoDirectoTrigo").val(),
            configuracionGirasol: $("#pasoDirectoGirasol").val()
        },
        success: function (response) {
            if (response.EsValido == true) {
                MostrarAlertaExitosa("Se proceso correctamente.");
            } else {
                MostrarRespuestaMensajes(response);

            }
        },
        error: function (error) {

        },
        complete: function () {
            $.unblockUI();
        }
    });
}

