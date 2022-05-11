var estaActualizado = false;
$(document).ready(function () {
    // remueve los submenues vacios
    $('li.menu-adminrepo li.dropdown-submenu').each(function () {
        var item = $(this);
        if (item.find('a[href!="#"]').length == 0) {
            item.remove();
        }
    });

    $('#idiomacentro').on('show.bs.dropdown', function () {
        if ($('#centrosDescripciones').html() == "") {
            recargarCentros();
        }
    });
    
    $(document).on('click', '#refrescarCookies', (function () {
        $.removeCookie("PuestoDeTrabajoId");
        $.removeCookie("RedireccionarAListaAutomatizada");
        $.removeCookie("RedireccionarABalanzaAutomatizada");
        $.ajax({
            type: "GET",
            url: $("#BorrarPermisosCookie").val()
        }).complete(function () {
            $.ajax({
                type: "GET",
                url: $("#ActualizarCookie").val()
            });
            window.location = $("#ListaDeCamionesUrl").val();

        });             
    }));
    
    $(document).on('click', '#centrosDescripciones li', (function () {
        $('#centroActualBoton').text($(this).data().centro);
        $.ajax({
            url: $("#centroActualBoton").data().seleccionarcentro,
            data: { centroId: $(this).data().centroid },
            traditional: true,
            beforeSend: function () {
                BlockUI($("#centroActualBoton").data().mensajeEspera);
            },
            success: function (e) {
                $(conectarSignalR());
                recargarCentros();
                $.unblockUI();
                MostrarAlertaExitosa();
                $.removeCookie("PuestoDeTrabajoId");
                $.removeCookie("RedireccionarAListaAutomatizada");
                window.location = $("#ListaDeCamionesUrl").val();
            },
            error: function () {
                $.unblockUI();
                MostrarAlertaError();
            }
        });
        
    }));
    
    $('#idiomas li').on('click', (function() {
        $('#idiomaBoton').text($(this).data().idioma);
    }));

    $("#NotificacionesDropDownButton").on('click', (function() {
        if (!$("#NotificacionesDropDown").hasClass("open")) {
            if (!estaActualizado) {
                recargarNotificaciones(true, true, true);
            }
            actualizarTimeAgo();
        }
    }));

    $(document).on('click', '.noLeido', (function () {
        var item = $(this);
        actualizarJewelCount($('#unreadbadge').text() - 1);
        item.removeClass("noLeido");
        notificarLectura(item.data().id, item.data().grupo);
    }));

    $(document).on('click', '.eliminarNotificacion',(function () {
        var item = $(this);
        var liparent = item.closest('li');
        if (liparent.hasClass("noLeido")) actualizarJewelCount($('#unreadbadge').text() - 1);
        $.getJSON($("#NotificacionesDropDown").data().eliminarNotificacionUrl, { id: item.data().id }, function () {
            liparent.remove();
        });
        return false;
    }));

    $(document).on('click', "#BorrarTodas", (function () {

        $('#messages li').each(function () {
            var item = $(this);
            if (item.data().id != null) {
                if (item.hasClass("noLeido")) actualizarJewelCount($('#unreadbadge').text() - 1);
                item.remove();
            }
        });
        $.ajax({
            url: $("#NotificacionesDropDown").data().eliminarNotificacionesUrl,
            data: null,
            traditional: true,
            success: function (data) {
                if (data.grupos.length > 0) {
                    estaActualizado = false;
                }
            }
        });
    }));

    $(conectarSignalR());

    $("li.view-dashboard-barreras").click(function () {
        $("#dashboardBarrerasModal").modal("show");
    });

    $(document).on('click', ".tarjetaMaestroSupervisorBarrera", function () {
        $("#elemento-id-Menu").val($(this).data().puestoId);
        $("#elemento-barrera-Menu").val(obtenerDispositivoBarrera($(this).data().id, 3));
        $("#url-Menu").val($("#GestionarBarrera").val());
        $("#elemento-accion-Menu").val(obtenerDispositivoBarrera($(this).data().id, 1));
        $("#actividades-modal").modal("hide");
        $("#modalGestionBarrera").modal("hide");

        if ($("#gestionarBarrera").val() == 'True') {
            if ($("#requiereComentarioGestionarBarrera").val() == 'True') {
                $("#gestionbarreraSupervisorModal").modal("show");
            } else {
                EjecutarComandoBarrera();
            }
        }
        else {
            $("#gestionbarreraSupervisorModal").modal("show");
        }        
    });

    $("#guardarEventoBarrera").click(function () {
        if (ValidarMotivoMenu()) { return false; }
        EjecutarComandoBarrera();
    });

    $("#barreraHeader").click(function () {
        if ($("#cantidadBarreras").val() && $("#cantidadBarreras").val() == 1) {
            $("#modalGestionBarrera").modal("show");
        }        
    });
    if ($("#barreraHeader")) {
        $.post($("#ActualizarEstadoBarrera").val());
    }

});

function conectarSignalR() {
    var notificador = $.connection.notificarUsuario;

    notificador.client.actualizarNotificaciones = function (notificacion) {
        if (notificacion !== null) {
            mostrarAlertaPorPantalla(notificacion.TipoAlerta, notificacion.Mensaje);
            notificarLectura(notificacion.Id, notificacion.Grupo);
        } else {
            recargarNotificaciones(true, true, true);
        }
    };

    
    // Start the connection
    window.hubReady.done(function () {
        recargarNotificaciones(false, true, true);
        notificador.server.unirseAGrupo(grupos);
        notificador.server.unirseAGrupo('SENSORESBARRERA');
        if ($("#barreraHeader")) {
            $.post($("#ActualizarEstadoBarrera").val());
        }
    });
}

function recargarCentros() {
    $.getJSON($("#centroActualBoton").data().listarcentros, null,
            function (response) {
                var options = '';
                for (var i = 0; i < response.length; i++) {
                    

                    if (response[i].Descripcion == $('#centroActualBoton').text()) {
                        options += '<li data-centro="' + response[i].Descripcion + '" data-centroid="' + response[i].Id + '"><a href="#">' + response[i].Descripcion + ' <i class="icon-ok"></i></a></li>';
                    } else {
                        options += '<li data-centro="' + response[i].Descripcion + '" data-centroid="' + response[i].Id + '"><a href="#">' + response[i].Descripcion + '</a></li>';
                    }
                }
                $('#centrosDescripciones').html(options);
            });
}

function recargarNotificaciones(listarSobre, mostrarAlerta, contar) {
    if (listarSobre) {
        limpiarNotificaciones();
    }
    $.getJSON($("#NotificacionesDropDown").data().obtenerNotificacionesUrl, { listarSobre: listarSobre, mostrarAlerta: mostrarAlerta, contar: contar },
        function (response) {
            if (mostrarAlerta && response.AlertasNoLeidas != null && response.AlertasNoLeidas.length > 0) {

                var mensaje =  response.AlertasNoLeidas[0].Mensaje;
                var tipoAlerta = response.AlertasNoLeidas[0].TipoAlerta;
                
                for (var j = 1; j < response.AlertasNoLeidas.length; j++) {
                    if (tipoAlerta == response.AlertasNoLeidas[j].TipoAlerta) {
                        
                        mensaje += (tipoAlerta == 4 ? '\n' : '<br />') + response.AlertasNoLeidas[j].Mensaje;
                        
                    } else {
                        mostrarAlertaPorPantalla(tipoAlerta, mensaje);
                        mensaje = response.AlertasNoLeidas[j].Mensaje;
                        tipoAlerta = response.AlertasNoLeidas[j].TipoAlerta;
                    }
                }
                mostrarAlertaPorPantalla(tipoAlerta, mensaje);

                for (var b = 0; b < response.AlertasNoLeidas.length; b++) {
                    notificarLectura(response.AlertasNoLeidas[b].Id, response.AlertasNoLeidas[b].Grupo);
                    response.Cantidad = response.Cantidad - 1;
                }
            }
            
            if (listarSobre && response.NotificacionesSobre != null) {
                var options = '';
                for (var i = 0; i < response.NotificacionesSobre.length; i++) {
                    options += notificacionDropDownItem(response.NotificacionesSobre[i]);
                }
                options += '<li id="BorrarTodas"><a href="#"><div>' + $("#messages").data().eliminar + '<i class="icon-trash"></i></div></a></li>';
                $('#messages').html(options);
            }
            if (contar) {
                actualizarJewelCount(response.Cantidad);
            }
            estaActualizado = listarSobre;
        });
}

function mostrarAlertaPorPantalla(tipoAlerta, mensaje) {
    if (tipoAlerta == 0) {
        MostrarAlertaExitosa(mensaje);
    } else if (tipoAlerta == 1) {
        MostrarAlertaAdvertencia(mensaje);
    } else if (tipoAlerta == 2) {
        MostrarAlertaError(mensaje);
    } else if (tipoAlerta == 3) {
        MostrarAlertaInfo(mensaje);
    } else if (tipoAlerta == 4) {
        alert(mensaje);
    } else if (tipoAlerta == 8) {
        ActualizarEstadoServicios(mensaje);
    } else if (tipoAlerta == 9) {
        NotificarCPMesaEntrada(mensaje);
    } else if (tipoAlerta == 12) {
        NotificarCambioEstadoBarrera(mensaje);
    }
}

function notificarLectura(id, grupo) {
    $.getJSON($("#NotificacionesDropDown").data().marcarLeidosUrl, { id: id });
}

function notificarLecturaTodos(grupos) {
    $.getJSON($("#NotificacionesDropDown").data().marcarLeidosTodosUrl, null,
        function () {
            estaActualizado = false;
            });
}

function limpiarNotificaciones() {
    $('#messages li').each(function() {
        var item = $(this);
        if (item.data().id != null) {
            item.remove();
        }
    });
    $('#messages').html('<li><a><div>' + $("#messages").data().cargando + '</i></div></a></li>');
}

function actualizarJewelCount(cantidad) {
    if (cantidad == 0) {
        $('#unreadbadge').hide();
    } else {
        $('#unreadbadge').text(cantidad > 99 ? '+99' : cantidad);
        $('#unreadbadge').show();
    }
}

function notificacionDropDownItem(notificacion) {
    var item = '<li';
    if (!notificacion.Leido) item += ' class="noLeido"';
    item += ' data-id="' + notificacion.Id + '" data-grupo="' + notificacion.Grupo + '">' +
            '<a href="#">' + '<button type="button" class="eliminarNotificacion" data-id="' + notificacion.Id + '">×</button>' +
                '<div class="notificacion-body">' +
                    '<div class="notificacion-mensaje">' + notificacion.Mensaje + '</div>' +
                    '<div class="notificacion-hora">' +
                        '<i class="icon-time"></i>' +
                        '<span>' + actualizarTimeAgo(notificacion.HoraServidor, notificacion.HoraVista) + '</span>' +
                    '</div>' +
                '</div>' +
            '</a>' +
        '</li>';
    return item;
}

function actualizarTimeAgo(server, hora) {
    var serverd = new Date(server);
    var horad = new Date(hora);
    var distance = (serverd.getTime() - horad.getTime()),
				seconds = Math.abs(distance) / 1000,
				minutes = seconds / 60,
				hours = minutes / 60;
    if (hours < 24) {
        return Globalize.format(horad, Globalize.culture().calendars.standard.patterns.t);
    } else {
        return Globalize.format(horad, 'D');
    }
}

(function ($) {
    $.whenAll = function (deferreds) {
        return $.when.apply($, deferreds);
    };
})(jQuery);

function obtenerDispositivoBarrera(id, length) {
    var resultado = "";

    try {
        if (id != null && id != "") {
            var splitId = id.split("-");
            if (splitId.length >= length) {
                resultado = splitId[length-1];
            }
        }
    } catch (error) {

    }

    return resultado;
}

function ValidarMotivoMenu() {
    $("#error-requerido").hide();
    $("#error-largo").hide();
    var motivo = $("#motivoMenu").val();
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

function EjecutarComandoBarrera() {
    $.ajax({
        url: $("#url-Menu").val(),
        dataType: 'json',
        data: {
            puestoId: $("#elemento-id-Menu").val(),
            codigo: $("#elemento-barrera-Menu").val(),
            motivo: $("#motivoMenu").val(),
            accion: $("#elemento-accion-Menu").val()
        },
        type: "GET",
        success: function (data) {
            if (data == "ok") {
                $("#motivoMenu").val("");
                $("#gestionbarreraSupervisorModal").modal("hide");
            }
        }
    });
}
