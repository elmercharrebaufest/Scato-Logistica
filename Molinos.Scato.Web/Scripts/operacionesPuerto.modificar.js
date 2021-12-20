jQuery(document).ready(function ($) {
    if ($('#IdFin').val() == "0") {
        $('.btn-primary').attr("href", "#");
        $("a[title='Modificar']").attr("href", "#");
    }
    var href = window.location.href;
    href = href.substring(0, href.lastIndexOf('/'));
    href = href.substring(0, href.lastIndexOf('/'));
    href = href + "/TodoEnviado/" + $("#Id").val() + "?cargaId=" + $("#Id").val() + "&numeroBalanza=" + $("#NumeroBalanza").val();
    $.get(href, function (data) {
        if (data) {
            $("#enviarASap").attr("disabled", "disabled");
        };
    });

    var numeroColumnaEnviadoASAP = 7;
    var numeroColumnaId = 2;

    $(document).on('click', "#enviarASap", function (e) {
        $("#cargandoEnviandoASap").toggle("slow");
        $("#enviarASap").attr("disabled", "disabled");
        $('#errores ul').empty();
        var href = window.location.href;
        href = href.substring(0, href.lastIndexOf('/'));
        href = href.substring(0, href.lastIndexOf('/'));
        href = href + "/EnviarASap/" + $("#Id").val() + "?cargaId=" + $("#Id").val() + "&numeroBalanza=" + $("#NumeroBalanza").val();
        $.get(href, function (data) { modificarEnviado(data); });



        e.preventDefault();
        return false;
    });


    function modificarEnviado(datos) {
        $('#grid tbody tr').each(function (index) {

            var idBalanzada = $('#grid tbody tr:nth-child(' + (index + 1) + ')  td:nth-child(' + numeroColumnaId + ')').text();
            var resultado = datos[idBalanzada];

            if (resultado == "Ok") {
                $('#grid tbody tr:nth-child(' + (index + 1) + ')  td:nth-child(' + numeroColumnaEnviadoASAP + ')').text("Si");
                $('#grid tbody tr:nth-child(' + (index + 1) + ') td.editar-borrar-columna a').remove();
            }
        });
        mostrarErrores(datos);
        $("#cargandoEnviandoASap").toggle("slow");
    }

    function mostrarErrores(datos) {
        var todosOk = true;
        $.each(datos, function (indice, item) {
            if (item !== "Ok") {
                $('#errores ul').append('<li> Error enviando a SAP la balanzada ' + indice + ". " + item + '</li>');
                todosOk = false;
            }
        });

        if (!todosOk) {
            $("#enviarASap").removeAttr("disabled");
        }
    }

});

