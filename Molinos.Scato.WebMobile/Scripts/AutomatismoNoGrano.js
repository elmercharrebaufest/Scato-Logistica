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
                    recargarListaAutomatismos();
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
    bindearEventos();
    //FUNCIONES SLIDERS
    //Funcion Detecta y Redirecciona las Consultas


})


function bindearEventos() {
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
}

function ModalCrear() {
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

function ModalModificarAutomatismoNoGrano(item) {
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
            $(".modal-title").text("Modificar Automatismo");
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

function ModalEliminarAutomatismoNoGrano(idAutomatismo) {
    $("#automatismoIdABorrar").val(idAutomatismo);
    modalConfirmarBorrar.showModal();
}

function EliminarAutomatismoNoGrano() {
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
function ModalModificarCallePlanta(item) {
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

function ModalModificarPuntoDeCarga(item) {
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
            $(".modal-title").text("Modificar Punto de Carga");
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
            bindearEventos();
        }
    });
}

function recargarListaCallePlanta() {
    $.ajax({
        url: urlListarCallePlanta,
        success: function (listaActualizada) {
            $("#gridCallePlanta").html(listaActualizada);
            bindearEventos();
        }
    });
}

function recargarListaPuntoDeCarga() {
    $.ajax({
        url: urlListarPuntoDeCarga,
        success: function (listaActualizada) {
            $("#gridPuntoDeCarga").html(listaActualizada);
            bindearEventos();
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

function dllCallePlantaOnChange() {
    var selectedCalleId = $("#ddlCallePlanta").val();
    if (selectedCalleId == "") {
        $('#ddlPuntosDeCarga').find('option').not(':first').remove();
    }
    else {
        $.ajax({
            url: urlObtenerPuntosDeCargaPorMaterialId,
            data: { calleId: selectedCalleId },
            success: function (puntosDeCarga) {
                let valorActual = $('#ddlPuntosDeCarga').val();
                $('#ddlPuntosDeCarga').find('option').not(':first').remove();
                var ddlPuntosDeCarga = $("#ddlPuntosDeCarga");

                $.each(puntosDeCarga, function (indice, puntoDeCarga) {
                    if (puntoDeCarga.Value == valorActual) {
                        ddlPuntosDeCarga.append("<option selected value='" + puntoDeCarga.Value + "'>" + puntoDeCarga.Text + "</option>");
                    } else {
                        ddlPuntosDeCarga.append("<option value='" + puntoDeCarga.Value + "'>" + puntoDeCarga.Text + "</option>");
                    }
                });

                obtenerAlmacenesPorCalleMaterial(selectedCalleId)
            }
        });
    }
}

function obtenerAlmacenesPorCalleMaterial(calleId) {
    $.ajax({
        url: urlObtenerAlmacenesCalleMaterialId,
        data: { calleId: calleId },
        success: function (almacenes) {
            let valorActual = $('#AutomatismoNoGrano_AlmacenId').val();
            let valorActualHidden = $('#AutomatismoNoGrano_AlmacenId_hdn').val();

            $('#AutomatismoNoGrano_AlmacenId').find('option').not(':first').remove();
            var cmbAlmacen = $("#AutomatismoNoGrano_AlmacenId");

            $.each(almacenes, function (indice, almacen) {
                if (almacen.Value == valorActual || almacen.Value == valorActualHidden) {
                    cmbAlmacen.append("<option selected value='" + almacen.Value + "'>" + almacen.Text + "</option>");
                } else {
                    cmbAlmacen.append("<option value='" + almacen.Value + "'>" + almacen.Text + "</option>");
                }
            });
        }
    });
}