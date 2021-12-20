$(document).ready(function () {

    /*Comportamiento de los campos para el envío automático de mails*/
    //if ($('#EnvioAutomaticoMail').prop('checked'))
    //    $(".envioMail").attr("disabled", false);
    //else
    //    $(".envioMail").attr("disabled", true);

    //$("#EnvioAutomaticoMail").change(function () {
    //    if ($('#EnvioAutomaticoMail').prop('checked'))
    //        $(".envioMail").attr("disabled", false);
    //    else
    //        $(".envioMail").attr("disabled", true);
    //});

    $.validator.addMethod("validarMail", function (value, element) {
        var resultado = true;
        if ($("#EnvioAutomaticoMail").prop('checked') && value.length == 0)
            resultado = false;
        return resultado;
    }, $("#Mail").data().mailError);



    $.validator.addMethod("validarOpcion", function (value, element) {
        var resultado = true;
        if ($("#EnvioAutomaticoMail").prop('checked') && !$("#Analisis").prop('checked') && !$("#Pesada").prop('checked'))
            resultado = false;
        if (!resultado) {
            element.parentElement.className = "control-group error";
        } else {
            element.parentElement.className = "control-group";
        }
        return resultado;
    }, $("#Analisis").data().opcionError);

});