jQuery(document).ready(function ($) {

    var urls = {
        reiniciar: $('#links').data('url-reiniciar')
    };

    $('#btnReiniciar').on('click', function () {
        if (!confirm('¿Está seguro que desea reiniciar el servicio Intercomunicador?')) {
            return;
        }

        BlockUI();

        $.ajax({
            url: urls.reiniciar,
            type: 'POST',
            dataType: 'json',
            error: function () {
                MostrarAlertaError('Ocurrió un error al intentar reiniciar el servicio Intercomunicador.');
            },
            success: function (data) {
                if (data.exito) {
                    MostrarAlertaExitosa(data.mensaje);
                } else {
                    MostrarAlertaError(data.mensaje);
                }
            }
        }).always(function () {
            $.unblockUI();
        });
    });

});
