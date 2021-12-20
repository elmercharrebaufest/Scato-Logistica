$(document).ready(function () {
    $(document).on('click', '.botonCerear',(function () {
        $("#BalanzaId").val($(this).data().botonCerearId);
        $('#dialogo-Cerear').modal({
            backdrop: 'static', keyboard: false
        }).css({
            width: function () {
                return $('#dialogo-Cerear').outerWidth();
            }, 'margin-left': function () {
                return -($(this).width() / 2);
            },
            'top': '50%',
            'margin-top': function () {
                return -($(this).height() / 2);
            }
        });
    }));
    
    $('#dialogo-cerear-cerear').on('click', (function () {
        $.getJSON($("#cerear").data().cerearBalanzaUrl, { balanzaId: $("#BalanzaId").val() }, function (data) {
            if (data.data == 'true') {
                MostrarAlertaExitosa();
                $('#dialogo-Cerear').modal('hide');
            } else {
                MostrarAlertaError(data.data);
                $('#dialogo-Cerear').modal('hide');
            }
        })
        .error(
            function (message) {
                alert(message.responseText);
            });
    }));
});