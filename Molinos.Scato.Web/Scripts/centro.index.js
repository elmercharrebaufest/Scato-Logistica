$(document).ready(function() {

    $("a.ajax-editar-link").removeClass("ajax-editar-link").addClass("ajax-editar-centro-link");

    $(document).on('click', '.ajax-editar-centro-link', function() {
        $.get(this.href, cargarPartialEditar);
        return false;
    });
});

function cargarPartialEditar(data) {
    $('#gridContainer').html(data);
}