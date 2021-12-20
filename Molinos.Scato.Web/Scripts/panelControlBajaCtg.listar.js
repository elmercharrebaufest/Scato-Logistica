$(document).ready(function () {
    //Guardo los registros seleccionados utilizando pipe como separador
    $("input[type=checkbox]").change(function () {
        if ($(this).is(':checked')) {
            $("#transmisiones").val($("#transmisiones").val().replace($(this).val() + "|", ""));
            $("#transmisiones").val($("#transmisiones").val() + $(this).val() + "|");
            $("#cantidad").text(parseInt($("#cantidad").text()) + 1);
            $(this).closest('tr').children('td, th').css('background-color', '#F0F8FF');

        } else {
            $("#transmisiones").val($("#transmisiones").val().replace($(this).val() + "|", ""));
            $("#cantidad").text(parseInt($("#cantidad").text()) - 1);
            $(this).closest('tr').children('td, th').css('background', 'transparent');
        }
        DeshabilitarBotones();
    });
    
    //Recuerdo la seleccion mientras cambio de página
    $("input[type=checkbox]").each(function () {
        if ($("#transmisiones").val().indexOf("|" + $(this).val() + "|") > -1) {
            {
                $(this).attr("checked", true);
                $(this).closest('tr').children('td, th').css('background-color', '#F0F8FF');
            }
        }
    });

    DeshabilitarBotones();
});

//Deshabilito los botones cuando no hay selección
function DeshabilitarBotones() {
    if ($("#transmisiones").val().length > 1) {
        $(".transmitir").attr("disabled", false);
    } else {
        $(".transmitir").attr("disabled", true);
    }
}