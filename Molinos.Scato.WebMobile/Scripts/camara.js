$(document).ready(function () {
    setInterval(function () {
        try {
            $.getJSON($('#imagen-camara').data().urlFoto + "?timestamp=" + new Date().getTime() + "&codigo=" + $("#camara").val(), null,
                function (data) {
                    $('#imagen-camara').html('<img src="data:image/jpeg;base64,' + data +'" alt="Error al obtener imagen" style="max-height: 560px; margin-bottom: 10%;" />');
                });
        }
        catch (err) {
            
        }

    }, 1000);
});

