$(document).ready(function () {
    $('#dialogo-editar').show(function () {
        if ($('#Codigo').val() != "") {
            $('#Codigo').attr('readonly', true);
        } else {
            $('#Codigo').attr('readonly', false);
        }
    });
    mostrarBloque();
    if ($('#MaterialId').val()) {
        crearOpcionesCaracteristicasDeCalidad($('#MaterialId').val());
    } else {
        $('#MaterialId').val(0);
        $('#MaterialDesc').val('');
    }

    $('#TipoCalle').change(reiniciarVistayValores);
    $('#TipoCalidad').change(mostrarBloque);


    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, manejarSeleccionDeMaterial, null, { tipoCalle: $('#TipoCalle').val() });

    $("#MaterialDesc").autocomplete("option", "appendTo", "#dialogo-editar");

    $('#CaracteristicaDeCalidadDesc').change(function (res) {
        var seleccionado = $('#CaracteristicaDeCalidadDesc').find(":selected")
        $("#CaracteristicaDeCalidadId").val(seleccionado.val())
        setearRangosCaracteristicasDeCalidad();
    });
});

function manejarSeleccionDeMaterial() {
    crearOpcionesCaracteristicasDeCalidad($('#MaterialId').val());
    $('#RangoCaracteristicaCalidadMinimo').val('');
    $('#RangoCaracteristicaCalidadMaximo').val('');
    $('#CaracteristicaDeCalidadId').val('');
    $('#CaracteristicaDeCalidadDesc').val('');
}

function crearOpcionesCaracteristicasDeCalidad(materialId) {
        $('#CaracteristicaDeCalidadDesc').prop('disabled', true);

        if (materialId != null || materialId != '') {
            const url = $('#links').data().urlBuscarCaracteristicasdecalidad;

            BlockUI($("#BuscandoActividadesMensaje").val());
            $.getJSON(
                url,
                { materialId: materialId },
                function (resultado) {
                    $('#CaracteristicaDeCalidadDesc').prop('disabled', false);
                    $('#CaracteristicaDeCalidadDesc').each(function () {
                        var value = $(this).val();

                        var combo = $(this);
                        combo.empty();

                        $.each(resultado, function (index, data) {
                            combo.append($('<option/>', {
                                value: data.Id,
                                text: data.label,
                                selected: data.id == value
                            }));
                        });
                        combo.val(value);
                    });
                }).complete(function () {
                    $.unblockUI();
                });
        }
};

function reiniciarVistayValores() {    
    $('#MaterialDesc').val('');
    $('#MaterialId').val(0);
    $('#CaracteristicaDeCalidadDesc').val('');
    $('#CaracteristicaDeCalidadId').val('');
    $('#TipoCalidad').val(0);

    $('#RangoCaracteristicaCalidadMinimo').val('');
    $('#RangoCaracteristicaCalidadMaximo').val('');

    DefinirAutocompletar('#MaterialDesc', '#MaterialId', $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, manejarSeleccionDeMaterial, null, { tipoCalle: $('#TipoCalle').val() });
    mostrarBloque()
};

function getCaracteristicaDeCalidad() {
    var caracteristicaDeCalidadId = $('#CaracteristicaDeCalidadDesc').find(":selected").val(); 
    const url = $('#links').data().urlBuscarCaracteristicadecalidadporid.trim();
    return $.getJSON(url, { id: caracteristicaDeCalidadId });
}

function mostrarBloque() {
    if ($('#TipoCalle').val() == "PreCalado" || $('#TipoCalle').val() == "Circular" || $('#TipoCalle').val() == "NoGranos" || $('#TipoCalle').val() == "PlantaNoGranos" || $('#TipoCalle').val() == "PreBalanzaGranos") { // pre calado
        $('.material-group').css('display', '');
        $('.calidad-group').css('display', 'none');
        $('.caracteristicas-calidad-group').css('display', 'none');
        if($('#TipoCalle').val() == "NoGranos" || $('#TipoCalle').val() == "PlantaNoGranos" || $('#TipoCalle').val() == "PreBalanzaGranos") {
            $('#MaterialDesc').prop('required', true);
        }
    }
    else if ($('#TipoCalle').val() == "PostCalado") { //Post calado
        $('.calidad-group').css('display', '');
        if ($('#TipoCalidad').val() == "Otros") {
            $('.material-group').css('display', '');
            $('.caracteristicas-calidad-group').css('display', '');

        } else {
            $('.material-group').css('display', 'none');
            $('.caracteristicas-calidad-group').css('display', 'none');
            $('#MaterialDesc').val('');
            $('#MaterialId').val(0);
            $('#CaracteristicaDeCalidadDesc').val('');
            $('#CaracteristicaDeCalidadId').val('');
            $('#RangoCaracteristicaCalidadMinimo').val('');
            $('#RangoCaracteristicaCalidadMaximo').val('');

        }

    } else {
        $('.material-group').css('display', 'none');
        $('.caracteristicas-calidad-group').css('display', 'none');
    }
}

function setearRangosCaracteristicasDeCalidad() {
    var caracteristicaDeCalidadId = $('#CaracteristicaDeCalidadDesc').find(":selected").val();
    if (!caracteristicaDeCalidadId) return;

    getCaracteristicaDeCalidad()
        .done(function (resultado) {
            $('#RangoCaracteristicaCalidadMinimo').val(resultado.caladoMinimo)
            $('#RangoCaracteristicaCalidadMaximo').val(resultado.caladoMaximo)
    });
}