$(document).ready(function () {

    if ($("#AutomatismoGrano_AplicaFiltroCalidad").is(":checked")) {
        $("#divCalidad").show();
    }
    $("#AutomatismoGrano_AplicaFiltroCalidad").change(function () {
        if (this.checked) {
            $("#divCalidad").show();
        }
        else {
            $("#divCalidad").hide();
            $("#AutomatismoGrano_Minimo").val("");
            $("#AutomatismoGrano_Maximo").val("");
            $("#AutomatismoGrano_CalidadId").val(0);
        }
    });

    $('.multiselect').multiselect({
        includeSelectAllOption: true,
        buttonWidth: '100%'
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

        if ($('#AutomatismoGrano_AplicaFiltroCalidad').val()) {
            obtenerCalidadPorMaterial(materialId)
        }

        obtenerAlmacenesPorMaterial(materialId);
    });

    $('.esEscalableCheck').change(function () {
        var id = $("#AutomatismoGrano_Id").val()
        $.ajax({
            url: urlObtenerHidraulicasEscalables,
            data: {
                id: id,
                valor: this.checked
            },
            success: function (response) {
                $("#AutomatismoGrano_Hidraulicas").multiselect('dataprovider', response);
            },
            error: function () {
                MostrarAlertaError('Error al realizar la petición.');
            }
        });
    });
});

function guardarAutomatismo(idRegistro, valorActual) {
    $.ajax({
        url: urlActualizarEstadoLlamadoVolcable,
        type: 'POST',
        data: {
            id: idRegistro,
            valor: valorActual
        },
        success: function (response) {
            if (!response.success) {
                alert('Error al actualizar el registro.');
            }
        },
        error: function () {
            alert('Error al realizar la petición.');
        }
    });
}

function ModificarAutomatismo(idRegistro, valorActual) {
    $.ajax({
        url: urlActualizarEstadoLlamadoVolcable,
        type: 'POST',
        data: {
            id: idRegistro,
            valor: valorActual
        },
        success: function (response) {
            if (!response.success) {
                alert('Error al actualizar el registro.');
            }
        },
        error: function () {
            alert('Error al realizar la petición.');
        }
    });
}

function AutomatismoVM() {
    this.materialId = ko.observable();
    this.variedadId = ko.observable();
}

function guardarSuccess(response) {
    if (response.Mensajes[0].TipoDeMensaje === 0 || response.Mensajes[0].TipoDeMensaje === 2) {
        recargarListaAutomatismos();
    }
    ProcesarRespuestaToAlert(response, "#modalAutomatismo");
}

function modificarSuccess(response) {
    if (response.Mensajes[0].TipoDeMensaje === 0) {
        recargarListaAutomatismos();
    }
    ProcesarRespuestaToAlert(response, "#modalModificarAutomatismo");
}

function obtenerCalidadPorMaterial(materialId) {
    $.ajax({
        url: urlObtenerCalidad,
        data: { id: materialId },
        success: function (calidades) {
            $("#AutomatismoGrano_CalidadId").empty();
            $.each(calidades, function (indice, calidad) {
                $("#AutomatismoGrano_CalidadId").append("<option value='" + calidad.Value + "'>" + calidad.Text + "</option>");
            });
        }
    });
}

function obtenerAlmacenesPorMaterial(materialId) {
    $.ajax({
        url: urlObtenerAlmacenes,
        data: { materialId: materialId },
        success: function (almacenes) {
            $("#cboxAlmacenes").empty();
            $.each(almacenes, function (indice, almacen) {
                $("#cboxAlmacenes").append("<option value='" + almacen.Value + "'>" + almacen.Text + "</option>");
            });
        }
    });
}