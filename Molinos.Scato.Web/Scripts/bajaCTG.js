$(document).ready(function() {
    if ($("#verReintentar").val() == "True" && $("#postDeManual").val() == "True") {
        $("#Automatica").addClass("hide");
    } else if ($("#verReintentar").val() == "True") {
        $("#Manual").addClass("hide");
    } else {
        $("#Automatica").addClass("hide");
        $("#boton-volver").addClass("hide");
    }

    $("#boton-manual").on('click', function() {
        $("#Manual").removeClass("hide");
        $("#Automatica").addClass("hide");
        $("#CodigoDeBaja").focus();
        return false;
    });
    $("#boton-volver").on('click', function() {
        $("#Manual").addClass("hide");
        $("#Automatica").removeClass("hide");
        return false;
    });
});