var url;
var error = {};
var home;

$(document).ready(function () {
    var notificador = $.connection.notificarUsuario;
    $(".patente-internacional").mask("?*******", { placeholder: "" });

    notificador.client.actualizarNotificaciones = function (notificacion) {
        if (notificacion !== null && (notificacion.TipoAlerta == 7 || notificacion.TipoAlerta == 6)) {
            var balanza = JSON.parse(notificacion.Mensaje);
            Acualizarfoto(false, balanza.FotoAlMarcarTarjeta, balanza.Tarjeta, balanza.Id);
            if (notificacion.TipoAlerta == 7) {
                CargarbalanzadaAutomatica(balanza);
                MostrarAlertaAdvertencia(balanza.Error);
            }
            else if (balanza.Error) {
                if (balanza.Error == "Patente no reconocida") {
                    PatenteNoReconocida(balanza, notificacion.Id);
                } else if (balanza.Error == "Espera Confirmacion") {
                    MostrarEspera(balanza, notificacion.Id);
                } else {
                    ErrorBalanza(balanza, notificacion.Id);
                }
            } else {
                if (balanza.Actividad == "En Espera") {
                    RecetearbalanzadaAutomatica(balanza.Id);
                } else {
                    CargarbalanzadaAutomatica(balanza);
                    if ($("#puesto-exportacion" + balanza.Id).val() === "True") {
                        CargarDatosExpo(balanza);
                    }
                }
            }
        }

        if (notificacion !== null && notificacion.TipoAlerta == 10) {
            var estadoSensores = JSON.parse(notificacion.Mensaje);
            ModificarEstados(estadoSensores);

        }

        if (notificacion !== null && notificacion.TipoAlerta == 11) {
            var estadoSemaforos = JSON.parse(notificacion.Mensaje);
            ModificarEstadosSemaforo(estadoSemaforos);
        }

        if (notificacion !== null && notificacion.TipoAlerta == 12) {
            NotificarCambioEstadoBarrera(notificacion.Mensaje);
        }
    };

    // Start the connection
    window.hubReady.done(function () {
        notificador.server.unirseAGrupo('Automaticas');
    });

    $(".btn-aceptar").click(function () {
        VerificarPatente(this);
    });
    $(".aceptar-vagon").click(function () {
        VerificarVagon(this);
    });
    var balanzas = $(".card");
    CrearIntervals(balanzas);
    if (eventos) {
        var array = $.map(eventos, function (value, index) {
            return [value];
        });
        for (var i = 0; i < array.length; i++) {
            var balanza = array[i].Mensaje;
            if (balanza.Error) {
                if (balanza.Error == "Patente no reconocida") {
                    PatenteNoReconocida(balanza, array[i].Id);
                } else if (balanza.Error == "Espera Confirmacion") {
                    MostrarEspera(balanza, array[i].Id);
                } else {
                    ErrorBalanza(balanza, array[i].Id);
                }
            }
        }
    }
    $("#btnPantallaPrincipal").click(function () {
        $.cookie('RedireccionarABalanzaAutomatizada', false);
        $.cookie('RedireccionarAListaAutomatizada', true);
        window.location = $("#home").val();
    });

    $(document).on('click', '.slider.pausa', function () {
        CambiarModalidadBalanza(this);
    });
    $(document).on('click', '.cambio-tipo', function () {
        if ($(this).is(":checked")) {
            $("#balanzas-camiones").show();
            $("#titulo-balanzas").html("Balanzas Automáticas");
            $("#balanzas-expo").hide();
        } else {
            $("#balanzas-camiones").hide();
            $("#balanzas-expo").show();
            $("#titulo-balanzas").html("Balanzas Manuales");
        }
    });


    $(".btn-acciones-especiales").click(function () {
        event.stopPropagation();
        $.ajax({
            url: $("#accionesEspeciales").val(),
            data: {
                puestoId: $(this).data().id,
            },
            type: "GET",
            success: function (data) {
                $("#actividadesEspecialesDiv").html(data);
                $("#actividades-modal").modal('show');
            }
        });
    });

    $(document).on('click', ".tarjetaMaestro", function () {
        $("#elemento-id").val($(this).data().puestoId);
        $("#elemento-barrera").val($(this).data().id);
        $("#url").val($("#activarMaestro").val());
        $("#actividades-modal").modal("hide");
        $("#barreraModal").modal("show");
    });

    $("#guardar").click(function () {
        if (ValidarMotivo()) { return false; }

        $.ajax({
            url: $("#url").val(),
            dataType: 'json',
            data: {
                puestoId: $("#elemento-id").val(),
                balanzaId: $("#elemento-id").val(),
                codigo: $("#elemento-barrera").val(),
                motivo: $("#motivo").val()
            },
            type: "GET",
            success: function (data) {
                if (data == "ok") {
                    $("#motivo").val("");
                    $("#barreraModal").modal("hide");
                }
            }
        });
    });
    $(document).on('click', "#cerear-balanza", function () {
        $.ajax({
            url: $("#cerearBalanza").val(),
            data: {
                balanzaId: $(this).data().id,
            },
            type: "GET",
            success: function (data) {
                if (data == "ok") {
                    $("#actividades-modal").modal("hide");

                    MostrarAlertaExitosa("Cereado exitoso");
                }
            }
        });
    });

    $(document).on('click', "#forzarBalanza", function () {
        $("#elemento-id").val($(this).data().id);
        $("#elemento-barrera").val($(this).data().id);
        $("#url").val($("#avanzar").val());
        $("#actividades-modal").modal("hide");
        $("#barreraModal").modal("show");
    });

    $('.vagones').click(function (e) {

        var dropdown = $("#patente" + $(this).data().id + ".select-vagones")[0];
        RecetearbalanzadaAutomatica($(this).data().id )

        $.ajax({
            url: $("#obtenerVagonesEnBalanza").val(),
            dataType: 'json',
            type: "GET",
            success: function (result) {

                var i, L = dropdown.options.length - 1;
                for (i = L; i >= 0; i--) {
                    dropdown.options.remove(i);
                }
                if (result != null) {
                    for (var j = 0; j < result.length; j++) {
                        $(dropdown).append($("<option></option>").val(result[j].Value).html(result[j].Text));
                    }
                }
            }
        });
    });
    $(document).on('click', ".tomarPeso", TomarPeso);
    $(document).on('click', ".expo-finalizar", PesadaExportacion);
    $(document).on('click', ".expo-carga", CargaExportacion);
    $(".btn.expo-btn").attr("disabled", true);
    $(".btn-balanza").attr("disabled", true);  
    $(document).on('click', ".btn-modal-finalizar", AbrirModalFinalizarPesaje);
});

function ValidarMotivo() {
    $("#error-requerido").hide();
    $("#error-largo").hide();
    var motivo = $("#motivo").val();
    if (motivo == "") {
        $("#error-requerido").show();
        return true;
    }
    if (motivo.length < 10) {
        $("#error-largo").show();
        return true;
    }
    return false;
}

function CargarbalanzadaAutomatica(b) {
    CancelarInterval(b.Id);

    var pesoBruto = b.PesoBruto ? b.PesoBruto : 0;
    var pesoTara = b.PesoTara ? b.PesoTara : 0;

    if (!$("#aceptar" + b.Id).hasClass("vagon")) {
        $("#patente" + b.Id).prop('disabled', true);
        $("#patente" + b.Id).val(b.Patente);
        $("#patente" + b.Id).html(b.Patente);
    } else {
        $("#peso-neto-tren" + b.Id).data().bruto = pesoBruto;
    }
    $("#cp" + b.Id).html(b.CartaPorte);
    $("#material" + b.Id).html(b.Material);
    $("#entregador" + b.Id).html(b.Entregador);
    $("#tipoPeso" + b.Id).html(b.TipoPeso);
    $("#peso" + b.Id).html(b.Peso);
    $("#diferencia" + b.Id).html(b.Diferencia);
    $("#etapa" + b.Id).html(b.Actividad);
    $("#difPeso" + b.Id).html(b.DifPeso);
    $("#vehiculo" + b.Id).html(b.TipoVehiculo);
    $("#numeroTarjeta" + b.Id).html(b.Tarjeta);
    $("#difNeto" + b.Id).html(b.DifNeto);
    $("#calle" + b.Id).html(b.Calle);
    $("#tipoPesoOrigen" + b.Id).html(b.TipoPeso.replace(':','') + " Org:");
    $("#orgBruto" + b.Id).html(b.PesoBrutoOrigen);
    $("#orgNeto" + b.Id).html(b.PesoNetoOrigen);
    $("#orgTara" + b.Id).html(b.PesoBrutoOrigen - b.PesoNetoOrigen);
    $("#peso-bruto" + b.Id).html(pesoBruto);
    $("#peso-tara" + b.Id).html(pesoTara);
    $("#tipo-comercial" + b.Id).html(b.TipoComercial);
    $("#doc-ing" + b.Id).html(b.DocumentoIngreso);
    $("#instanceId" + b.Id).val(b.WorkflowInstanceId);
    if (b.PesoNetoFinal) {
        $("#peso-neto" + b.Id).val(b.PesoNetoFinal);
    }
    //if (!b.Actividad.startsWith("Pesada")) {
    //    $(".btn" + b.Id).attr("disabled",true)
    //}
    if (!(StartWitch(b.Actividad, "Pesada"))) {
        $(".btn" + b.Id).attr("disabled", true)
    }
}

function RecetearbalanzadaAutomatica(id) {
    CancelarInterval(id);
    if ($("#" + id).hasClass("carta-vagon") ||
        $("#" + id).hasClass("carta-expo")) {
        CargarbalanzadaAutomatica({
            Id: id,
            Material: "Material",
            Peso: "0",
            Diferencia: "0",
            Actividad: "En Espera",
            DifPeso: "0",
            TipoVehiculo: "Vehículo",
            DifNeto: "0",
            TipoPeso: "Peso:",
            Tarjeta: "",
            CartaPorte: "N° Documento",
            Calle: "Calle",
            PesoBrutoOrigen: "0",
            PesoNetoOrigen: "0",
            PesoBruto: "0",
            PesoTara: "0",
            Entregador: "Entregador",
            DocumentoIngreso: "Documento Ingreso",
            TipoComercial: "Tipo Comercial",
            Patente: "Patente",
            PesoNetoFinal:"0"
        });
        $("#peso-vagon"+id).html("0")
        $("#peso-neto-tren" + id).html("0")
        if ($("#" + id).hasClass("carta-vagon")) {
            $("#peso-neto-tren" + id).data().bruto = 0;
        }
        $("#peso-neto-tren" + id).html("0");
        $(".btn-finalizar-pesaje" + id).attr("disabled",true)
    } else {
        CargarbalanzadaAutomatica({
            Id: id,
            Material: "Material",
            Peso: "0",
            Diferencia: "0",
            Actividad: "En Espera",
            DifPeso: "0",
            TipoVehiculo: "Vehículo",
            DifNeto: "0",
            TipoPeso: "Peso:",
            Tarjeta: "",
            CartaPorte: "",
            Calle: "Calle",
            TipoPesoOrigen : "Peso Org:",
            PesoBrutoOrigen: "0",
            PesoNetoOrigen: "0"
        });
        $(".btn-balanza").attr("disabled", true);
    }
}

function ErrorBalanza(b, id) {
    if (b.Error == "El puesto de trabajo actual no está habilitado para ejecutar la próxima actividad") {
        return;
    }
    if (b.Actividad == "" || b.Actividad == null) {
        CancelarInterval(b.Id);
    } else {
        CargarbalanzadaAutomatica(b);
    }
    $("#etapa" + b.Id).html(b.NoRedirecciona == false ? b.Actividad : "Error: " + b.Actividad);

    $('#' + b.Id).tooltip({ 'title': b.Error, 'trigger': 'manual' });
    if (!$("#aceptar" + b.Id).hasClass("vagon")) {
        $("#aceptar" + b.Id).prop('disabled', true);
        $("#patente" + b.Id).prop('disabled', true);
    }

    ActivarInterval(b.Id);
    $('#' + b.Id).tooltip('show');
    $("#" + b.Id + " .estado-balanza").click(function (e) {
        //document.cookie = "PuestoDeTrabajoId=" + b.Id;
        if (b.NoRedirecciona == false) {
            $.cookie('PuestoDeTrabajoId', b.Id);
            window.open(url + "?id=" + b.WorkflowInstanceId + "&&proxima=" + b.Actividad + "&&notificacionId=" + id,
                "popupWindow", "width=1000, height=561, scrollbars=yes,directories=no,location=no");
            CancelarInterval(b.Id);
        } else {
            $.ajax({
                url: $("#ConfirmarNotificacion").val(),
                dataType: 'json',
                data: {
                    notificacionId: id
                },
                type: "GET",
                success: function (data) {
                    if (data == "ok") {
                        RecetearbalanzadaAutomatica(b.Id);
                    }
                }
            });
        }
    });
}

function PatenteNoReconocida(b, id) {
    CargarbalanzadaAutomatica(b);
    $("#patente" + b.Id).prop('disabled', false);
    $("#aceptar" + b.Id).prop('disabled', false);
    $('#' + b.Id).tooltip({ 'title': b.Error, 'trigger': 'manual' });
    ActivarInterval(b.Id);

    $("#tarjeta" + b.Id).val(b.Tarjeta);
    $("#notificacionId" + b.Id).val(id);
    Acualizarfoto(true, b.FotoAlMarcarTarjeta, b.Tarjeta, b.Id);

    $("#patente" + b.Id).click(function () {
        CancelarInterval(b.Id);
    });
}

function VerificarPatente(boton) {
    
    var puesto = $(boton).data().id;
    $(".btn" + puesto).attr("disabled", true);
    $.cookie('PuestoDeTrabajoId', puesto);
    $("#aceptar" + puesto).prop('disabled', true);
    $.ajax({
        url: $("#ValidarPatente").val(),
        dataType: 'json',
        data: {
            patente: $("#patente" + puesto).val(),
            puestoId: puesto,
            tarjeta: $("#tarjeta" + puesto).val(),
            peso: $("#peso" + puesto).val(),
            //notificacionId: $("#notificacionId" + puesto).val()
        },
        type: "GET",
        success: function (data) {
            if (data == "ok") {
                $("#patente" + puesto).prop('disabled', false);
                $("#aceptar" + puesto).prop('disabled', false);

                $("#peso-vagon" + puesto).html("0");
                $("#peso-neto-tren" + puesto).html("0");
                $("#peso-neto-tren" + puesto).data().bruto = 0;
            } else {
                $('#' + puesto).tooltip({ 'title': data.mensaje, 'trigger': 'manual' });
                $("#aceptar" + puesto).prop('disabled', false);
                $("#patente" + puesto).click(function () {
                    $('#' + puesto).tooltip('destroy');
                });

                ActivarInterval(puesto);
            }
        }
    });
}
function VerificarVagon(tren) {
    var puesto = $(tren).data().id;
    //$(self).prop('disabled', true);
    $.ajax({
        url: $("#validarVagon").val(),
        dataType: 'json',
        data: {
            vagon: $("#patente" + puesto).val(),
            puestoId: puesto
        },
        type: "POST",
        success: function (data) {
            if (data.Error == null) {
                CargarbalanzadaAutomatica(data);
                //$("#balanza" + puesto).attr('disabled', true);
                $(".btn" + puesto + ".tomarPeso").attr('disabled', false);
            } else {
                $('#' + data.Id).tooltip({ 'title': data.Error, 'trigger': 'manual' });
                ActivarInterval(data.Id);
                $('#' + data.Id).tooltip('show');
                $("#" + data.Id).click(function () { CancelarInterval(data.Id) });
            }
        }
    });
}

function CrearIntervals(balanzas) {
    for (var i = 0; i < balanzas.length; i++) {
        var id = $(balanzas[i]).attr('id');
        error[id] = false;
        alerta[id] = false;
        Parpadeo(id);
    }
}
function Parpadeo(id) {
    if (error[id] == true) {
        if ($("#" + id).hasClass("error")) {
            $("#" + id).removeClass("error");
        } else {
            $("#" + id).addClass("error");
        }
    }
    else {
        $("#" + id).removeClass("error");
    }
    if (alerta[id] == true) {
        if ($("#" + id).hasClass("alerta")) {
            $("#" + id).removeClass("alerta");
        } else {
            $("#" + id).addClass("alerta");
        }
    }
    else {
        $("#" + id).removeClass("alerta");
    }
    setTimeout(function () { Parpadeo(id); }, 1000);
}
function CancelarInterval(id) {
    error[id] = false;
    $('#' + id).tooltip('destroy');
    $('#' + id + " .estado-balanza").unbind("click");
    $('#' + id).css('cursor', 'auto');
}
function ActivarInterval(id) {
    error[id] = true;
    $('#' + id).tooltip('show');
    $('#' + id).css('cursor', 'pointer');
}

function CambiarModalidadBalanza(elemento) {
    var puesto = $(elemento).data().puesto;
    BlockUI();
    $.ajax({
        url: $("#cambiarModalidad").val(),
        dataType: 'json',
        data: {
            puestoId: puesto
        },
        type: "GET",
        complete: function () {
            $.unblockUI();
        }
    });
}

function Acualizarfoto(cargarFoto, fotoAlMarcarTarjeta, lectura, puestoId) {
    if (cargarFoto) {
        if (fotoAlMarcarTarjeta) {
            $.getJSON($("#obtenerFoto").val(),
                { id: lectura, puestoDeTrabajoId: puestoId },
                function (data) {
                    setTimeout(function () {
                        if (data != null && data != '') {
                            $('#noFoto' + puestoId).addClass('hidden');
                            $('#foto' + puestoId).removeClass('hidden');
                            cargarCanvas(puestoId, data);
                            //self.MostrarMensajeNoHayFoto(false);
                        } else {
                            $('#noFoto' + puestoId).removeClass('hidden');
                            $('#foto' + puestoId).addClass('hidden');
                            //self.MostrarMensajeNoHayFoto(true);
                        }
                    },
                        200);
                });
        }
    } else {
        //self.Foto('');
        //cargarCanvas(puestoId, '');
        $('#noFoto' + puestoId).addClass('hidden');
        $('#foto' + puestoId).addClass('hidden');
        //self.MostrarMensajeNoHayFoto(false);
    }
}
function cargarCanvas(id, foto) {
    var canvas = document.getElementById('canvas' + id);
    canvas.width = 380;
    canvas.height = 120;
    var gkhead = new Image;
    gkhead.src = 'data:image/jpeg;base64,' + foto;
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

        canvas.width = 380;
        redraw(ctx, gkhead, canvas);
    }

    fitImage(ctx, gkhead, canvas);
};

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
    };
}

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

function MostrarEspera(b, notificacionId) {
    b.Actividad = "Vehiculo Rechazado"
    CargarbalanzadaAutomatica(b);
    $("#etapa" + b.Id).html(b.Actividad);

    $('#' + b.Id).tooltip({ 'title': 'Esperando Confirmacion de Balancero', 'trigger': 'manual' });

    ActivarInterval(b.Id);
    $('#' + b.Id).tooltip('show');
    $("#" + b.Id + " .estado-balanza").click(function () {
        $.ajax({
            url: $("#confirmarEspera").val(),
            dataType: 'json',
            data: {
                guid: b.WorkflowInstanceId,
                notificacionId: notificacionId
            },
            type: "GET",
            success: function (data) {
                if (data == "ok") {
                    RecetearbalanzadaAutomatica(b.Id);
                }
            }
        });
    });

}
function ModificarEstados(estadoSensores) {
    if (estadoSensores.SensorVagones == true) {
        if (estadoSensores.SensorDireccionId == 70) {
            //Sensor Oeste
            if (estadoSensores.SensorVagonStatus == true) {
                $("#sensor-oeste-" + estadoSensores.PuestoId).removeClass("sensor-disponible");
                $("#sensor-oeste-" + estadoSensores.PuestoId).addClass("sensor-bloqueado");
            } else {
                $("#sensor-oeste-" + estadoSensores.PuestoId).removeClass("sensor-bloqueado");
                $("#sensor-oeste-" + estadoSensores.PuestoId).addClass("sensor-disponible");
            }
        }
        else if (estadoSensores.SensorDireccionId == 71) {
            //Sensor Este
            if (estadoSensores.SensorVagonStatus == true) {
                $("#sensor-este-" + estadoSensores.PuestoId).removeClass("sensor-disponible");
                $("#sensor-este-" + estadoSensores.PuestoId).addClass("sensor-bloqueado");
            } else {
                $("#sensor-este-" + estadoSensores.PuestoId).removeClass("sensor-bloqueado");
                $("#sensor-este-" + estadoSensores.PuestoId).addClass("sensor-disponible");
            }
        }
        else if (estadoSensores.SensorDireccionId == 72) {
            //Sensor Diganoal
            if (estadoSensores.SensorVagonStatus == true) {
                $("#sensor-diagonal-" + estadoSensores.PuestoId).removeClass("sensor-disponible");
                $("#sensor-diagonal-" + estadoSensores.PuestoId).addClass("sensor-bloqueado");
            } else {
                $("#sensor-diagonal-" + estadoSensores.PuestoId).removeClass("sensor-bloqueado");
                $("#sensor-diagonal-" + estadoSensores.PuestoId).addClass("sensor-disponible");
            }
        }
    }
    else {
        if (estadoSensores.BarreraEntradaActiva == true) {
            $("#barrera-entrada-" + estadoSensores.PuestoId).removeClass("icon-barrera-cerrada");
            $("#barrera-entrada-" + estadoSensores.PuestoId).addClass("icon-barrera-abierta");
        } else {
            $("#barrera-entrada-" + estadoSensores.PuestoId).removeClass("icon-barrera-abierta");
            $("#barrera-entrada-" + estadoSensores.PuestoId).addClass("icon-barrera-cerrada");
        }

        if (estadoSensores.BarreraSalidaActiva == true) {
            $("#barrera-salida-" + estadoSensores.PuestoId).removeClass("icon-barrera-cerrada");
            $("#barrera-salida-" + estadoSensores.PuestoId).addClass("icon-barrera-abierta");
        } else {
            $("#barrera-salida-" + estadoSensores.PuestoId).removeClass("icon-barrera-abierta");
            $("#barrera-salida-" + estadoSensores.PuestoId).addClass("icon-barrera-cerrada");
        }

        if (estadoSensores.SensorIngresoActiva == true) {
            $("#sensor-ingreso-" + estadoSensores.PuestoId).removeClass("sensor-disponible");
            $("#sensor-ingreso-" + estadoSensores.PuestoId).addClass("sensor-bloqueado");
        } else {
            $("#sensor-ingreso-" + estadoSensores.PuestoId).removeClass("sensor-bloqueado");
            $("#sensor-ingreso-" + estadoSensores.PuestoId).addClass("sensor-disponible");
        }

        if (estadoSensores.SensorTrompaActiva == true) {
            $("#sensor-trompa-" + estadoSensores.PuestoId).removeClass("sensor-disponible");
            $("#sensor-trompa-" + estadoSensores.PuestoId).addClass("sensor-bloqueado");
        } else {
            $("#sensor-trompa-" + estadoSensores.PuestoId).removeClass("sensor-bloqueado");
            $("#sensor-trompa-" + estadoSensores.PuestoId).addClass("sensor-disponible");
        }
    }
}

function TomarPeso() {
    self = this;
    if ($(self).is('[disabled=disabled]'))
        return false;

    //Toma el peso desde el orquestador
    var label = $(self).html();
    $(self).attr("disabled", true);
    var tipo = $(self).data().tipo;
    var id = $(self).data().id;
    $.getJSON($("#tomarPeso").val(), { balanzaId: $(self).data().balanzaId, instanceId: $("#instanceId" + id).val(), actividad: $("#etapa" + id).html() }, function (data) {
        if ($.isNumeric(data)) {
            $(".btn" + $(self).data().id).attr("disabled",false);
            $("#peso-" + tipo).val(data);
            $("#peso" + $(self).data().id).val(data);
            $("#peso-" + tipo.toLowerCase() + $(self).data().id).html(data);
            $("#dato-peso" + $(self).data().id).html(data);
            $("#dato-peso" + $(self).data().id).val(data);
            if ($(self).data().tipo == "vagon") {
                if ($("#peso-neto-tren" + id).data().bruto == 0) {
                    $("#peso-neto-tren" + id).html(data);
                    var difPeso =  data - Number($("#orgBruto" + id).html());
                    $("#difPeso" + id).html(difPeso);
                }
                else {
                    $("#peso-neto-tren" + id).html(Number($("#peso-neto-tren" + id).data().bruto) - data);
                    var difTara = data - Number($("#orgTara" + id).html());
                    $("#difNeto" + id).html(difTara);
                }
            }
            if ($(self).data().tipo == "bruto") {
                $(".btn" + $(self).data().id).attr("disabled", true);

                var neto = data - Number($("#peso-tara" + id).html()) ;
                $("#peso-neto" + id).html(neto);
                $(".btn" + $(self).data().id +".expo-bruto.expo-finalizar").attr("disabled", false);
            }
            if ($(self).data().tipo == "tara") {
                $(".btn" + $(self).data().id).attr("disabled", true);

                $(".btn" + $(self).data().id + ".expo-tara.expo-finalizar").attr("disabled", false);
            }
            

        } else { //Devolvió error
            MostrarAlertaError(data);
        }
    }).complete(function () {
        $(self).html(label);
        $(self).attr("disabled", false);
    });
}

function CargarDatosExpo(b) {
    $(".btn.expo-btn").attr("disabled", true);
    $("#dato-tipoPesada" + b.Id).val("Tara");
    $("#dato-rechazado" + b.Id).val(false);
    $("#dato-actividadXaml" + b.Id).val("PesadaCargaExportacionInicio");
    $("#dato-workflowInstanceId" + b.Id).val(b.WorkflowInstanceId);
    $("#dato-comentario" + b.Id).val("");
    $("#dato-mensaje" + b.Id).val("");
    $("#dato-peso" + b.Id).val("");
    $("#dato-patente" + b.Id).val(b.Patente);
    $("#dato-patenteOriginal" + b.Id).val(b.Patente);
    $("#dato-workflowDefinicionId" + b.Id).val(b.WorkflowDefinicionId)

    $.getJSON($("#obtenerDatosExpo").val(),
        { recorrido: b.WorkflowInstanceId },
        function (data) {
            $(".btn.expo-btn").attr("disabled", true);

            if (data.Actividad == "Pesada Tara Exportacion") {
                $("#" + b.Id + " .expo-tara").attr("disabled", false);
                $(".expo-finalizar").attr("disabled", true);

            } else if (data.Actividad == "Confirmacion de Carga/Descarga") {
                $("#" + b.Id + " .expo-carga").attr("disabled", false);

            } else if (data.Actividad == "Pesada Bruto Exportacion") {
                $("#dato-tipoPesada" + b.Id).val("Bruto");
                $("#" + b.Id + " .expo-bruto").attr("disabled", false);
                $(".expo-finalizar").attr("disabled", true);
            }

            $("#orgNeto" + b.Id).html(data.PesoNeto);            
        });

}

function PesadaExportacion() {
    var self = this;
    if ($(self).is('[disabled=disabled]'))
        return false;
    var puestoId = $(self).data().puesto
    $.cookie('PuestoDeTrabajoId', puestoId);

    var tipoPeso = $("#dato-tipoPesada" + puestoId).val();
    $.ajax({
        url: $("#pesadaExportacion").val(),
        type: "POST",
        dataType: 'json',
        data: {
            balanzaId: $("#dato-balanza" + puestoId).val(),
            tipoPesada: $("#dato-tipoPesada" + puestoId).val(),
            rechazado: $("#dato-rechazado" + puestoId).val(),
            actividadXaml: $("#dato-actividadXaml" + puestoId).val(),
            workflowInstanceId: $("#dato-workflowInstanceId" + puestoId).val(),
            comentario: $("#dato-comentario" + puestoId).val(),
            mensaje: $("#dato-mensaje" + puestoId).val(),
            peso: $("#dato-peso" + puestoId).val(),
            patenteOriginal: $("#dato-patenteOriginal" + puestoId).val(),
            workflowDefinicionId: $("#dato-workflowDefinicionId" + puestoId).val()
        },
        success: function (result) {
            if (result == "OK") {
                if (tipoPeso == "Tara") {
                    $("#" + puestoId + " .expo-tara").attr("disabled", true)
                    $("#" + puestoId + " .expo-carga").attr("disabled", false)
                } else {
                    LimpiarDatosExpo(puestoId);
                }
            } else {
                $('#' + puestoId).tooltip({ 'title': result, 'trigger': 'manual' });
                ActivarInterval(puestoId);
                $('#' + puestoId).tooltip('show');
                $('#' + puestoId).click(function () {
                    CancelarInterval(puestoId);
                })
            }
        }
    });
}

function CargaExportacion() {
    var self = this;
    if ($(self).is('[disabled=disabled]'))
        return false;

    var puestoId = $(self).data().puesto
    $.cookie('PuestoDeTrabajoId', puestoId);
    $.ajax({
        url: $("#cargaExportacion").val(),
        type: "POST",
        data: {
            workflowInstanceId: $("#dato-workflowInstanceId" + puestoId).val(),
            cargaParcial: $(self).hasClass("carga-parcial"),
            workflowDefinicionId: $("#dato-workflowDefinicionId" + puestoId).val()
        },
        success: function (data) {
            if (data.responseText != undefined && (data.responseText == "OK" || data.responseText == "ErrorActividadYaEjecutada")) {
                if (data.CargaParcial == true) {
                    
                    LimpiarDatosExpo(puestoId);
                }
                else {
                    $("#" + puestoId + " .expo-carga").attr("disabled", true)
                    $("#" + puestoId + " .expo-bruto.tomarPeso").attr("disabled", false)
                    $("#dato-tipoPesada" + puestoId).val("Bruto");
                }
            }
        }
    });
}
function LimpiarDatosExpo(puestoId) {
    $("dato-tipoPesada" + puestoId).val("");
    $("dato-workflowInstanceId" + puestoId).val("");
    $("dato-comentario" + puestoId).val("");
    $("dato-mensaje" + puestoId).val("");
    $("dato-peso" + puestoId).val("");
    $("dato-patente" + puestoId).val("");
    $("dato-patenteOriginal" + puestoId).val("");

    $(".btn.expo-btn").attr("disabled", true);
    RecetearbalanzadaAutomatica(puestoId);
}

function AbrirModalFinalizarPesaje() {
    if ($(this).is('[disabled=disabled]'))
        return false;

    var id = $(this).data().id;

    $("#finalizar-pesaje").data().id = id;
    var peso = Number($("#peso-bruto" + id).html()) == 0 ? $("#difPeso" + id).html() : $("#difNeto" + id).html();
    $("#peso-modal").html(peso);
    $("#confirmar-pesada-vagon").modal("show");
}

function ModificarEstadosSemaforo(estadoSemaforos) {
    LimpiarLedSemaforo(estadoSemaforos);

    if (estadoSemaforos.Color == "ROJO") { //Color Rojo  
        $("#semaforo-led-rojo-" + estadoSemaforos.PuestoId).removeClass("semaforo-color-default");
        $("#semaforo-led-rojo-" + estadoSemaforos.PuestoId).addClass("semaforo-color-rojo");
    }

    if (estadoSemaforos.Color == "AMARILLO") { //Color Verde
        $("#semaforo-led-amarillo-" + estadoSemaforos.PuestoId).removeClass("semaforo-color-default");
        $("#semaforo-led-amarillo-" + estadoSemaforos.PuestoId).addClass("semaforo-color-amarillo");
    }

    if (estadoSemaforos.Color == "VERDE") { //Color Amarillo
        $("#semaforo-led-verde-" + estadoSemaforos.PuestoId).removeClass("semaforo-color-default");
        $("#semaforo-led-verde-" + estadoSemaforos.PuestoId).addClass("semaforo-color-verde");
    }
}

function LimpiarLedSemaforo(estadoSemaforos) {
    $("#semaforo-led-rojo-" + estadoSemaforos.PuestoId).removeClass("semaforo-color-rojo");
    $("#semaforo-led-rojo-" + estadoSemaforos.PuestoId).addClass("semaforo-color-default");

    $("#semaforo-led-amarillo-" + estadoSemaforos.PuestoId).removeClass("semaforo-color-amarillo");
    $("#semaforo-led-amarillo-" + estadoSemaforos.PuestoId).addClass("semaforo-color-default");

    $("#semaforo-led-verde-" + estadoSemaforos.PuestoId).removeClass("semaforo-color-verde");
    $("#semaforo-led-verde-" + estadoSemaforos.PuestoId).addClass("semaforo-color-default");
}

function StartWitch(input, validation) {
    try {
        if (input != '' || input != null) {
            if (input.indexOf(validation) == 0) {
                return true;
            } else {
                return false;
            }
        }
        else {
            return false;
        }
    } catch (e) {
        return false;
    }
}