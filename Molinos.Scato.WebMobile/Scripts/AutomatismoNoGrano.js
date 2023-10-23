//FUNCIONES PRINCIPALES ABM
//Automatismo

var TipoMensaje = {
    Success: 0,
    Warning: 1,
    Error: 2
}
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
            $(".modal-title").text("Modal Crear");
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

function funcionModalModificar(item) {
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

function funcionModalEliminar(item) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        type: "GET",
        url: urlEliminarAutomatismo,
        contentType: "application/json; charset=utf-8",
        data: { id: item },
        success: function (response) {
            $("#partialModal").find(".modal-body").html(response);
            $("#partialModal").modal('show');
            $(".modal-title").text("Modal Eliminar");
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
            $(".modal-title").text("Modal Modificar Calle Planta");
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

$('#body').on("click", ".cambia-estado", function (e) {
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

function dlCallePlantaOnChange(e) {
    var selectedCalleId = e;

    $.ajax({
        url: urlObtenerPuntosDeCargaPorMaterialId,
        data: { calleId: selectedCalleId },
        success: function (puntosDeCarga) {

            $('#ddlPuntosDeCarga')
                .find('option')
                .remove();
            var ddlPuntosDeCarga = $("#ddlPuntosDeCarga");

            $.each(puntosDeCarga, function (indice, puntoDeCarga) {
                ddlPuntosDeCarga.append("<option value='" + puntoDeCarga.Value + "'>" + puntoDeCarga.Text + "</option>");
            });

        }
    });

}

//FUNCIONES DE RESPUESTA
var fnResponse = function (response) {
    if (response.Key != undefined && response.TipoDeMensaje != TipoMensaje.Success) {
        MostrarAlertaError(response.Mensaje);
        switch (response.Key) {
            case 'Automatismo':
                recargarListaAutomatismos();
                break;
            case 'CallePlanta':
                recargarListaCallePlanta();
                break;
            case 'PuntoDeCarga':
                recargarListaPuntoDeCarga();
                break;
        }
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