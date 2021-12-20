function descargarTemplate() {
    Download(urlTemplate);
}

$(document).ready(function () {
    var object;
    
    $(document).on('click', '#imprimirEtiquetas', (function () {
        object = $(this);

        if (hayEtiquetas) {
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
        } else {
            MostrarAlertaError("No hay etiquetas para imprimimir");
        }
        
        return false;
    }));

    $('#dialogo-imprimir-imprimir').on('click', (function (e) {
        url = $("#diag-imprimir").data("imprimirUrl");
        var deleteLinkObj = object;
        BlockUI($("#gridContainer").data().mensajeEspera);
        $.post(url, { impresora: $("#ImpresoraDropDown").val() }, function (data) {
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

    const fileInput = document.querySelector('#file-js-example input[type=file]');
    $("#file-js-example").change(function () {

        if (fileInput.files.length > 0) {
            const fileName = document.querySelector('#file-js-example .file-name');
            fileName.textContent = fileInput.files[0].name;
            $("#uploadFormEtiquetaPuerto").submit();
        }
    });
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