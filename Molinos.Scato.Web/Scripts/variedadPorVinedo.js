$(document).ready(function () { 
    var elementoId = "#vinedoIdFiltro";

    DefinirAutocompletar('#vinedoFiltro', elementoId, $('#links').data().urlBuscarVinedos, $('#links').data().urlBuscarVinedo, CargarVariedadesPorVinedo,AlInvalidar);
    
    $(document).on('click', '.ajax-editar-variedadvinedo-link', function () {
        $.get(this.href + "?vinedoId=" + $(elementoId).val(), cargarDialogoEditar2);
        return false;
    });
});


function CargarVariedadesPorVinedo(vinedo) {
    BlockUI($("#vinedoFiltro").data().buscando);
    $.get($('#ConsultarVinedos').val(), { vinedoId: vinedo || $("#vinedoIdFiltro").val() }, function (data) {
        $('#gridContainer').html(data);
        $("#botonVariedadPorVinedoNuevo").removeAttr("disabled");
        $.unblockUI();
        $('.ajax-editar-link').each(function () {
            $(this).addClass('ajax-editar-variedadvinedo-link');
            $(this).removeClass('ajax-editar-link');
        });
    });
}

function AlInvalidar() {
    $.get($('#ConsultarVinedos').val(), { vinedoId: 0 }, function (data) {
        $('#gridContainer').html(data);
        $("#botonVariedadPorVinedoNuevo").attr("disabled", "disabled");
        $.unblockUI();
        $('.ajax-editar-link').each(function () {
            $(this).addClass('ajax-editar-variedadvinedo-link');
            $(this).removeClass('ajax-editar-link');
        });
    });
}


function editarRepuestaFormulario2(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        if ($('#search-form').length == 0) {
            window.location.href = window.location.href;
        } else {
            $('#dialogo-editar').modal('hide');
            CargarVariedadesPorVinedo($("#vinedoIdFiltro").val());
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