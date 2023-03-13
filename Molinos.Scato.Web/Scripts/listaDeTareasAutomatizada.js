var i = 0;
function fitImage(ctx, gkhead, canvas) {
    if (gkhead.height == 1080 /* ||gkhead.height == 720*/) {
        // Clear the entire canvas
        var p1 = ctx.transformedPoint(0, 0);
        var p2 = ctx.transformedPoint(canvas.width, canvas.height);
        ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

        ctx.scale(0.5, 0.5);

        ctx.save();
        ctx.setTransform(1, 0, 0, 1, 0, 0);
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.restore();
        //ctx.drawImage(gkhead, -500, -100);
        ctx.drawImage(gkhead,
            canvas.width / 4 - gkhead.width / 4,
            canvas.height / 4 - gkhead.height / 4
        );
    }

}

function PuestoDeTrabajo(id) {
    var self = this;
    self.Id = id.Id;
    self.NombrePuesto = id.NombrePuesto;
    self.PidePatente = id.PidePatente;
    self.Automatico = id.Automatico;
    self.Automatizado = id.Automatizado;
    self.FotoAlMarcarTarjeta = id.FotoAlMarcarTarjeta;
    self.EstadoConexion = ko.observable(id.EstadoConexion);
    self.MensajeConexion = ko.observable(id.MensajeConexion);
    self.Indice1 = ko.observable(i++);
    self.IndicePantente = ko.observable("Patente" + i);
    self.MostrarMensajeNoHayFoto = ko.observable(false);
    self.Patente = ko.observable();
    self.PatenteCorrecta = ko.observable(null);
    self.VisualizacionBarrera_Id = id.VisualizacionBarrera_Id;
    self.Sensores = ko.observableArray([]);

    if (id.VisualizacionBarrera_Id) {
        self.Sensores(id.VisualizacionBarrera.SensoresBarreras);
    }

    self.consultarPatenteVehiculo = function () {
        var data = { tarjetaDeAcceso: self.Lectura() };
        $.ajax({
            url: $("#DocumentoOrigenConsultaUrl").val(),
            type: "POST",
            data: JSON.stringify(data),
            contentType: "application/json; charset=utf-8",
            async: false,
            success: function (result) {
                if (result.status == "success") {
                    window.open(result.redirectTo, '_blank').focus();
                } else if (result.status == "error") {
                    MostrarAlertaError(result.message);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    };

    self.Foto = ko.observable('');
    self.EsAutomatizado = ko.observable(false);
    function cargarCanvas() {
        var canvas = document.getElementById('canvas' + self.Id);
        canvas.width = 0;
        canvas.height = 400;
        var gkhead = new Image;
        gkhead.src = 'data:image/jpeg;base64,' + self.Foto();
        var ctx = canvas.getContext('2d');
        trackTransforms(ctx);
        redraw(ctx, gkhead, canvas);

        var lastX = 0, lastY = 0;

        var dragStart, dragged;

        canvas.addEventListener('mousedown', function (evt) {
            document.body.style.mozUserSelect = document.body.style.webkitUserSelect = document.body.style.userSelect = 'none';
            lastX = evt.pageX;
            lastY = evt.pageY;
            dragStart = ctx.transformedPoint(lastX, lastY);
            dragged = false;

            $(document).attr('unselectable', 'on')
                .css({
                    '-moz-user-select': '-moz-none',
                    '-moz-user-select': 'none',
                    '-o-user-select': 'none',
                    '-khtml-user-select': 'none', /* you could also put this in a class */
                    '-webkit-user-select': 'none',/* and add the CSS class here instead */
                    '-ms-user-select': 'none',
                    'user-select': 'none'
                }).bind('selectstart', function () { return false; });
        }, false);

        document.addEventListener('mousemove', function (evt) {
            lastX = evt.pageX;
            lastY = evt.pageY;
            dragged = true;
            if (dragStart) {
                var pt = ctx.transformedPoint(lastX, lastY);
                ctx.translate(pt.x - dragStart.x, pt.y - dragStart.y);
                redraw(ctx, gkhead, canvas);
            }
        }, false);

        var zoom = function (clicks) {
        }

        document.addEventListener('mouseup', function (evt) {
            $(document).removeAttr('unselectable');
            $(document).removeAttr("style");
            $(document).attr('unselectable', 'on');
            $(document).unbind('selectstart');

            dragStart = null;
            if (!dragged) zoom(evt.shiftKey ? -1 : 1);
        }, false);

        window.addEventListener('resize', resizeCanvas, false);
        resizeCanvas();
        function resizeCanvas() {

            canvas.width = $('#aceptarContainer').width() - ($('#aceptarContainer').width() * 3.8 / 100);
            redraw(ctx, gkhead, canvas);
        }

        fitImage(ctx, gkhead, canvas);
    }

    self.Acualizarfoto = function (patente) {
        if (patente) {
            if (self.Automatico && self.FotoAlMarcarTarjeta) {
                $.getJSON($("#obtenerFoto").val(),
                    { id: self.Lectura(), puestoDeTrabajoId: self.Id },
                    function (data) {
                        self.Foto(data);
                        setTimeout(function () {

                            if (data != null && data != '') {
                                cargarCanvas();
                                self.MostrarMensajeNoHayFoto(false);
                            } else {
                                self.MostrarMensajeNoHayFoto(true);
                                console.log("MostrarMensajeNoHayFoto", self.MostrarMensajeNoHayFoto());
                            }
                        },
                            200);
                    });
            }
        } else {
            self.Foto('');
            self.MostrarMensajeNoHayFoto(false);
        }
    };

    if (id.Automatico && id.PrimerLectura != null && id.PrimerLectura != "") {
        self.Lectura = ko.observable(id.PrimerLectura);
        self.Acualizarfoto(id.Patente);
        setTimeout(function () {
            cargarPatenteLeida(self, id.PatenteLeida, id.Patente, false, id.PrimerLectura, id.OcrActivo, id.ReconocimientoExitoso);
        }, 500);

    }
    else if (id.Automatico) {
        self.Lectura = ko.observable($("#esperandoLectura").val());
    } else {
        self.Lectura = ko.observable();
    }

    self.HayLectura = ko.computed(function () {
        return self.PidePatente && (self.Lectura() != $('#esperandoLectura').val());
    });
    self.HayFoto = ko.computed(function () {
        return (self.Lectura() != $('#esperandoLectura').val()) && self.Foto() != '' && self.Foto() != null;
    });

    self.Lectura.valueHasMutated();

    setInterval(function () {
        //console.log("PatenteCorrecta", self.PatenteCorrecta() + "-" + self.Patente());

        if (self.PatenteCorrecta() && self.PatenteCorrecta() !== self.Patente()) {
            BordeColor(self.IndicePantente(), false);
        } else if (self.PatenteCorrecta() && self.PatenteCorrecta() === self.Patente()) {
            BordeColor(self.IndicePantente(), true);
        } else {
            BordeColor(self.IndicePantente());
        }
    }, 500);


    //if (sessionStorage.getItem('EsAutomatizado') === "true") {
    if (window.name === "true") {
        self.EsAutomatizado(true);
    }
    self.EsAutomatizado.subscribe(function (newValue) {
        //sessionStorage.setItem('EsAutomatizado', newValue);
        window.name = newValue;
        if (newValue) {
            $("body").css("background-color", "rgb(169, 219, 169)");
        } else {
            $("body").css("background-color", "rgba(252, 239, 161, 1)");
        }
    });
    if (self.EsAutomatizado()) {
        $("body").css("background-color", "rgb(169, 219, 169)");
    }
}


function compara(d1, d2) {
    if (d1.Automatico == d2.Automatico) return 0;
    return d1.Automatico == true ? 1 : -1;
}

function cargarPatenteLeida(puesto, patenteLeida, patente, hayError, lectura, ocrActivo, reconocimientoExitoso) {
    //puesto.Patente(''); 
    puesto.PatenteCorrecta(patente);
    if (puesto.EsAutomatizado() && hayError) {
        $("body").css("background-color", "rgba(252, 239, 161, 1)");
    }
    if (!patente) {
        if (!hayError) {
            $("#validation-patente").html("La tarjeta leída (" + lectura + ") no se encuentra asociada a un camión activo");
            $("#validation-patente-alert").removeClass("hide");
            if (puesto.EsAutomatizado()) {
                $("body").css("background-color", "rgba(252, 239, 161, 1)");
            }
        }
    }
    else {
        if (patenteLeida) {
            if (reconocimientoExitoso) {
                puesto.Patente(patente);
            } else {
                puesto.Patente(patenteLeida);
            }

            if (puesto.EsAutomatizado()) {
                if (reconocimientoExitoso) {
                    $("#search-form").submit();
                } else {
                    $("body").css("background-color", "rgba(252, 239, 161, 1)");
                }
            }
        } else if (ocrActivo) {
            if (!hayError) {
                $("#validation-patente").html("No se pudo leer la patente en la imagen");
                $("#validation-patente-alert").removeClass("hide");
                if (puesto.EsAutomatizado()) {
                    $("body").css("background-color", "rgba(252, 239, 161, 1)");
                }
            }
        }
    }
}

function PuestoDeTrabajoViewModel() {
    // Inicializo observers
    var self = this;
    self.puestosDeTrabajo = ko.observableArray([]);

    if ($("#puestosDeTrabajo").val() != "") {

        var mappedPuestoDeTrabajo = $.map(JSON.parse($("#puestosDeTrabajo").val()), function (item) { return new PuestoDeTrabajo(item); });

        self.puestosDeTrabajo(mappedPuestoDeTrabajo);
        self.puestosDeTrabajo.sort(compara);
    }

    self.puestosDeTrabajoGrouped = ko.computed(function () {
        var rows = [], current = [];
        var rowsSen = [], currentSen = [];
        rows.push(current);
        for (var i = 0; i < self.puestosDeTrabajo().length; i += 1) {
            current.push(self.puestosDeTrabajo()[i]);
            if (((i + 1) % 2) === 0) {
                current = [];
                rows.push(current);
            }
        }
        return rows;
    }, this);

    //////////////
    var notificaLectura = $.connection.notificaLectura;
    var notificarUsuario = $.connection.notificarUsuario;

    notificaLectura.client.informarLectura = function (notificacion) {
        $("#validation-patente-alert").addClass("hide");
        $("#validation-patente-danger").addClass("hide");
        var hayError = false;
        if (!notificacion.TarjetaValida && !notificacion.EsTarjetaSupervisor) {
            $("#validation-patente").html(notificacion.MensajeError);
            $("#validation-patente-alert").removeClass("hide");
            hayError = true;
        } else if (notificacion.EsTarjetaSupervisor) {
            $("#validation-danger").html(notificacion.MensajeError);
            $("#validation-patente-danger").removeClass("hide");
            hayError = true;
        }
        if (notificacion.PrimerNumeroDeTarjeta === notificacion.NumeroDeTarjeta) {
            $.each(self.puestosDeTrabajo(), function (index, value) {
                if (value.Id == notificacion.PuestoDeTrabajoId) {
                    if (value.Automatico && notificacion.PrimerNumeroDeTarjeta != null && notificacion.PrimerNumeroDeTarjeta != "") {
                        value.Lectura(notificacion.PrimerNumeroDeTarjeta);
                        value.Acualizarfoto(notificacion.Patente);
                    } else if (value.Automatico) {
                        value.Lectura($("#esperandoLectura").val());
                        value.Acualizarfoto(false);
                    } else {
                        value.Lectura = ko.observable();
                        value.Acualizarfoto(false);
                    }
                    cargarPatenteLeida(value, notificacion.PatenteLeida, notificacion.Patente, hayError, notificacion.PrimerNumeroDeTarjeta, notificacion.OcrActivo, notificacion.ReconocimientoExitoso);
                }
            });
        }
    };

    notificaLectura.client.informarEstadoConexion = function (notificacion) {
        $.each(self.puestosDeTrabajo(), function (index, value) {
            if (value.Id == notificacion.PuestoDeTrabajoId) {
                value.EstadoConexion(notificacion.Estado);
                value.MensajeConexion(notificacion.Mensaje);
            }
        });
    };

    notificarUsuario.client.actualizarNotificaciones = function (notificacion) {
        if (notificacion !== null && notificacion.TipoAlerta == 13) {
            var estadoSensores = JSON.parse(notificacion.Mensaje);
            ModificarEstados(estadoSensores);
        }
    };

    self.removeLectura = function (puesto) {
        $.getJSON($('#eliminarUltimaLectura').val(), { puestoDeTrabajoId: puesto.Id },
            function (allData) {
                puesto.Patente("");
                puesto.PatenteCorrecta(null);
                $("#validation-patente-alert").addClass("hide");
                if (puesto.EsAutomatizado()) {
                    $("body").css("background-color", "rgb(169, 219, 169)");
                }
                if (puesto.Automatico && allData.ProximaLectura != null && allData.ProximaLectura != "") {
                    puesto.Lectura(allData.ProximaLectura);
                    puesto.Acualizarfoto(allData.ProximaPatente);
                    cargarPatenteLeida(puesto, allData.ProximaPatenteLeida, allData.ProximaPatente, false, allData.ProximaLectura, allData.OcrActivo, allData.ReconocimientoExitoso);
                } else {
                    puesto.Lectura($("#esperandoLectura").val());
                    puesto.Acualizarfoto(false);
                }
            }
        );
    };

    BlockUI($("#cargando").val());
    // Start the connection
    try {
        window.hubReady.done(function () {
            $.each(self.puestosDeTrabajo(), function (index, value) {
                if (value.Automatico) {
                    notificaLectura.server.escucharPuestosDeTrabajo($('#centroId').val(), value.Id);
                }
            });
            notificarUsuario.server.unirseAGrupo('SensoresBarreraHidraulica');
            $.unblockUI();
        }).fail(function (error) {
            window.location.href = window.location.href;
        });
    }
    catch (err) {
        window.location.href = window.location.href;
    }
    //////////////
}

function BordeColor(indice, bool) {
    if (bool) {
        $("#" + indice).css('border', 'solid 1px green');
        $("#" + indice).css('border-right', 'solid 5px green');
        $("#" + indice).addClass("italic");
    } else if (bool === false) {
        $("#" + indice).css('border', 'solid 1px red');
        $("#" + indice).css('border-right', 'solid 5px red');
        $("#" + indice).removeClass("italic");
    }
    else {
        $("#" + indice).css('border', 'solid 1px #CCCCCC');
        $("#" + indice).removeClass("italic");
    }
}

$(document).ready(function () {
    $.validator.addMethod("PatenteRequerida", function (value, element) {
        return value.length > 0;
    }, $('#validation-patente-alert').data().errorRequerido);

    ko.applyBindings(new PuestoDeTrabajoViewModel());

    $(".patente-internacional").mask("?*******", { placeholder: "" });
    $(".filaValida").mask("9999999999");

    $("#validation-patente-close").on("click", function () {
        $("#validation-patente-alert").addClass("hide");
        return false;
    });
    $("#validation-danger-close").on("click", function () {
        $("#validation-patente-danger").addClass("hide");
        return false;
    });
    //$(".PatenteRequerida").on('focusout', function () {
    //    $("#validation-patente-alert").addClass("hide");
    //    return false;
    //});

    $("input:visible:enabled:not([readonly]):first").focus();

    Mousetrap.stopCallback = function (e, element, combo) {
        return false;
    };

    Mousetrap.bind('alt+1', function () {
        $("#boton-aceptar").click();
    });
    $(document).on("submit", "form.causaBlock", function () {
        Mousetrap.pause();
    });

    $(document).on("submit", "form", function () {
        formSubmit = this;
    });

    $("#btnPantallaPrincipal").click(function () {
        $.cookie('RedireccionarABalanzaAutomatizada', true);
        $.cookie('RedireccionarAListaAutomatizada', false);
        window.location = $("#home").val();
    });



});
var formSubmit = null;
function respuestaForm(data, status, xhr) {
    if (!data.valida && data.mensaje != null && data.mensaje != "") {
        $("#validation-patente").html(data.mensaje);
        $("#validation-patente-alert").removeClass("hide");
        if ($("body").css("background-color") == "rgb(169, 219, 169)") {
            $("body").css("background-color", "rgba(252, 239, 161, 1)");
        }

        if (formSubmit != null) {
            $(formSubmit).find('.PatenteRequerida').first().focus();
            formSubmit = null;
        }

        $.unblockUI();
    } else if (!data.valida) {
        if (data.valida == undefined) {
            window.location = $("#sessionExpirada").val();
        } else {
            MostrarAlertaError();
        }
        $.unblockUI();
    } else {
        $.cookie('PuestoDeTrabajoId', data.puestoId);
        window.location = data.url;
    }
}

function respuestaFormError(xhr, status, error) {
    $.unblockUI();
    window.location = $("#errorGenerico").val();
}


function redraw(ctx, gkhead, canvas) {

    // Clear the entire canvas
    var p1 = ctx.transformedPoint(0, 0);
    var p2 = ctx.transformedPoint(canvas.width, canvas.height);
    ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

    ctx.save();
    ctx.setTransform(1, 0, 0, 1, 0, 0);
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.restore();
    ctx.drawImage(gkhead, -500, -100);

}

function trackTransforms(ctx) {
    var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    var xform = svg.createSVGMatrix();
    ctx.getTransform = function () { return xform; };

    var savedTransforms = [];
    var save = ctx.save;
    ctx.save = function () {
        savedTransforms.push(xform.translate(0, 0));
        return save.call(ctx);
    };

    var restore = ctx.restore;
    ctx.restore = function () {
        xform = savedTransforms.pop();
        return restore.call(ctx);
    };

    var scale = ctx.scale;
    ctx.scale = function (sx, sy) {
        xform = xform.scaleNonUniform(sx, sy);
        return scale.call(ctx, sx, sy);
    };

    var rotate = ctx.rotate;
    ctx.rotate = function (radians) {
        xform = xform.rotate(radians * 180 / Math.PI);
        return rotate.call(ctx, radians);
    };

    var translate = ctx.translate;
    ctx.translate = function (dx, dy) {
        xform = xform.translate(dx, dy);
        return translate.call(ctx, dx, dy);
    };

    var transform = ctx.transform;
    ctx.transform = function (a, b, c, d, e, f) {
        var m2 = svg.createSVGMatrix();
        m2.a = a; m2.b = b; m2.c = c; m2.d = d; m2.e = e; m2.f = f;
        xform = xform.multiply(m2);
        return transform.call(ctx, a, b, c, d, e, f);
    };

    var setTransform = ctx.setTransform;
    ctx.setTransform = function (a, b, c, d, e, f) {
        xform.a = a;
        xform.b = b;
        xform.c = c;
        xform.d = d;
        xform.e = e;
        xform.f = f;
        return setTransform.call(ctx, a, b, c, d, e, f);
    };

    var pt = svg.createSVGPoint();
    ctx.transformedPoint = function (x, y) {
        pt.x = x; pt.y = y;
        return pt.matrixTransform(xform.inverse());
    }
}

$(document).on('click', ".barreraTareasAuto", function () {
    $("#elemento-id").val($(this).data().puestoId);
    $("#elemento-barrera").val($(this).data().id);
    $("#actividades-modal").modal("hide");
    $("#barreraModal").modal("show");

    $("#aceptarComandoBarrera").click(function () {
        if (ValidarMotivo()) { return false; }

        $.ajax({
            url: $("#GestionarBarreraUrl").val(),
            dataType: 'json',
            data: {
                puestoId: $("#elemento-id").val(),
                codigo: $("#elemento-barrera").val(),
                motivo: $("#motivo").val(),
                accion: 'levantar'
            },
            type: "GET",
            success: function (data) {
                if (data == "ok") {
                    $("#motivo").val("");
                    $("#barreraModal").modal("hide");
                }
            }
        });
    })
});

function ValidarMotivo() {
    $("#error-requerido-hidraulica").hide();
    $("#error-largo-hidraulica").hide();
    var motivo = $("#motivo").val();
    if (motivo == "") {
        $("#error-requerido-hidraulica").show();
        return true;
    }
    if (motivo.length < 10) {
        $("#error-largo-hidraulica").show();
        return true;
    }
    return false;
}

function ModificarEstados(estadoSensores) {
    $.each(estadoSensores.SensoresArriba, function (index, sensor) {
        if (sensor.Estado == true) {
            $("#barrera-hidraulica-" + sensor.PuestoDeTrabajoId).removeClass("icon-barrera-cerrada");
            $("#barrera-hidraulica-" + sensor.PuestoDeTrabajoId).addClass("icon-barrera-abierta");
        }
    })

    $.each(estadoSensores.SensoresAbajo, function (index, sensor) {
        if (sensor.Estado == true) {
            $("#barrera-hidraulica-" + sensor.PuestoDeTrabajoId).removeClass("icon-barrera-abierta");
            $("#barrera-hidraulica-" + sensor.PuestoDeTrabajoId).addClass("icon-barrera-cerrada");
        }
    })
}