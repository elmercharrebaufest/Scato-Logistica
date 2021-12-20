$(document).ready(function () {
    $("#Manual").addClass("hide");

    $("#boton-manual").on('click', function () {
        $("#Manual").removeClass("hide");
        $("#Automatica").addClass("hide");
        $("#esManual").val(true);
        $("#numeroCot").focus();
        return false;
    });
    $("#boton-volver").on('click', function () {
        $("#Manual").addClass("hide");
        $("#Automatica").removeClass("hide");
        $("#esManual").val(false);
        return false;
    });
});