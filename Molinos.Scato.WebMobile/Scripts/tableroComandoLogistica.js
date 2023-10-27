$(document).ready(function () {
    $("#nuevoAutomatismo").click(function () {
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: 'Cargando...'
        });

        $.ajax({
            url: urlCrearAutomatismo,
            success: function (result) {
                $(".modal-backdrop").remove();
                $(".modalAutomatismo").remove();
                $("#DetalleAutomatismo").html(result);
                $.getScript(urlFileJavaScript);
                $("#modalAutomatismo").modal("show");
            },
            error: function (error) {
            },
            complete: function () {
                $.unblockUI();
            }
        });
    });

    $("#deshabilitado").click(function () {
        MostrarAlertaError('No Posee los Permisos para realizar la Acción');
    });

    $('#automatismoGeneral').on("click", function (e) {
        let element = e.currentTarget;
        let valor = $(element).prop('checked');
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: 'Cargando...'
        });
        $.ajax({
            url: urlActualizarEstadoAutomatismoGeneral,
            data: { nuevoEstado: valor },
            success: function (response) {
                if (response.Mensajes[0].TipoDeMensaje === 0) {
                    element.checked = valor;
                } else {
                    MostrarAlertaError(response.Mensajes[0].Mensaje);
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

    $('#llamadoAPreBalanza').on("click", function (e) {
        let element = e.currentTarget;
        let valor = $(element).prop('checked');
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: 'Cargando...'
        });
        $.ajax({
            url: urlActualizarEstadoAutomatismoLlamadoPreBalanza,
            data: { nuevoEstado: valor },
            success: function (response) {
                if (response.Mensajes[0].TipoDeMensaje === 0) {
                    element.checked = valor;
                } else {
                    MostrarAlertaError(response.Mensajes[0].Mensaje);
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

    bindearChecksAutomatismo();

    $('.columna-checkbox').on("click", function (e) {
        var nombreCampo = $(this).prop('name');

        let element = e.currentTarget;
        element.checked = !element.checked;
        if (nombreCampo == 'EsEscalable') {
            ModificarEsEscalableHidraulica(element);
        }

        $("#modalAutomatismo").modal("hide");
        $("#modalModificarAutomatismo").modal("hide");
        $("#modalModificarPreBalanza").modal("hide");
        $("#modalModificarPreHidraulica").modal("hide");
    });

    $('body').on('change', '#AutomatismoGrano_CalidadId', function () {
        var seleccionado = $('#AutomatismoGrano_CalidadId').find(":selected")
        $("#CaracteristicaDeCalidadId").val(seleccionado.val())
        setearRangosCaracteristicasDeCalidad();
    })
});

var popupComandoLogistica;

function funcionModalModificarTableroComandoLogistica(idAutomatismo) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });

    $.ajax({
        url: urlModificarAutomatismo,
        data: { id: idAutomatismo },
        success: function (result) {
            $(".modal-backdrop").remove()
            $(".modalModificarAutomatismo").remove()
            $("#DetalleAutomatismo").html(result);
            $.getScript(urlFileJavaScript);
            $("#modalModificarAutomatismo").modal("show");
        },
        error: function (error) {
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function funcionModalEliminarTableroComandoLogistica(idAutomatismo) {
    var id = '<input id=automatismoId value=' + idAutomatismo + ' />'
    $("#automatismo").html(id);
    modalConfirmarBorrar.showModal();
}

function funcionEliminarAutomatismo() {
    var idAutomatismo = $("#automatismoId").val();
    $.ajax({
        url: urlEliminarAutomatismo,
        method: "POST",
        data: { id: idAutomatismo },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                MostrarAlertaExitosa(response.Mensajes[0].Mensaje);
                modalConfirmarBorrar.close()
                recargarListaAutomatismos();
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
            }
        }
    });
}

function recargarListaAutomatismos() {
    $.ajax({
        url: urlListarAutomatismoGrano,
        success: function (listaActualizada) {
            $("#listarAutomatismo").html(listaActualizada);
            bindearChecksAutomatismo();
        }
    });
}

function funcionModalModificarCallePreBalanza(idCalle) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlModificarCallePB,
        data: { id: idCalle },
        success: function (result) {
            $(".modal-backdrop").remove();
            $(".modalModificarPreBalanza").remove();
            $("#DetalleAutomatismo").html(result);
            $.getScript(urlFileJavaScript);
            $("#modalModificarPreBalanza").modal("show");
        },
        error: function (error) {
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function funcionModalModificarCallePreHidraulica(idCalle) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlModificarCallePH,
        data: { id: idCalle },
        success: function (result) {
            $(".modal-backdrop").remove();
            $(".modalModificarPreHidraulica").remove();
            $("#DetalleAutomatismo").html(result);
            $.getScript(urlFileJavaScript);
            $("#modalModificarPreHidraulica").modal("show");
        },
        error: function (error) {
        },
        complete: function () {
            $.unblockUI();
        }
    });
}

function guardarModificarPasoDirecto(idRegistro, valorActual) {
    $.ajax({
        url: urlActualizarEstadoPaseDirecto,
        type: 'POST',
        data: {
            id: idRegistro,
            valor: valorActual
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                MostrarAlertaExitosa(response.Mensaje);
            } else {
                MostrarAlertaError(response.Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición');
        }
    });
}

function guardarModificarLlamadoVolcable(idRegistro, valorActual) {
    $.ajax({
        url: urlActualizarEstadoLlamadoVolcable,
        type: 'POST',
        data: {
            id: idRegistro,
            valor: valorActual
        },
        success: function (response) {
            if (response.Exitoso) {
                MostrarAlertaExitosa(response.Mensaje);
            } else {
                MostrarAlertaError(response.Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición');
        }
    });
}

function ModificarPasoDirecto(element) {
    let id = $(element).data('id');
    let valor = $(element).prop('checked');
    popupComandoLogistica = element;
    if (!valor) {
        $.blockUI({
            blockMsgClass: 'blocuiBox',
            message: 'Cargando...'
        });
        $.ajax({
            url: urlVerificarLlamadoUnoAUno,
            type: 'POST',
            data: {
                id: id,
            },
            success: function (response) {
                if (response.Data.TipoDeMensaje === 0) {
                    ActualizarEstadoPaseDirecto(id, !valor, response.Data.Mensaje)
                }
            },
            error: function () {
                MostrarAlertaError('Error al realizar la petición.');
            },
            complete: function () {
                $.unblockUI();
            }
        });
    }
    else {
        $('#PasoDirecto').attr("data-id", id);
        $('#PasoDirecto').attr("data-value", !valor);

        modalConfirmarPaseDirecto.showModal();
    }
}

function ModificarLlamadoVolcable(element) {
    cambiarEstadoSwitch(urlActualizarEstadoLlamadoVolcable, element)
}

function ModificarEstadoPreBalanza(element) {
    cambiarEstadoSwitch(urlModificarEstadoCallePB, element)
}

function ModificarEstadoPreHidraulica(element) {
    cambiarEstadoSwitch(urlModificarEstadoCallePH, element)
}

function ModificarEstadoHidraulica(element) {
    cambiarEstadoSwitch(urlModificarEstadoHidraulica, element)
}

function ModificarEstadoHidraulicaConfirmacion() {
    var idRegistro = $('#modalConfirmarEstadoHidraulica').data('id');
    var valorActual = $('#modalConfirmarEstadoHidraulica').data('value');
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlModificarEstadoHidraulica,
        type: 'POST',
        data: {
            id: idRegistro,
            valor: valorActual
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                modalConfirmarEstadoHidraulica.close();
                popupComandoLogistica.checked = !popupComandoLogistica.checked;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
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

function ValidarEstadoHidraulica(element) {
    let id = $(element).data('id');
    let valor = $(element).prop('checked');
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlValidarEstadoHidraulica,
        type: 'POST',
        data: {
            id: id
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                $('#modalConfirmarEstadoHidraulica').attr("data-id", id);
                $('#modalConfirmarEstadoHidraulica').attr("data-value", !valor);
                popupComandoLogistica = element;
                modalConfirmarEstadoHidraulica.showModal();
            } else {
                ModificarEstadoHidraulica(element)
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

function ModificarEsEscalableHidraulica(element) {
    let id = $(element).data('id');
    let valor = $(element).prop('checked');
    $.ajax({
        url: urlModificarEstadoEsEscalableHidraulica,
        type: 'POST',
        data: {
            id: id,
            valor: !valor
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                element.checked = !valor;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición');
        }
    });
}

function ProcesarRespuestaToAlert(response, selector) {
    if (response) {
        if (response.Mensajes && response.Mensajes.length > 0) {
            const mensaje = response.Mensajes[0];

            if (mensaje.TipoDeMensaje === 2) {
                MostrarAlertaError(mensaje.Mensaje);
            } else if (mensaje.TipoDeMensaje === 1) {
                MostrarAlertaAdvertencia(mensaje.Mensaje);
            } else {
                MostrarAlertaExitosa(mensaje.Mensaje);
                $(selector).modal("hide");
            }
        }
    }
}

function ActualizarEstadoPaseDirecto(idRegistro, valorActual, validacion) {
    if (validacion) {
        $('#modalConfirmarPaseDirectoHabilitado').attr("data-id", idRegistro);
        $('#modalConfirmarPaseDirectoHabilitado').attr("data-value", valorActual);
        modalConfirmarPaseDirectoHabilitado.showModal();
    } else {
        ActualizarEstadoPaseDirectoHabilitado(idRegistro, valorActual)
    }
}

function ActualizarEstadoPaseDirectoHabilitado(idRegistro, valorActual) {
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlActualizarEstadoPaseDirecto,
        type: 'POST',
        data: {
            id: idRegistro,
            valor: valorActual
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                popupComandoLogistica.checked = !popupComandoLogistica.checked;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición.');
        },
        complete: function () {
            $.unblockUI();
            popupComandoLogistica = null;
        }
    });
}

function ActualizarEstadoPaseDirectoPorFila() {
    let id = $(popupComandoLogistica).data('id');
    let valor = $(popupComandoLogistica).prop('checked');
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlActualizarEstadoPaseDirecto,
        type: 'POST',
        data: {
            id: id,
            valor: !valor
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                modalConfirmarPaseDirecto.close()
                popupComandoLogistica.checked = !popupComandoLogistica.checked;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición.');
        },
        complete: function () {
            $.unblockUI();
            popupComandoLogistica = null;
        }
    });
}

function ActualizarEstadoPaseDirectoUnoAUno() {
    let id = $(popupComandoLogistica).data('id');
    let valor = $(popupComandoLogistica).prop('checked');
    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlActualizarEstadoPaseDirectoUnoAUno,
        type: 'POST',
        data: {
            id: id,
            valor: !valor
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                modalConfirmarPaseDirecto.close()
                popupComandoLogistica.checked = !popupComandoLogistica.checked;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición.');
        },
        complete: function () {
            $.unblockUI();
            popupComandoLogistica = null;
        }
    });
}

function ActualizarEstadoPaseDirectoUnoAUnoHabilitado() {
    let id = $(popupComandoLogistica).data('id');

    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: urlActualizarEstadoPaseDirectoUnoAUno,
        type: 'POST',
        data: {
            id: id,
            valor: true
        },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                modalConfirmarPaseDirectoHabilitado.close()
                popupComandoLogistica.checked = !popupComandoLogistica.checked;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
            }
        },
        error: function () {
            MostrarAlertaError('Error al realizar la petición.');
        },
        complete: function () {
            $.unblockUI();
            popupComandoLogistica = null;
        }
    });
}

function cambiarEstadoSwitch(url, elemento) {
    let id = $(elemento).data('id');
    let valor = $(elemento).prop('checked');

    $.blockUI({
        blockMsgClass: 'blocuiBox',
        message: 'Cargando...'
    });
    $.ajax({
        url: url,
        data: { valor: !valor, id: id },
        success: function (response) {
            if (response.Mensajes[0].TipoDeMensaje === 0) {
                elemento.checked = !valor;
            } else {
                MostrarAlertaError(response.Mensajes[0].Mensaje);
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

function bindearChecksAutomatismo() {
    $(".cambio-tipo").on("click", function (e) {
        var nombreCampo = $(this).data('field');
        let element = e.currentTarget;
        element.checked = !element.checked;
        if (nombreCampo == 'EsPasoDirecto') {
            ModificarPasoDirecto(element);
        } else if (nombreCampo == 'Activo') {
            ModificarLlamadoVolcable(element);
        } else if (nombreCampo == 'ActivoAutomaticoPB') {
            ModificarEstadoPreBalanza(element);
        } else if (nombreCampo == 'ActivoAutomaticoPH') {
            ModificarEstadoPreHidraulica(element);
        } else if (nombreCampo == 'ActivoAutomaticoH') {
            if (!element.checked === false) {
                ValidarEstadoHidraulica(element);
            }
            else {
                ModificarEstadoHidraulica(element);
            }
        }
    });
}

function setearRangosCaracteristicasDeCalidad() {
    var caracteristicaDeCalidadId = $('#AutomatismoGrano_CalidadId').find(":selected").val();
    if (!caracteristicaDeCalidadId) return;

    getCaracteristicaDeCalidad()
        .done(function (resultado) {
            $('#AutomatismoGrano_Minimo').val(resultado.caladoMinimo)
            $('#AutomatismoGrano_Maximo').val(resultado.caladoMaximo)
        });
}

function getCaracteristicaDeCalidad() {
    var caracteristicaDeCalidadId = $('#AutomatismoGrano_CalidadId').find(":selected").val();
    const url = $('#links').data().urlBuscarCaracteristicadecalidadporid.trim();
    return $.getJSON(url, { id: caracteristicaDeCalidadId });
}