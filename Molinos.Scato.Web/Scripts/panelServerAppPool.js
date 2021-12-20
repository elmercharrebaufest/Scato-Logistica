jQuery(document).ready(function ($) {

    $('#Tiempo').keyup(function () {
        if (!$.isNumeric($('#Tiempo').val())) {
            $('#Tiempo').val('');
        }
        if ($('#Tiempo').val().length > 10) {
            var a = $('#Tiempo').val().substring(0, 10);
            $('#Tiempo').val(a);
        }
    });

    $('#EventoId').keyup(function () {
        if (!$.isNumeric($('#EventoId').val())) {
            $('#EventoId').val('');
        }
        if ($('#EventoId').val().length > 10) {
            var a = $('#EventoId').val().substring(0, 10);
            $('#EventoId').val(a);
        }
    });
    formatoFechas();
    jQuery(document).on('click', '.iniciar', function () {
        BlockUI();
        $.ajax({
            url: $(this).attr('href'),
            dataType: "json",
            type: "GET",
            error: function () {
                MostrarAlertaError("Ocurrió un error al procesar los datos del server");
            },
            success: function (data) {
                MostrarAlertaInfo(data);
            }
        }).always(function () {
            $.unblockUI();
            $('#buscar').click();
        });
        return false;
    });
    jQuery(document).on('click', '.detener', function () {
        BlockUI();
        $.ajax({
            url: $(this).attr('href'),
            dataType: "json",
            type: "GET",
            error: function () {
                MostrarAlertaError("Ocurrió un error al procesar los datos del server");
            },
            success: function (data) {
                MostrarAlertaInfo(data);
            }
        }).always(function () {
            $.unblockUI();
            $('#buscar').click();
        });
        return false;
    });

    jQuery(document).on('click', '.reciclar', function () {
        BlockUI();
        $.ajax({
            url: $(this).attr('href'),
            dataType: "json",
            type: "GET",
            error: function () {
                MostrarAlertaError("Ocurrió un error al procesar los datos del server");
            },
            success: function (data) {
                MostrarAlertaInfo(data);
            }
        }).always(function () {
            $.unblockUI();
            $('#buscar').click();
        });
        return false;
    });
    jQuery(document).on('click', '.configurar', function () {
        $('#url').val($(this).attr('href'));
        cargarDialogoConfig();
        return false;
    });

    $('#dialogo-config-guardar').click(function () {
        recyclingTimeAppPool($('#Tiempo').val());
        $('#dialogo-config-recycling').modal('hide');
        return false;
    });

    $('#dialogo-config-cancelar').click(function () {
        $('#dialogo-config-recycling').modal('hide');
        return false;
    });

    validarCamposDialogoLogs();
    $('#dialogo-obtener-eventos-ver').click(function () {
        if ($('#FechaDesde').val() != "" && $('#FechaHasta').val() != "" && $('#EventoId').val() != "") {
            cargarLogs();
        } else {
            alert('Revise los datos ingresados');
        }
    });

    $('#dialogo-obtener-eventos-cancelar').click(function () {
        ocultarDialogoLogs();
    });

    $('#close-dialogo-logs').click(function () {
        ocultarDialogoLogs();
    });

    $('#verLogs').click(function () {
        cargarDialogoLogs();
    });

    $('#DescargarLog').click(function () {
        ObtenerLogs();
    });
});

function validarCamposDialogoLogs() {
    $('#EventoId').blur(function () {
        if ($('#EventoId').val() == "" && ($('#EventoIdError').html() == "" || $('#EventoIdError').html() == undefined)) {
            $('#EventoId').after('<div class="field-validation-error" id="EventoIdError" >El campo Log Id es requerido</div>');
        } else if ($('#EventoId').val() != "") {
            $('#EventoIdError').remove();
        }
    });
    $('#FechaDesde').blur(function () {
        if ($('#FechaDesde').val() == "" && ($('#FechaDesdeError').html() == "" || $('#FechaDesdeError').html() == undefined)) {
            $('#FechaDesde').after('<div class="field-validation-error" id="FechaDesdeError" >El campo Fecha Desde es requerido</div>');
        } else if ($('#FechaDesde').val() != "") {
            $('#FechaDesdeError').remove();
        }
    });
    $('#FechaHasta').blur(function () {
        if ($('#FechaHasta').val() == "" && ($('#FechaHastaError').html() == "" || $('#FechaHastaError').html() == undefined)) {
            $('#FechaHasta').after('<div class="field-validation-error" id="FechaHastaError" >El campo Fecha Hasta es requerido</div>');
        } else if ($('#FechaHasta').val() != "") {
            $('#FechaHastaError').remove();
        }
    });
}

function formatoFechas() {
    var formatoFecha = Globalize.culture().calendars.standard.patterns.d.replace(/[a-z]/g, '9');
    formatoFecha = formatoFecha.replace(/[A-Z]/g, '9');

    var formatoHora = Globalize.culture().calendar.patterns.t.replace(/[a-su-zA-SU-Z]/g, '9');
    formatoHora = formatoHora.replace(/[t]/g, '');
    formatoHora = formatoHora.replace(formatoHora, '?' + formatoHora);

    $('#FechaDesde').mask(formatoFecha + " " + formatoHora);
    $('#FechaHasta').mask(formatoFecha + " " + formatoHora);

    $('#FechaDesde').on('change', function () {
        var fDesde = $('#FechaDesde').val();
        if ((fDesde.length - 1) == formatoFecha.length) {
            $('#FechaDesde').val($('#FechaDesde').val() + '00:00');
        }
    });

    $.validator.addMethod("date", function () {
        return Globalize.parseDate($('#FechaDesde').val()) <= Globalize.parseDate($('#FechaHasta').val()) || $('#FechaHasta').val() == "" || $('#FechaDesde').val() == "";
    }, $('#FechaHasta').data().secondDateValidation);

    $('#FechaHasta').on('change', function () {
        var fDesde = $('#FechaHasta').val();
        if ((fDesde.length - 1) == formatoFecha.length) {
            $('#FechaHasta').val($('#FechaHasta').val() + '00:00');
        }
    });
}

function cargarDialogoConfig() {
    setTimeout(function () { $('#Tiempo').focus(); }, 800);

    $('#dialogo-config-recycling').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-config-recycling').outerWidth();
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
}

function ocultarDialogoLogs() {
    $('#dialogo-obtener-eventos').modal('hide');
    $('#listaLogs').html("");
    $('#FechaHasta').val("");
    $('#FechaDesde').val("");
    $('#EventoId').val("");
}

function cargarDialogoLogs() {
    setTimeout(function () { $('#EventoId').focus(); }, 800);

    $('#dialogo-obtener-eventos').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-obtener-eventos').outerWidth(1000);
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        height: '500px',
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
}

function cargarLogs() {
    var url = $(links).data().urlObtenerEventos;
    BlockUI();
    $.ajax({
        url: url,
        data: { eventoId: $('#EventoId').val(), fechaDesde: $('#FechaDesde').val(), fechaHasta: $('#FechaHasta').val(), servidorNombre: $('#serversLista option:selected').val() },
        type: "POST",
        error: function (data) {
            alert("Ocurrió un error al procesar los datos del server" + data);
        },
        success: function (data) {
            $('#listaLogs').html(data);
        }
    }).always(function () {
        $.unblockUI();
    });
};

function recyclingTimeAppPool(tiempoReciclado) {
    BlockUI();
    $.ajax({
        url: $('#url').val(),
        data: {
            tiempoReciclado: tiempoReciclado,
        },
        datatype: "json",
        type: "Post",
        error: function (data) {
            MostrarAlertaError(data);
        },
        success: function (data) {
            MostrarAlertaExitosa(data);
        }
    }).always(function () {
        $.unblockUI();
        $('#buscar').click();
    });
}

function ObtenerLogs() {
    setTimeout(function () { $('#EventoId').focus(); }, 800);

    $('#dialogo-descargar-logs').modal({
        backdrop: 'static', keyboard: false
    }).css({
        width: function () {
            return $('#dialogo-descargar-logs').outerWidth(1000);
        }, 'margin-left': function () {
            return -($(this).width() / 2);
        },
        height: '500px',
        'top': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    $.getJSON($('#dialogo-descargar-logs').data().urlObtenerLogs, function (data) {
        $.each(data, function (key, val) {
            var row = ('<tr>' + '<th>' + val + '</th>' + '<th>' + '<a id="dialogo-obtener-logs-ver" href="PanelServerAppPool\\DescargarLogs?archivo=' + val + '"  target= "_blank" type="button" class="btn btn-primary boton-descargar" tabindex="2">Descargar</a>' + '</th>' + '</tr>');
            $("#archivos").append(row);
        });
        VerificarLog();
    });

    function VerificarLog() {
        setInterval(function () {
            if ($.cookie('RetornoArchivo') != null && $.cookie('RetornoArchivo') != "ok") {
                MostrarAlertaError("Erorr al descargar el Log");
                $.removeCookie('RetornoArchivo', { path: '/' });
            }
        }, 1000);
    }
}