$(document).ready(function () {
    
    $(".repesar").click(function () {
        $(".valorDecision").val("true");
    });
    
    $(".aceptar").click(function () {
        $(".valorDecision").val("false");
    });
    
    $(document).on('click', '.confirmar-boton', function () {
        $("#dialogo-confirmar-confirmar").val($(this).data().accion);
        $("#dialogo-confirmar-body").text($(this).data().mensaje);
        $("#dialogo-confirmar").modal('show');
        return false;
    });
});