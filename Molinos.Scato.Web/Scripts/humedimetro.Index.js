$(document).ready(function() {

    $('#dialogo-editar-modalidad-cancelar').click(function () {
        $('#dialogo-editar-modalidad').modal('hide');
        $('#dialogo-editar-guardar').removeAttr('disabled');
    });
    
    $('#dialogo-editar-modalidad-guardar').click(function () {
        $('#dialogo-editar-modalidad form').submit();
        if ($('#dialogo-editar-modalidad form').valid()) {
            $("#dialogo-editar-modalidad-guardar").attr("disabled", true);
            $('#dialogo-editar-guardar').removeAttr('disabled');
            $('.modalidad-editar').val($('#modalidad-editada option:selected').val());
        }
    });

    $(document).on('click', '.ajax-editar-modalidad-link', function () {
        $.get(this.href, cargarDialogoEditarModalidad);
        return false;
    });
    
    $('#dialogo-editar-modalidad').on('show', function () {
        $(this).find('.modal-body').css({
            height: 'auto', 'max-height': '400px', 'padding-right': '50px'
        });
    });
    
    $('#dialogo-editar-modalidad').on('shown', function () {
        $(this).find('.modal-body').find(':input:enabled:visible:first').focus();
    });
    
    $(document).keyup(function(e) {
        if (e.keyCode == 27) {  // escape
            $('#dialogo-editar-modalidad').modal('hide');
            $('#dialogo-editar-guardar').removeAttr('disabled');
        }
    });

});

function cargarDialogoEditarModalidad(data) {
    $('#dialogo-editar-modalidad-body').html(data);
    $("#dialogo-editar-modalidad-guardar").attr("disabled", false);
    $('#dialogo-editar-modalidad-title').html($('#dialogo-editar-modalidad-body form').data().dialogoTitulo);
    $('#dialogo-editar-modalidad-body form').attr('data-ajax-success', 'editarRepuestaFormularioModalidad');
    if ($('#dialogo-editar-modalidad-body form').data().dialogoExtraclass) {
        $('#dialogo-modalidad-editar').addClass($('#dialogo-editar-modalidad-body form').data().dialogoExtraclass);
    }

    $('#dialogo-editar-modalidad').modal({
        backdrop: 'static', keyboard: false
    }).css({

        'top': '45%',
        'margin-top': function () {
            return -($(this).height() / 2);
        }

    });
    attachDataPickers();
}

function editarRepuestaFormularioModalidad(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        window.location.href = window.location.href;
    } else {
        cargarDialogoEditarModalidad(respuesta);
    }
}

function editarRepuestaFormularioModalidad(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        if ($('#search-form').length == 0) {
            window.location.href = window.location.href;
        } else {
            $('#dialogo-editar-modalidad').modal('hide');
            $('#search-form').submit();
            MostrarAlertaExitosa();
        }
    } else {
        cargarDialogoEditarModalidad(respuesta);
    }
}