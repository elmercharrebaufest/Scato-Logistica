$(document).ready(function () {

    $("#FechaDesde").on('dp.change', function (e) {

        var seleccionada = $("#FechaDesde").val() + "";
        var fecha = new Date(seleccionada.split('/')[2], parseInt(seleccionada.split('/')[1]) - 1, parseInt(seleccionada.split('/')[0]) + 60);

        CambiarFechaDesde($("#FechaDesde").val(), fecha);
    });

    $("#FechaHasta").on('dp.change', function (e) {
        var seleccionada = $("#FechaHasta").val() + "";
        var fecha = new Date(seleccionada.split('/')[2], parseInt(seleccionada.split('/')[1]) - 1, parseInt(seleccionada.split('/')[0]) - 60);

        CambiarFechaHasta($("#FechaHasta").val(), fecha);
    });



});

function CambiarFechaDesde(selected, hasta) {

    $('#FechaHasta').data("DateTimePicker").minDate(selected);
    $('#FechaHasta').data("DateTimePicker").maxDate(hasta);

}

function CambiarFechaHasta(selected, desde) {

    $('#FechaHasta').data("DateTimePicker").minDate(desde);
    $('#FechaHasta').data("DateTimePicker").maxDate(selected);
}