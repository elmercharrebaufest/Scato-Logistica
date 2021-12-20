$(document).ready(function () {
    $("#btnPuestoCrear").prop("disabled", $('#puesto option').length == 0);
    $("#search-form").submit();

    $('#puesto').change(function() {
        var url = $('#CrearUrl').val() + "?puestoId=" + $(this).val();
        $('#btnPuestoCrear').attr("href", url);
        
        $("#search-form").submit();
    });
});