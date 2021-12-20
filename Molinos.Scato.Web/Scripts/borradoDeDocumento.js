$(document).ready(function () {
    $('#dialogo-borrar-confirmar').off('click');

    $('#dialogo-borrar-confirmar').click(function () {
        BlockUI($('#message').val());
        $.post(deleteLinkObj[0].href, { motivo: $("#dialogo-borrar-motivo-text").val() }, function (data) { /*Post to action*/
            if (data.eliminado == 'true') {
                deleteLinkObj.closest("tr").hide(); /*Hide Row*/
                $("#dialogo-borrar-motivo-text").val("");
                if (data.advertencia != '') {
                    MostrarAlertaAdvertencia(data.advertencia);
                } else {
                    MostrarAlertaExitosa();
                }
            } else {
                MostrarAlertaError(data.eliminado);
            }
            
        })
            .error(
                function (message) {
                    alert(message.responseText);
                })
            .always(function() {
                $.unblockUI();
            });
            
        $('#dialogo-borrar').modal('hide');
    });
});