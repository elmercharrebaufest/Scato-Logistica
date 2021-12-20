$(document).ready(function () {

    $('#tablaJobs tr a').on('click', (function () {
        $.get($(this).attr('href'), cargarDialogoVer);
        return false;
    }));
    $('#refrescarColaBtn').on('click', (function () {
        $.get($('#verColaUrl').val(), cargarDialogoVer);
        return true;
    }));
});