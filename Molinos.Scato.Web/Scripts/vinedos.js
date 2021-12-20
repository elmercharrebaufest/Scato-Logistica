jQuery(document).ready(function ($) {
    var elementoId = "#fincaIdFiltro";
    
    DefinirAutocompletar('#fincaFiltro', elementoId, $('#links').data().urlBuscarMateriales, $('#links').data().urlBuscarMaterial, CargarCalidades);
    
    $(document).on('click', '.ajax-editar-custom-link', function () {
        $.get(this.href + "?fincaId=" + $(elementoId).val(), cargarDialogoEditar2);
        return false;
    });
});

function Cargar(material) {
    $.get($('#ConsultarFincas').val(), { materialId: material || $("#fincaIdFiltro").val() }, function(data) {
        $('#gridContainer').html(data);
    });
}

function editarRepuestaFormulario2(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        if ($('#search-form').length == 0) {
            window.location.href = window.location.href;
        } else {
            $('#dialogo-editar').modal('hide');
            Cargar($("#FincaId").val());
            MostrarAlertaExitosa();
        }
    } else {
        cargarDialogoEditar2(respuesta);
    }
}

function cargarDialogoEditar2(data) {
    $('#dialogo-editar-body').html(data);
    $("#dialogo-editar-guardar").attr("disabled", false);
    $('#dialogo-editar-title').html($('#dialogo-editar-body form').data().dialogoTitulo);
    $('#dialogo-editar-body form').attr('data-ajax-success', 'editarRepuestaFormulario2');
    if ($('#dialogo-editar-body form').data().dialogoExtraclass) {
        $('#dialogo-editar').addClass($('#dialogo-editar-body form').data().dialogoExtraclass);
    }

    $('#dialogo-editar').modal({
        backdrop: 'static', keyboard: false
    }).css({
        'top': '50%',
        'margin-left': function () {
            return -($(this).width() / 2);
        },
        'left': '50%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }
    });
    attachDataPickers();
}