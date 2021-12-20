$(document).ready(function () {
    $("#Descripcion").mask("99-99");
    $.validator.addMethod("cosechaValidacion", function (value, element) {
        var cosecha = $("#Descripcion").val().split('-');
        if (parseInt(cosecha[0]) > parseInt(cosecha[1]))
            return false;
        return true;
    }, $("#Descripcion").data().error);

    $.validator.addMethod("cosechaValidacionDiferencia", function (value, element) {
        var cosecha = $('#Descripcion').val().split('-');
        if (parseInt(cosecha[1]) - parseInt(cosecha[0]) !== 1)
            return false;
        return true;
    }, $("#Descripcion").data().errordiferencia);
});