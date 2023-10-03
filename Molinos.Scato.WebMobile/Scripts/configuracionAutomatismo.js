$(document).ready(function () {
    $("#AutomatismoGrano_AplicaFiltroCalidad").change(function () {
        if (this.checked) {
            $("#divCalidad").show();
            $("#AutomatismoGrano_CalidadId").prop('required', true);
        }
        else {
            $("#divCalidad").hide();
            $("#AutomatismoGrano_CalidadId").prop('required', false);
        }
    });

    // Cuando cambie el combobox de materiales
    $("#cboxMateriales").change(function () {
        var materialId = $(this).val();
        var url = $(this).data("url");

        $.ajax({
            url: url,
            data: { materialId: materialId },
            success: function (variedades) {
                $("#cboxVariedades").empty();
                $.each(variedades, function (indice, variedad) {
                    $("#cboxVariedades").append("<option value='" + variedad.Value + "'>" + variedad.Text + "</option>");
                });
            }
        });
    });

    $('.esEscalableCheck').change(function () {
        $.ajax({
            url: urlModificarEstadoEsEscalableHidraulica,
            data: { newValue: this.checked },
            success: function (response) {
                if (response.Mensajes[0].TipoDeMensaje === 0) {
                    MostrarAlertaExitosa(response.Mensajes[0].Mensaje);
                } else {
                    MostrarAlertaError(response.Mensajes[0].Mensaje);
                }
            },
            error: function () {
                MostrarAlertaError('Error al realizar la petición');
            }
        });
    });
});

function AutomatismoVM() {
    this.materialId = ko.observable();
    this.variedadId = ko.observable();
}

function guardarSuccess(response) {
    if (response.Mensajes[0].TipoDeMensaje === 0) {
        recargarListaAutomatismos();
    }
    ProcesarRespuestaToAlert(response, "#modalAutomatismo");
}

function modificarCallePBSuccess(response) {
    if (response.Mensajes[0].TipoDeMensaje === 0) {
        location.reload(true);
    }
    ProcesarRespuestaToAlert(response, "#modalModificarPreBalanza");
}

function modificarCallePHSuccess(response) {
    if (response.Mensajes[0].TipoDeMensaje === 0) {
        location.reload(true);
    }
    ProcesarRespuestaToAlert(response, "#modalModificarPreHidraulica");
}