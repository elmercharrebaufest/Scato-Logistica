//FUNCIONES PRINCIPALES ABM
//Automatismo

var TipoMensaje = {
    Success: 0,
    Warning: 1,
    Error: 2
}

$(document).ready(function () {
    //Funcion Actualiza estadoGeneral de AutomatismoNoGrano
    $('#automatismoGeneral').change(function (e) {
        let element = e.currentTarget;
        element.checked = !element.checked;
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: 'Cargando...'
        });
        $.ajax({
            url: urlActualizarEstadoAutomatismoGeneral,
            data: { nuevoEstado: !element.checked },
            success: function (response) {
                if (response.TipoDeMensaje == TipoMensaje.Success) {
                    element.checked = !element.checked;
                } else {
                    MostrarAlertaError(response.Mensaje);
                }
            },
            error: function () {
                MostrarAlertaError('Error al realizar la petición');
            },
            complete: function () {
                $.unblockUI();
            }
        });
    });

    //FUNCIONES SLIDERS
    //Funcion Detecta y Redirecciona las Consultas

    $('.cambia-estado').click(function (e) {
        let element = e.currentTarget;
        element.checked = !element.checked;
        let identidad = $(this).data('identity');

        switch (identidad) {
            case 'calleplanta':
                cambiarEstadoSwitch(urlActualizarEstadoCallePlanta, element);
                break;
            case 'puntodecarga':
                cambiarEstadoSwitch(urlActualizarEstadoPuntoDeCarga, element);
                break;
            case 'almacen':
                cambiarEstadoSwitch(urlActualizarEstadoAlmacen, element);
                break;
            case 'automatismo':
                cambiarEstadoSwitch(urlActualizarEstadoAutomatismoNoGrano, element);
                break;
        }
    });
})

function funcionModalCrear() {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        type: "GET",
        url: urlCrearAutomatismo,
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            $("#partialModal").find(".modal-body").html(response);
            $("#partialModal").modal('show');
            $(".modal-title").text("Crear Automatismo");
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function funcionModalModificarAutomatismoNoGrano(item) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        type: "GET",
        url: urlModificarAutomatismo,
        contentType: "application/json; charset=utf-8",
        data: { id: item },
        success: function (response) {
            $("#partialModal").find(".modal-body").html(response);
            $("#partialModal").modal('show');
            $(".modal-title").text("Modal Modificar");
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function funcionModalEliminarAutomatismoNoGrano(idAutomatismo) {
    $("#automatismoIdABorrar").val(idAutomatismo);
    modalConfirmarBorrar.showModal();
}

function funcionEliminarAutomatismoNoGrano() {
    modalConfirmarBorrar.close();
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlEliminarAutomatismo,
        method: "POST",
        data: { id: $("#automatismoIdABorrar").val() },
        success: function (response) {
            if (response.EsValido == true) {
                MostrarAlertaExitosa();
            } else {
                MostrarAlertaAdvertencia();
            }
            recargarListaAutomatismos();
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

//Configuraciones
function funcionModalModificarCallePlanta(item) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        type: "GET",
        url: urlModificarCallePlanta,
        contentType: "application/json; charset=utf-8",
        data: { id: item },
        success: function (response) {
            $("#partialModal").find(".modal-body").html(response);
            $("#partialModal").modal('show');
            $(".modal-title").text("Modificar Calle Planta");
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function funcionModalModificarPuntoDeCarga(item) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        type: "GET",
        url: urlModificarPuntoDeCarga,
        contentType: "application/json; charset=utf-8",
        data: { id: item },
        success: function (response) {
            $("#partialModal").find(".modal-body").html(response);
            $("#partialModal").modal('show');
            $(".modal-title").text("Modal Modificar Punto de Carga");
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

//Funcion Consulta Ajax Generica para Reutilizar
function cambiarEstadoSwitch(url, elemento) {
    let id = $(elemento).data('id');
    let valor = elemento.checked;
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: url,
        data: { nuevoEstado: !valor, id: id },
        success: function (response) {
            if (response.TipoDeMensaje == TipoMensaje.Success) {
                elemento.checked = !valor;
            } else {
                MostrarAlertaError(response.Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición');
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

var refreshAutomatismo = function (response) {
    mostrarMensajeDeRespuesta(response)
    recargarListaAutomatismos();
}

var refreshCallePlanta = function (response) {
    mostrarMensajeDeRespuesta(response)
    recargarListaCallePlanta();
}

var refreshPuntoDeCarga = function (response) {
    mostrarMensajeDeRespuesta(response)
    recargarListaPuntoDeCarga();
}

function mostrarMensajeDeRespuesta(response) {
    if (!response.EsValido) {
        var mensajes = [];
        response.Mensajes.forEach(function (item, index, array) {
            mensajes.push(item.Mensaje);
        })
        MostrarAlertaError(mensajes.join("<br>"))
    } else {
        $('#partialModal').modal('hide');
        MostrarAlertaExitosa("Se proceso correctamente.");
    }
}

function recargarListaAutomatismos() {
    $.ajax({
        url: urlListarAutomatismoNoGrano,
        success: function (listaActualizada) {
            $("#gridContainer").html(listaActualizada);
        }
    });
}

function recargarListaCallePlanta() {
    $.ajax({
        url: urlListarCallePlanta,
        success: function (listaActualizada) {
            $("#gridCallePlanta").html(listaActualizada);
        }
    });
}

function recargarListaPuntoDeCarga() {
    $.ajax({
        url: urlListarPuntoDeCarga,
        success: function (listaActualizada) {
            $("#gridPuntoDeCarga").html(listaActualizada);
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

function dllCallePlantaOnChange(e) {
    var selectedCalleId = e;

    $.ajax({
        url: urlObtenerPuntosDeCargaPorMaterialId,
        data: { calleId: selectedCalleId },
        success: function (puntosDeCarga) {
            $('#ddlPuntosDeCarga').find('option').not(':first').remove();
            var ddlPuntosDeCarga = $("#ddlPuntosDeCarga");

            $.each(puntosDeCarga, function (indice, puntoDeCarga) {
                ddlPuntosDeCarga.append("<option value='" + puntoDeCarga.Value + "'>" + puntoDeCarga.Text + "</option>");
            });
        }
    });
}