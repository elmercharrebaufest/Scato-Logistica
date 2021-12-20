$(document).ready(function () {
    setInterval(Refrescar, 30000);

    $(function () { $('#esGrano').bootstrapToggle() });
    $(function () { $('#mostrarIngresos').bootstrapToggle() });
    
    $("#mostrarIngresos").change(function () {
        $('#graficodeplantahead').block({
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
            url: urlCabezera,            
            type: 'GET',
            cache: false,
            data: { mostrarIngresos: $('#mostrarIngresos').is(':checked'), esGrano: $('#esGrano').is(':checked') }
        }).done(function (result) {
            $("#mostrarIngresos").prop('disabled', false);
            $("#esGrano").prop('disabled', false);
            $("#graficodeplantahead").html(result);
        });
    });


    $("#esGrano").change(function () {

        $('#graficodeplantahead').block({
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
            url: urlCabezera,
            success: function (data) {
                //Cargamos finalmente el contenido deseado
                $('#content').fadeIn(1000).html(data);
            },
            type: 'GET',
            cache: false,
            data: { mostrarIngresos: $('#mostrarIngresos').is(':checked'), esGrano: $('#esGrano').is(':checked') }
        }).done(function (result) {
            $("#mostrarIngresos").prop('disabled', false);
            $("#esGrano").prop('disabled', false);
            $("#graficodeplantahead").html(result);
        });
    });

    //if ($('#mostrarIngresos').is(':checked')) {
    //    $("#egresoIngresoTitulo").val("INGRESOS EN EL DÍA")
    //} else $("#egresoIngresoTitulo").val("EGRESOS EN EL DÍA")
   
});

function Refrescar() {
    $.get($("#links").data().urlCabezera, { mostrarIngresos: $('#mostrarIngresos').is(':checked'), esGrano: $('#esGrano').is(':checked') } ,function (data) {
        $("#graficodeplantahead").html(data);
    });
};


