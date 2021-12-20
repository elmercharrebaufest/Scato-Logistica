$(document).ready(function () {
    
    //Agrego checkbox en el header de la primer columna
    //$("#grid thead tr th:first").html("<input type='checkbox' id='SeleccionarTodos' checked/>"); Quitado por nueva especificacion
    
    //Comenzar con todos los registros seleccionados
    //$(".columna-checkbox").prop("checked", false);
    //$(".columna-checkbox").each(function () {
    //    $("#muestras").val($("#muestras").val().replace($(this).val() + "|", ""));
    //    $("#muestras").val($("#muestras").val() + $(this).val() + "|");
    //});
    $(".columna-checkbox").closest('tr').children('td, th').css('background-color', '#F0F8FF');
    DeshabilitarBotones();
    
    //Guardo los registros seleccionados utilizando pipe como separador
    $(".columna-checkbox").change(function () {
        if ($(this).is(':checked')) {
            $("#muestras").val($("#muestras").val().replace($(this).val() + "|", ""));
            $("#muestras").val($("#muestras").val() + $(this).val() + "|");
            $(this).closest('tr').children('td, th').css('background-color', '#F0F8FF');

            //if ($("#grid tbody tr").length == $(".columna-checkbox:checkbox:checked").length) {
            //    $("#SeleccionarTodos").prop("checked", true);  
            //}

        } else {
            $("#muestras").val($("#muestras").val().replace($(this).val() + "|", ""));
            $(this).closest('tr').children('td, th').css('background', 'transparent');
            //$("#SeleccionarTodos").prop("checked", false);
        }
        DeshabilitarBotones();
    });

    //Escondo botones cuando no hay elementos en grilla
    if ($(".columna-checkbox").length > 0) {
        $(".acciones").show();
    } else {
        $(".acciones").hide();
    }

    //Recuerdo la seleccion mientras cambio de página
    $(".columna-checkbox").each(function () {
        if ($("#muestras").val().indexOf("|" + $(this).val() + "|") > -1) {
            {
                $(this).attr("checked", true);
                $(this).closest('tr').children('td, th').css('background-color', '#F0F8FF');
            }
        }
    });
    
    //$("#SeleccionarTodos").change(function () {
    //    if ($(this).is(':checked')) {
    //        $(".columna-checkbox").prop("checked", true);
    //        $(".columna-checkbox").trigger("change");
    //        DeshabilitarBotones();
    //    } else {
    //        $(".columna-checkbox").prop("checked", false);
    //        $(".columna-checkbox").trigger("change");
    //        DeshabilitarBotones();
    //    }
    //});
});

//Deshabilito los botones cuando no hay selección
function DeshabilitarBotones() {
    if ($("#muestras").val().length > 1) {
        $("#btnLiberarCasilleros").attr("disabled", false);
        $("#ImprimirMuestraAEliminar").attr("disabled", false);
    } else {
        $("#btnLiberarCasilleros").attr("disabled", true);
        $("#ImprimirMuestraAEliminar").attr("disabled", true);
    }
}