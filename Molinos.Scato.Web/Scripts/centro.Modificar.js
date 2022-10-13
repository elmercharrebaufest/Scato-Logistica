$(document).ready(function() {

    $("a.ajax-editar-link").removeClass("ajax-editar-link").addClass("ajax-editar-centro-link");

    $(document).on('click', '.ajax-editar-centro-link', function() {
        $.get(this.href, cargarPartialEditar);
        return false;
    });

    $("#regresar").on("click", function () {
        window.location = $("#urlIndex").val();
    });

    $("#cancelar").on("click", function () {
        $('.resultadoCancelada').val("Cancelada");
        $('.resultadoExitosa').val("");
        $('#search-form').submit();
    });
    
    $("#CodigoPostal").mask("a9999aaa");
    $("#CuitExportador").mask("99-99999999-9");

function cargarPartialEditar(data) {
    $('#gridContainer').html(data);
}
});


function redireccionarSiFueExitoso(respuesta) {
    if (respuesta == window.ajaxEditSuccess) {
        MostrarAlertaExitosa();
        window.location = $("#urlIndex").val();
    }
}