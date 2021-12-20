$(document).ready(function () {
    var object;
    $('#documentos').val(null);
    $('#tipos').val(null);
    
    $(document).on('click', '.ajax-imprimir-link', (function () {
        object = $(this);
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
        $.post(deleteLinkObj[0].href, { impresora: $("#ImpresoraDropDown").val(), cantCopias: $("#CantCopiasDropdown").val() }, function (data) {
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
    

    $(document).on('click', '.ajax-previsualizar-link', (function (e) {
        var obj = $(this)[0].href;
        BlockUI($("#gridContainer").data().mensajeEspera);
        Download(obj);
        return false;
    }));   
    DesbloquearBoton();
});

function DesbloquearBoton() {
    setInterval(function () {
        if ($.cookie('RetornoExportacion') != null) {
            $.unblockUI();
            $.removeCookie('RetornoExportacion', { path: '/' });
            $('.validation-summary-errors').html('');
        }
    }, 1000);
}

function Download(url) {
    document.getElementById('my_iframe').src = url;
};