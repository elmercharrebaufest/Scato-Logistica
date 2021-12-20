
$(document).ready(function () {
    $("#centrosDropdown li").click(function () {
    });
    
    $("#mostrarIngresos").change(function () {
        $('#panel').block({
            message: '<div>Cargando...</div>',
            css: {
                'width': '150px',
                'height': '25px',
                'font-size': '100%',
                'font- family': 'Arial, Helvetica, sans- serif',
                'backgroundColor': '#f0eeeb', 'color': '#00000',
                'font-weight': 'bolder'
            }
        });
        $("#mostrarIngresos").prop('disabled', true);
        $("#esGrano").prop('disabled', true);
        $.ajax({
            url: menu,
            type: 'GET',
            cache: false,
            data: { mostrarIngresos: $('#mostrarIngresos').is(':checked'), esGrano: $('#esGrano').is(':checked') }
        }).done(function (result) {
            $("#mostrarIngresos").prop('disabled', false);
            $("#esGrano").prop('disabled', false);
            $("#panel").html(result);
        });
    });


    $("#esGrano").change(function () {
        $('#panel').block({
            message: '<div>Cargando...</div>',
            css: {
                'width': '150px',
                'height': '25px',
                'font-size': '100%',
                'font- family': 'Arial, Helvetica, sans- serif',
                'backgroundColor': '#f0eeeb', 'color': '#00000',
                'font-weight': 'bolder'
            }
        });
        $("#mostrarIngresos").prop('disabled', true);
        $("#esGrano").prop('disabled', true);
        $.ajax({
            url: menu,
            type: 'GET',
            cache: false,
            data: { mostrarIngresos: $('#mostrarIngresos').is(':checked'), esGrano:  $('#esGrano').is(':checked') }
        }).done(function (result) {
            $("#mostrarIngresos").prop('disabled', false);
            $("#esGrano").prop('disabled', false);
            $("#panel").html(result);
        });
    });

});

