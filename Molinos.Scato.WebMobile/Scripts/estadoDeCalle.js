var actualizarCampos = true;

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

    self.LlamarCircular = function () {
        if (!self.Llamada() && self.CamionesEnCalle() > 0 && self.TipoCalle == 7 && self.TiempoEnColaEnMinutos() >= minutosEsperaCircular) {
            self.LLamar();
        }
    };

    self.LlamarPrecalado = function () {
        if (!self.Llamada() && self.CamionesEnCalle() > 0
            && self.TipoCalle == 1
            && self.TiempoEnColaEnMinutos() >= minutosEsperaPrecalado) {
            self.LLamar();
        }
    };

    self.LLamar = function () {
        LlamarCalle(self.Nombre, self.TipoCalle, self.Id, self.CalleCalado?.Id, self.Llamada);
    };

    self.LLamarManual = function (item, event) {
        let calleCaladoId = null;
        var caladorLleno = false;

        if (self.TipoCalle != 2){
            calleCaladoId = $(event.target).parent().parent().find('select').val();

            // $.ajax({
            //     url: urlCambioCalleCalado,
            //     data: {
            //         calleCaladoId: calleCaladoId,
            //     },
            //     async: false,
            //     type: "POST",
            //     success: function (data) {
            //         caladorLleno = data;
        
            //         if(caladorLleno){
            //             MostrarAlertaInfo("El calador seleccionado llegó a su límite");
            //         }
            //     },
            // });
        }    


        LlamarCalle(self.Nombre, self.TipoCalle, self.Id, calleCaladoId, self.Llamada);
        
    };


    self.CancelarLLamado = function () {
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: '<h5>' + cargandoGif() + ' Cancelando llamado de ' + self.Nombre + '</h5>'
        });
        $.getJSON(urlCancelarLLamarCalle, { calleId: self.Id },
            function () {
                self.Llamada(false);
                $.unblockUI();
            }
        );
    }

    self.LLamarSiguienteCalle = function () {
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: '<h5>' + cargandoGif() + ' LLamando fila para calle ' + self.Nombre + '</h5>'
        });

        $.getJSON(urlLLamarSiguienteCalle, { materialId: self.MaterialId() },
            function () {
                $.unblockUI();
            }
        );
    }

    self.LLamadoAutomatico = function () {
        actualizarCampos = false;

        $("#cambioEstadoModal").modal("show");
        $("#dialogo-confirmar-cambio-modo").click(function () {
            if ($("#motivo").val().length < 10) {
                $("#error-largo").show();
            } else {
                $("#error-largo").hide();
                $("#cambioEstadoModal").modal("hide");
                AceptarCambioModalidad(self);
                $("#dialogo-confirmar-cambio-modo").unbind("click");
            }
        });
    }
    self.CambiarModo = function () {
        actualizarCampos = false;
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: '<h5>' + cargandoGif() + (self.Automatica() ? 'Activando' : 'Desactivando') + ' llamando automático para ' + self.Nombre + '</h5>'
        });
        $.getJSON(urlLLamadoAutomaticoCalle, { calleId: self.Id, materialId: self.MaterialId(), activar: true, motivo: "" },
            function (itemActualizado) {

                self.Automatica(itemActualizado.Automatica);
                self.MaterialId(itemActualizado.MaterialId.toString());

                $.unblockUI();
            }
        );
        actualizarCampos = true;
    }
    self.TiempoEnCola = ko.computed(function () {
        return self.Posiciones().length > 0 ? self.Posiciones()[0].TiempoEnCola : "";
    });
    self.TiempoEnColaEnMinutos = ko.computed(function () {
        return self.Posiciones().length > 0 ? self.Posiciones()[0].TiempoEnColaEnMinutos : 0;
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

    self.Color = item.MaterialId == 63734 ? "bg-AceiteSj" : item.MaterialId == 4 ? "bg-soja" : item.MaterialId == 386 ? "bg-maiz" : item.MaterialId == 13 ? "bg-naranja" : item.MaterialId == 5 ? "bg-warning" : item.MaterialId == 81223 ? "bg-harina" : item.MaterialId == 63750 ? "bg-pellet" : item.MaterialId > 0 ? "bg-dark" : (self.TipoCalle == 1 ? 'bg-vacio' : "");
    self.Icon = (item.TipoCalidad == 2 ? "fas fa-tint" : item.TipoCalidad == 3 ? "fas fa-vial" : item.TipoCalidad == 1 ? "fas fa-clipboard-check" : "");
}

function AceptarCambioModalidad(calle) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: '<h5>' + cargandoGif() + (calle.Automatica() ? 'Desactivando' : 'Activando') + ' llamando automático para ' + calle.Nombre + '</h5>'
    });
    var automatica = !calle.Automatica();
    $.getJSON(urlLLamadoAutomaticoCalle, { calleId: calle.Id, materialId: calle.MaterialId(), activar: automatica, motivo: $("#motivo").val() },
        function (itemActualizado) {

            calle.Automatica(itemActualizado.Automatica);
            calle.MaterialId(itemActualizado.MaterialId.toString());
            $("#motivo").val("")
            $.unblockUI();
        }
    );
    actualizarCampos = true;
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
    self.ColorFondo = item.ColorFondo;
    self.ColorTexto = item.ColorTexto;
    self.EsSojaEPA = item.EsSojaEPA;
    self.Calle = calle;
    self.TiempoEnCola = null;
    self.TiempoEnColaEnMinutos = 0;

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


        self.TiempoEnColaEnMinutos = diffMins + (diffHrs * 60);

        diffHrs = (diffHrs < 10) ? "0" + diffHrs : diffHrs;
        diffMins = (diffMins < 10) ? "0" + diffMins : diffMins;

        self.TiempoEnCola = diffHrs < 01 && diffMins < 60 ? diffMins + 'm' : diffHrs + "h " + diffMins + 'm';
    }
    self.Icon = item.Rechazado ? "fas fa-times-circle" : (item.Calidad == 2 ? "fas fa-tint" : item.Calidad == 3 ? "fas fa-vial" : item.Calidad == 1 ? "fas fa-clipboard-check" : "");
    self.Color = item.MaterialId == 63734 ? "bg-AceiteSj" : item.MaterialId == 4 ? "bg-soja" : item.MaterialId == 386 ? "bg-maiz" : item.MaterialId == 13 ? "bg-naranja" : item.MaterialId == 5 ? "bg-warning" : item.MaterialId == 81223 ? "bg-harina" : item.MaterialId == 63750 ? "bg-pellet" : item.MaterialId > 0 ? "bg-dark" : 'bg-vacio';
    self.Escalable = item.Escalable ? "fas fa-truck" : "";
}

function EstadoDeCallesViewModel() {
    if (actualizarCampos == false) {
        return false;
    }
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
    //mejorar para que reciba lista de calles a filtrar
    self.CallesFiltradas = function (tipoCalle, tipoCalle2, tipoCalle3) {
        return ko.utils.arrayFilter(self.Calles(), function (customer) {
            if (self.PatenteBuscada()) {
                var patenteBuscada = self.PatenteBuscada();
                return (customer.TipoCalle == tipoCalle || customer.TipoCalle == tipoCalle2 || customer.TipoCalle == tipoCalle3) && customer.Posiciones().filter(function (obj) { return obj.Patente.includes(patenteBuscada); }).length > 0;
            }
            return (customer.TipoCalle == tipoCalle || customer.TipoCalle == tipoCalle2 || customer.TipoCalle == tipoCalle3);
        });
    };
    self.CallesLLamadas = function (tipoCalle, tipoCalle2, tipoCalle3, materialId) {
        self.dummy();
        //mejorar para que reciba lista de calles a filtrar
        var array = ko.utils.arrayFilter(self.Calles(), function (customer) {
            return (customer.TipoCalle == tipoCalle || customer.TipoCalle == tipoCalle2 || customer.TipoCalle == tipoCalle3) && (!materialId || materialId == customer.MaterialId()) && customer.Llamada();
        });
        return array.sort(function (a, b) { return a.FechaLLamada != null && b.FechaLLamada != null ? new Date(parseInt(a.FechaLLamada.substr(6))) - new Date(parseInt(b.FechaLLamada.substr(6))) : 1; });
    };
    self.UltimosCamiones = function (tipoCalle, tipoCalle2, tipoCalle3) {
        var camiones = [];
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            if (calle.TipoCalle == tipoCalle || calle.TipoCalle == tipoCalle2 || calle.TipoCalle == tipoCalle3) {
                camiones = camiones.concat(calle.Posiciones());
            }
        });

        return camiones
            .sort(function (a, b) { return b.Id - a.Id; })
            .slice(0, 5);
    };
    self.sumarCamiones = function (materialId) {
        let count = 0;
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            let calleId = calle.TipoCalle == 7 ? 1 : calle.TipoCalle;
            if ($('.nav-link.active').data().calle == calleId && calle.MaterialId() == materialId) {
                count += calle.CamionesEnCalle();
            }
        });
        return count;
    };
    self.sumarCamionesSoja = function (contarEPA) {
        let count = 0;
        self.dummy();
        ko.utils.arrayForEach(self.Calles(), function (calle) {
            let calleId = calle.TipoCalle == 7 ? 1 : calle.TipoCalle;
            if ($('.nav-link.active').data().calle == calleId && calle.MaterialId() == 4) {
                let camionesPorCalle = calle.Posiciones();
                $.each(camionesPorCalle, function (key, camion) {
                    if (camion.EsSojaEPA == contarEPA) {
                        count++;
                    }
                })
            }
        });
        return count;
    };
    self.Recalcular = function () {
        self.dummy.notifySubscribers();
    };
    self.CantidadSoja = ko.computed(function () { return self.sumarCamionesSoja(false); });
    self.CantidadMaiz = ko.computed(function () { return self.sumarCamiones(386); });
    self.CantidadTrigo = ko.computed(function () { return self.sumarCamiones(13); });
    self.CantidadGirasol = ko.computed(function () { return self.sumarCamiones(5); });
    self.CantidadHarina = ko.computed(function () { return self.sumarCamiones(81223); });
    self.CantidadPellet = ko.computed(function () { return self.sumarCamiones(63750); });
    self.CantidadAceiteSoja = ko.computed(function () { return self.sumarCamiones(63734); });
    self.CantidadSojaEPA = ko.computed(function () { return self.sumarCamionesSoja(true); });

    self.ListarCamiones = function () {
        $.ajax({
            url: urlEstadoDeCalles,
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            success: function (allData) {
                //obtener si hay calado automatico
                var caladoAutomatico = allData.calles.find(function (obj) { return obj.TipoCalle == 4 && obj.Automatica });

                // Obtener filas calador
                var filasCalador = allData.calles.filter(x => x.TipoCalle == 4 && x.Automatica);
                // Agregar calles precalado que han sido llamadas a las filas calador según su material
                if (filasCalador != undefined && filasCalador != null && filasCalador.length > 0) {
                    $.each(filasCalador, function (key, value) {
                        value.FilasPrecalado = allData.calles.filter(x => (x.TipoCalle == 1 || x.TipoCalle == 7) && x.MaterialId == value.MaterialId && x.Id != value.Id);
                        value.FilasPrecaladoLlamadas = allData.calles.filter(x => x.TipoCalle == 1 && x.MaterialId == value.MaterialId && x.Id != value.Id && x.FechaLLamada != null && x.Bloqueada && x.CalleCaladoId == value.Id).length;
                        value.FilasCircularLlamadas = allData.calles.filter(x => x.TipoCalle == 7 && x.MaterialId == value.MaterialId && x.Id != value.Id && x.FechaLLamada != null && x.Bloqueada & x.CalleCaladoId == value.Id).length;
                    });
                }

                ko.utils.arrayForEach(self.Calles(), function (calle) {
                    calle.CargarCamiones(allData.estado.filter(function (obj) { return obj.CalleId == calle.Id; }));
                    calle.Actualizar(allData.calles.filter(function (obj) { return obj.Id == calle.Id; }));

                    // Validar si existe calador para fila precalado o circular
                    var filaCalador = null;
                    if (filasCalador != null && filasCalador.length > 0) {
                        $.each(filasCalador, function (key, value) {
                            var callePorEvaluar = value.FilasPrecalado.find(x => x.Id == calle.Id);
                            if (callePorEvaluar != null) {
                               
                                if (callePorEvaluar.CalleCaladoId != 0) {
                                    return true;
                                }
                                filaCalador = value;
                                var limiteCalador = cantFilasPorCalador;

                                if (caladoAutomatico && calle.TipoCalle == 7) {

                                    var limite = (filaCalador.MaterialId == 4) ? 1 : limiteCalador;

                                    if (filaCalador.FilasCircularLlamadas < limite) {
                                        calle.CalleCalado = filaCalador;
                                        filaCalador.FilasCircularLlamadas++;
                                        calle.LlamarCircular();
                                    }
                                }

                                if (caladoAutomatico && calle.TipoCalle == 1) {

                                    var limite = (filaCalador.MaterialId == 4) ? limiteCalador - 1 : limiteCalador;

                                    if (filaCalador.FilasPrecaladoLlamadas < limite) {
                                        calle.CalleCalado = filaCalador;
                                        filaCalador.FilasPrecaladoLlamadas++;
                                        calle.LlamarPrecalado();
                                    }
                                }
                            }
                        });
                    }



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
    //$("#result").html("<div id='alert-reasignacioncallepostcalado' class='alert alert-success alert-dismissable'><button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>'Password Changed'</div>");
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
            $(".modal-backdrop").remove();
            $("#modal-rechazo-mover").remove();
            $("#div-rechazo-mover").html(result);
            $("#modal-rechazo-mover").modal("show");
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function CancelarReasignacionCalle() {
    $("#dialogo-confirmar").modal('hide');
}

function ConfirmarReasignacionCalle() {
    $("#modal-rechazo-mover").modal("hide");
    $("#dialogo-confirmar").modal('show');

    $.ajax({
        url: urlConfirmarReasignacionCalle,
        data: {
            instanciaWorflow: $('#InstanciaWorflow').val(),
            calleId: $('#calleId').val()
        },
        type: "POST",
        success: function (result) {
            $("#dialogo-confirmar").modal("hide");
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function AceptarReasignacionCalle() {
    $("#modal-rechazo-mover").modal("hide");
    $("#dialogo-confirmar").modal('show');
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

function LlamarCalle(nombre, tipoCalle, calleId, calleCaladoId, llamada) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: '<h5>' + cargandoGif() + ' LLamando a ' + nombre + '</h5>'
    });

    if (tipoCalle == 2) {
        $.getJSON(urlLLamarCallePostCalado, { calleId: calleId },
            function (data) {
                if (!data.EsValido) {
                    $.unblockUI();
                    MostrarAlertaInfo(data.Mensajes[0].Mensaje);
                    return;
                }

                llamada(true);
                $.unblockUI();
            }
        );
    }

    else {
        $.getJSON(urlLLamarCalle, { calleId: calleId, calleCaladoId: calleCaladoId },
            function (data) {
                if (data.Mensaje == "1") {
                    $.unblockUI();
                    MostrarAlertaInfo('Llamado de calles llegó al máximo');
                    return
                }

                if (data.Mensaje == "2") {
                    $.unblockUI();
                    MostrarAlertaInfo('Llamado de calles circulares llegó al máximo');
                    return
                }

                if (data.Mensaje == "3") {
                    $.unblockUI();
                    MostrarAlertaInfo('Llamado de calles precalado llegó al máximo');
                    return
                }
                llamada(true);
                $.unblockUI();
            }
        );
    }
}
