$(document).ready(function () {
    var object;
    var idImpresoraSeleccionada;
    $(document).on('click', '.ajax-imprimir-link', (function () {
        object = $(this);
        idImpresoraSeleccionada = object[0].href.split("/").pop();
        $('#dialogo-imprimir').modal({
            backdrop: 'static', keyboard: false
        }).css({
            width: function () {
                return $('#dialogo-imprimir').outerWidth();
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
    
    $('#dialogo-imprimir-imprimir').on('click', (function (e) {
        var deleteLinkObj = object;
        BlockUI($("#gridContainer").data().mensajeEspera);
        $.post(deleteLinkObj[0].href, { servidor: $("#serversLista").val(), impresoraId: idImpresoraSeleccionada }, function (data) {
            if (data == 'true') {
                MostrarAlertaExitosa();
                $('#dialogo-imprimir').modal('hide');
            } else {
                MostrarAlertaError(data);
                $('#dialogo-imprimir').modal('hide');
            }
        }).complete(function () {
            $.unblockUI();
        });
        e.preventDefault();
        e.stopPropagation();
    }));
    
    

});