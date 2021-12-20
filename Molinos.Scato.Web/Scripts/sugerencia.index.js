$(document).ready(function () {
    var object;

    $(document).on('click', '#panelSugerencia', (function () {
        object = $(this);
       
        $('#dialogo-sugerencia').modal({
            backdrop: 'static', keyboard: false
        }).css({
            width: function () {
                return $('#dialogo-sugerencia').outerWidth();
            }, 'margin-left': function () {
                return -($(this).width() / 2);
            },
            'top': '50%',
            'margin-top': function () {
                return -($(this).height() / 2);
            }
        });
        return false;
    }));

    $('#boton-enviar').on('click', (function (e) {
        var deleteLinkObj = $("#diag-sugerencia").data().enviarUrl;
        BlockUI($("#gridContainer").data().mensajeEspera);
        $.post(deleteLinkObj, { textoSugerencia: $('#TextoSugerencia').val() }, function (data) {
            if (data != null) {
                MostrarAlertaExitosa();
                $('#dialogo-sugerencia').modal('hide');
                $('#TextoSugerencia').val = "";
            } else {
                MostrarAlertaError(data);
                $('#dialogo-sugerencia').modal('hide');
            }
        }).complete(function () {
            $.unblockUI();
        });
        e.preventDefault();
        e.stopPropagation();
    }));
 
});

$(function () {
    var contador;
    var maximo = 4000;
    var valor = 0;
    $("#contador")[0].innerText = caracterText + ' ' + maximo;

    $('#TextoSugerencia').keyup(function () {
        valor = $('#TextoSugerencia').val().length;
        contador = maximo;
        contador = contador - valor;
        $("#contador")[0].innerText = caracterText + ' ' + contador;
    });
});